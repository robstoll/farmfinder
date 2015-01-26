using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Lucene.Net.Search;
using Lucene.Net.Store;
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

        private SubscriptionClient _topicClient;
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);
        private readonly ThrottlingCall _throttling = new ThrottlingCall(TimeSpan.FromMinutes(1));

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configuration.AddJsonpFormatter();

            Searcher = CreateIndexSearcher();
            //set DateTime - otherwise index will be updated immediatly even when minTimeBetweenCalls is not yet exceeded
            _throttling.LastTimeExecuteWasCalled = DateTime.Now;

            InitialiseTopic();
        }

        private IndexSearcher CreateIndexSearcher()
        {
            CloudStorageAccount cloudStorageAccount;
            CloudStorageAccount.TryParse(CloudConfigurationManager.GetSetting("blobStorage"), out cloudStorageAccount);
            var cacheDirectory = new RAMDirectory();
            var azureDirectory = new AzureDirectory(cloudStorageAccount, "FarmCatalog", cacheDirectory);
            return new IndexSearcher(azureDirectory, true);
        }

        public void InitialiseTopic()
        {
           
            ServicePointManager.DefaultConnectionLimit = 12;

            string connectionString = CloudConfigurationManager.GetSetting("ServiceBus.TopicConnectionString");
            _topicClient = SubscriptionClient.CreateFromConnectionString(connectionString, "recreateindex", "WebRoles");
            
            Task.Run(() =>
            {
                _topicClient.OnMessage(async receivedMessage =>
                {
                    var sequenceNumber = receivedMessage.SequenceNumber;
                    try
                    {
                        await _throttling.Execute(async () => ReCreateSearcher());
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

        private void ReCreateSearcher()
        {
            Searcher = CreateIndexSearcher();
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