using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CH.Tutteli.FarmFinder.Website.Models;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store.Azure;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Version = Lucene.Net.Util.Version;

namespace IndexUpdatingQueue
{
    public class WorkerRole : RoleEntryPoint
    {

        const string QueueName = "farmfinder";

        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);

        private QueueClient _client;
        private AzureDirectory _azureDirectory;

        private DateTime _lastUpdate;
        

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            _client.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        var last = _lastUpdate.AddMinutes(1);
                        if(last > DateTime.Now)
                        {
                            Thread.Sleep((int)(last - DateTime.Now).TotalMilliseconds);
                        }
                        Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());
                        CreateIndex().Wait();
                    }
                    catch
                    {
                        // Handle any message processing specific exceptions here
                    }
                });

            _completedEvent.WaitOne();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            _client = QueueClient.CreateFromConnectionString(connectionString, QueueName);

            InitialiseAzureDirectory();

            return base.OnStart();
        }

        private async void  InitialiseAzureDirectory()
        {
            CloudStorageAccount cloudStorageAccount;
            CloudStorageAccount.TryParse(CloudConfigurationManager.GetSetting("blobStorage"), out cloudStorageAccount);
            _azureDirectory = new AzureDirectory(cloudStorageAccount, "FarmCatalog");
            await CreateIndex();
        }

        private async Task CreateIndex()
        {
            using (var indexWriter = new IndexWriter(
                _azureDirectory,
                new StandardAnalyzer(Version.LUCENE_30),
                true,
                new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH)))
            {
                var db = new ApplicationDbContext();
                var farms = await db.Farms.Include(f => f.Products).ToListAsync();
                foreach (var farm in farms)
                {
                    var doc = CreateDocumentFromFarm(farm);
                    indexWriter.AddDocument(doc);
                }

                indexWriter.Optimize();
            }
        }

        private Document CreateDocumentFromFarm(Farm farm)
        {
            var doc = new Document();
            doc.Add(new Field("id", farm.FarmId.ToString(), Field.Store.YES, Field.Index.NO, Field.TermVector.NO));
            doc.Add(new Field("name", farm.Name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
            doc.Add(new NumericField("latitude", Field.Store.YES, true).SetDoubleValue(farm.Latitude));
            doc.Add(new NumericField("longitude", Field.Store.YES, true).SetDoubleValue(farm.Longitude));
            AddFieldIfNotNullOrEmpty(doc, "website", farm.Website);
            AddFieldIfNotNullOrEmpty(doc, "phone_number", farm.PhoneNumber);

            foreach (var product in farm.Products)
            {
                doc.Add(new Field("product_id", product.ProductId.ToString(), Field.Store.YES, Field.Index.NO, Field.TermVector.NO));
                doc.Add(new Field("product_inStock", product.InStock.ToString(), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                doc.Add(new Field("product_name", product.Name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                AddFieldIfNotNullOrEmpty(doc, "product_description", product.Description, Field.Index.ANALYZED);
            }
            return doc;
        }

        private void AddFieldIfNotNullOrEmpty(Document doc, string fieldName, string value, Field.Index index=Field.Index.NO)
        {
            if (!string.IsNullOrEmpty(value))
            {
                doc.Add(new Field(fieldName, value, Field.Store.YES, index, Field.TermVector.NO));
            }
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            _client.Close();
            _azureDirectory.Dispose();
            _completedEvent.Set();
            base.OnStop();
        }

       
    }
}
