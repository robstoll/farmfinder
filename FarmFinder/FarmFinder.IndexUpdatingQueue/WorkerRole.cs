using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CH.Tutteli.FarmFinder.Dtos;
using CH.Tutteli.FarmFinder.Website.Models;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
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
        private ApplicationDbContext _db = new ApplicationDbContext();
        private AzureDirectory _azureDirectory;

        private object _lockObject = new Object();
        private DateTime _lastUpdate;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            _client.OnMessage(receivedMessage =>
                {
                    try
                    {
                        //todo wait between updates
                        var last = _lastUpdate.AddMinutes(1);
                        if(last > DateTime.Now)
                        {
                            Thread.Sleep((int)(last - DateTime.Now).TotalMilliseconds);
                        }

                        HandleMessage(receivedMessage.GetBody<UpdateIndexDto>());
                    }
                    catch
                    {
                        //TODO error handling
                        // Handle any message processing specific exceptions here
                    }
                });

            _completedEvent.WaitOne();
        }

        private async void HandleMessage(UpdateIndexDto dto)
        {
            using (var indexWriter = CreateIndexWriter())
            {
                Farm farm;
                switch (dto.UpdateMethod)
                {
                    case EUpdateMethod.Create:
                        farm = await _db.Farms.Include(f => f.Products).Where(f => f.FarmId == dto.FarmId).FirstAsync();
                        indexWriter.AddDocument(CreateDocumentFromFarm(farm));
                        break;
                    case EUpdateMethod.Update:
                        farm = await _db.Farms.Include(f => f.Products).Where(f => f.FarmId == dto.FarmId).FirstAsync();
                        indexWriter.UpdateDocument(new Term("id",dto.FarmId.ToString()), CreateDocumentFromFarm(farm));
                        break;
                    case EUpdateMethod.Delete:
                        indexWriter.DeleteDocuments(new Term("id",dto.FarmId.ToString()));
                        farm = await _db.Farms.FindAsync(dto.FarmId);
                        _db.Farms.Remove(farm);
                        await _db.SaveChangesAsync();
                        break;
                }
            }
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

        private async void InitialiseAzureDirectory()
        {
            CloudStorageAccount cloudStorageAccount;
            CloudStorageAccount.TryParse(CloudConfigurationManager.GetSetting("blobStorage"), out cloudStorageAccount);
            _azureDirectory = new AzureDirectory(cloudStorageAccount, "FarmCatalog");

            using (var indexWriter = CreateIndexWriter())
            {
                indexWriter.DeleteAll();
            }

            var searcher = new IndexSearcher(_azureDirectory);
            var hits = searcher.Search(
                new QueryParser(Version.LUCENE_30, "name", new StandardAnalyzer(Version.LUCENE_30)).Parse("*:*"), 1);

            if (hits.TotalHits > 0)
            {
                await UpdateIndex();
            }
            else
            {
                await CreateIndex();
            }
        }

        private async Task UpdateIndex()
        {
            using (var indexWriter = CreateIndexWriter())
            {
                var farms = await _db.Farms.Include(f => f.Products).Where(f => f.UpdateDateTime > f.IndexDateTime && f.IndexDateTime == DateTime.MinValue).ToListAsync();
                Parallel.ForEach(farms, farm =>
                {
                    try
                    {
                        var doc = CreateDocumentFromFarm(farm);
                        indexWriter.AddDocument(doc);
                    }
                    catch (Exception e)
                    {
                        //TODO error handling. Should certainly not abort the adddition of documents
                    }
                });

                farms = await _db.Farms.Include(f => f.Products).Where(f => f.UpdateDateTime > f.IndexDateTime && f.IndexDateTime != DateTime.MinValue).ToListAsync();
                Parallel.ForEach(farms, farm =>
                {
                    try
                    {
                        var doc = CreateDocumentFromFarm(farm);
                        indexWriter.UpdateDocument(new Term("id", farm.FarmId.ToString()), doc);
                    }
                    catch (Exception e)
                    {
                        //TODO error handling. Should certainly not abort the adddition of documents
                    }
                });

                farms = await _db.Farms.Where(f => f.DeleteWhenRemovedFromIndex).ToListAsync();
                Parallel.ForEach(farms, farm =>
                {
                    try
                    {
                        indexWriter.DeleteDocuments(new Term("id",farm.FarmId.ToString()));
                        _db.Farms.Remove(farm);
                    }
                    catch (Exception e)
                    {
                        //TODO error handling. Should certainly not abort the adddition of documents
                    }
                });
                await _db.SaveChangesAsync();

                indexWriter.Optimize();
            }
        }

        private void CreateNew()
        {
            throw new NotImplementedException();
        }


        private async Task CreateIndex()
        {
            using (var indexWriter = CreateIndexWriter())
            {
                var farms = await _db.Farms.Include(f => f.Products).ToListAsync();
                Parallel.ForEach(farms, farm =>
                {
                    try
                    {
                        var doc = CreateDocumentFromFarm(farm);
                        indexWriter.AddDocument(doc);
                    }
                    catch (Exception e)
                    {
                        //TODO error handling. Should certainly not abort the adddition of documents
                    }
                });
                indexWriter.Optimize();
            }
        }

        private IndexWriter CreateIndexWriter()
        {
            return new IndexWriter(
                _azureDirectory,
                new StandardAnalyzer(Version.LUCENE_30),
                true,
                new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH));
        }

        private Document CreateDocumentFromFarm(Farm farm)
        {
            var doc = new Document();
            doc.Add(new Field("id", farm.FarmId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
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
            if (_client != null)
            {
                _client.Close();
            }
            if (_azureDirectory != null)
            {
                _azureDirectory.Dispose();
            }
            _db.Dispose();
            _completedEvent.Set();
            base.OnStop();
        }

       
    }
}
