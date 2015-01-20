using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Lucene.Net.Search;
using Lucene.Net.Store.Azure;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using WebApiContrib.Formatting.Jsonp;

namespace CH.Tutteli.FarmFinder.SearchApi
{
    public class WebApiApplication : HttpApplication
    {

        public static IndexSearcher Searcher { get; set; }

        private AzureDirectory _azureDirectory;
        private SubscriptionClient _topicClient;
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);

        private DateTime _lastNotificationDateTime;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configuration.AddJsonpFormatter();

            Searcher = CreateIndexSearcher();
            _lastNotificationDateTime = DateTime.Now;

            InitialiseTopic();
        }

        private IndexSearcher CreateIndexSearcher()
        {
            CloudStorageAccount cloudStorageAccount;
            CloudStorageAccount.TryParse(CloudConfigurationManager.GetSetting("blobStorage"), out cloudStorageAccount);
            _azureDirectory = new AzureDirectory(cloudStorageAccount, "FarmCatalog");
            return new IndexSearcher(_azureDirectory);
        }

        public void InitialiseTopic()
        {
           
            ServicePointManager.DefaultConnectionLimit = 12;

            string connectionString = CloudConfigurationManager.GetSetting("ServiceBus.TopicConnectionString");
            _topicClient = SubscriptionClient.CreateFromConnectionString(connectionString, "recreateindex", "WebRoles");
            
            Task.Run(() =>
            {
                _topicClient.OnMessage(receivedMessage =>
                {
                    var sequenceNumber = receivedMessage.SequenceNumber;
                    try
                    {
                        if (_lastNotificationDateTime >= DateTime.Now.AddMinutes(-1))
                        {
                            //check if a notification is already pending
                            if (_lastNotificationDateTime < DateTime.Now)
                            {
                                _lastNotificationDateTime = DateTime.Now.AddMinutes(1);
                                Task.Run(() =>
                                {
                                    _lastNotificationDateTime = DateTime.Now;
                                    //wait 50 seconds until we create a new index. 
                                    //If a user is modifying something then it is likely that more things will be modified shortly
                                    //creating a new index searcher is also costly. Better wait a bit and create then
                                    Thread.Sleep(50*1000);
                                    Searcher = CreateIndexSearcher();
                                });
                            }
                        }
                        else
                        {
                            _lastNotificationDateTime = DateTime.Now;
                            Searcher = CreateIndexSearcher();
                        }
                    }
                    catch (Exception ex)
                    {
                        //no idea why it does not work but well, log it
                        Trace.TraceWarning("Exception occurred during the read of message '" + sequenceNumber + "': " + ex.Message);
                    }
                }, new OnMessageOptions {
                    AutoComplete = true
                });

                _completedEvent.WaitOne();
            });
            
        }

        protected void Application_End()
        {
            if (_topicClient != null)
            {
                _topicClient.Close();
            }
            _completedEvent.Set();
        }
    }
}