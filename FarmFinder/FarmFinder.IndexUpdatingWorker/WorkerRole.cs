using System;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
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
using Lucene.Net.Store;
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

        private const string QueueName = "farmfinder";
        private const string TopicName = "recreateindex";

        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);

        private QueueClient _queueClient;
        private TopicClient _topicClient;
        private AzureDirectory _azureDirectory;
        private IndexWriter _indexWriter;

        public override void Run()
        {
            _queueClient.OnMessage(async receivedMessage =>
            {
                var sequenceNumber = receivedMessage.SequenceNumber;
                try
                {
                    var dto = receivedMessage.GetBody<UpdateIndexDto>();
                    await HandleMessage(dto);
                    InformWebRolesAboutNewIndex();
                }
                catch(Exception ex)
                {
                    //no idea why it does not work but well, log it
                    Trace.TraceWarning("Exception occurred during the read of message '" + sequenceNumber + "': " + ex.Message);
                }
            });

            _completedEvent.WaitOne();
        }

        private void InformWebRolesAboutNewIndex()
        {
            _topicClient.Send(new BrokeredMessage(true));
        }

        private async Task HandleMessage(UpdateIndexDto dto)
        {
            using (var db = new ApplicationDbContext())
            {
                Farm farm;
                switch (dto.UpdateMethod)
                {
                    case EUpdateMethod.Create:
                        farm =
                            await db.Farms.Include(f => f.Products).Where(f => f.FarmId == dto.FarmId).FirstAsync();
                        //good idea but different time zone or system time on different servers do not allow to use a timestamp to filter out old messages
                        if (farm != null) // && farm.UpdateDateTime == dto.UpdateTime) 
                        {
                            AddFarmToIndex(farm, _indexWriter, db);
                            await db.SaveChangesAsync();
                            _indexWriter.Commit();
                        }
                        break;
                    case EUpdateMethod.Update:
                        farm =
                            await db.Farms.Include(f => f.Products).Where(f => f.FarmId == dto.FarmId).FirstAsync();
                        if (farm != null) // && farm.UpdateDateTime == dto.UpdateTime)
                        {
                            UpdateFarmInIndex(farm, _indexWriter, db);
                            await db.SaveChangesAsync();
                            _indexWriter.Commit();
                        }
                        break;
                    case EUpdateMethod.Delete:
                        farm = await db.Farms.FindAsync(dto.FarmId);
                        if (farm != null)
                        {
                            DeleteFarmFromIndex(farm, _indexWriter, db);
                            await db.SaveChangesAsync();
                            _indexWriter.Commit();
                        }
                        break;
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            string connectionString = CloudConfigurationManager.GetSetting("ServiceBus.QueueConnectionString");
            _queueClient = QueueClient.CreateFromConnectionString(connectionString, QueueName);

            connectionString = CloudConfigurationManager.GetSetting("ServiceBus.TopicConnectionString");
            _topicClient = TopicClient.CreateFromConnectionString(connectionString, TopicName);

            _azureDirectory = CreateAzureDirectory();
            _indexWriter = CreateIndexWriter();

            Task.Run(async () => await InitialiseIndex()).Wait();

            return base.OnStart();
        }

        private AzureDirectory CreateAzureDirectory()
        {
            CloudStorageAccount cloudStorageAccount;
            CloudStorageAccount.TryParse(CloudConfigurationManager.GetSetting("blobStorage"), out cloudStorageAccount);
            return new AzureDirectory(cloudStorageAccount, "FarmCatalog");
        }

        private async Task InitialiseIndex()
        {
            int totalHits = 0;
            try
            {
                var searcher = new IndexSearcher(_azureDirectory);
                var hits = searcher.Search(
                    new QueryParser(Version.LUCENE_30, "name", new StandardAnalyzer(Version.LUCENE_30)).Parse("*:*"), 1);
                totalHits = hits.TotalHits;
            }
            catch (FileNotFoundException)
            {
                //that's ok, means no index was created yet
            }

            using (var db = new ApplicationDbContext())
            {
                if (totalHits > 0)
                {
                    var farmTotal = await db.Farms.CountAsync();
                    var totalAdded = await GetNewAddedQueryable(db).CountAsync();
                    if (farmTotal == totalHits + totalAdded)
                    {
                        await UpdateIndex(db);
                    }
                    else
                    {
                        //Unfortunately not consistent index
                        _indexWriter.DeleteAll();
                        await CreateIndex(db);
                    }
                }
                else
                {
                    await CreateIndex(db);
                }
            }

            InformWebRolesAboutNewIndex();
        }

        private static IQueryable<Farm> GetNewAddedQueryable(ApplicationDbContext db)
        {
            return db.Farms.Where(f => f.UpdateDateTime > f.IndexDateTime && f.IndexDateTime == DateTime.MinValue && f.DeleteWhenRemovedFromIndex == false);
        }

        private async Task UpdateIndex(ApplicationDbContext db)
        {
            var farms = await GetNewAddedQueryable(db).ToListAsync();
            Parallel.ForEach(farms, farm =>
            {
                try
                {
                    AddFarmToIndex(farm, _indexWriter, db);
                }
                catch (Exception)
                {
                    //TODO error handling. Should certainly not abort the adddition of documents
                }
            });

            farms = await db.Farms.Include(f => f.Products).Where(f => f.UpdateDateTime > f.IndexDateTime && f.IndexDateTime != DateTime.MinValue).ToListAsync();
            Parallel.ForEach(farms, farm =>
            {
                try
                {
                    UpdateFarmInIndex(farm, _indexWriter, db);
                }
                catch (Exception)
                {
                    //TODO error handling. Should certainly not abort the adddition of documents
                }
            });

            farms = await db.Farms.Where(f => f.DeleteWhenRemovedFromIndex).ToListAsync();
            Parallel.ForEach(farms, farm =>
            {
                try
                {
                    DeleteFarmFromIndex(farm, _indexWriter, db);
                }
                catch (Exception)
                {
                    //TODO error handling. Should certainly not abort the adddition of documents
                }
            });

            await db.SaveChangesAsync();

            _indexWriter.Optimize();
            _indexWriter.Commit();
        }
        
        private void AddFarmToIndex(Farm farm, IndexWriter indexWriter, ApplicationDbContext db)
        {
            var doc = CreateDocumentFromFarm(farm);
            indexWriter.AddDocument(doc);
            db.Entry(farm).State = EntityState.Modified;
            farm.IndexDateTime = DateTime.Now;
        }

        private void UpdateFarmInIndex(Farm farm, IndexWriter indexWriter, ApplicationDbContext db)
        {
            var doc = CreateDocumentFromFarm(farm);
            indexWriter.UpdateDocument(new Term("id", farm.FarmId.ToString()), doc);
            db.Entry(farm).State = EntityState.Modified;
            farm.IndexDateTime = DateTime.Now;
        }

        private void DeleteFarmFromIndex(Farm farm, IndexWriter indexWriter, ApplicationDbContext db)
        {
            indexWriter.DeleteDocuments(new Term("id", farm.FarmId.ToString()));
            db.Farms.Remove(farm);
        }

        private async Task CreateIndex(ApplicationDbContext db)
        {
            var farms = await db.Farms.Include(f => f.Products).ToListAsync();
            Parallel.ForEach(farms, farm =>
            //foreach (var farm in farms)
            {
                try
                {
                    if (!farm.DeleteWhenRemovedFromIndex)
                    {
                        AddFarmToIndex(farm, _indexWriter, db);
                    }
                    else
                    {
                        //since it is not yet in the index we can simply remove it from the db
                        db.Farms.Remove(farm);
                    }
                }
                catch (Exception e)
                {
                    //TODO error handling. Should certainly not abort the adddition of documents
                }
            });

            await db.SaveChangesAsync();
            _indexWriter.Optimize();
            _indexWriter.Commit();
        }

        private IndexWriter CreateIndexWriter()
        {
            var writer = new IndexWriter(
                _azureDirectory,
                new StandardAnalyzer(Version.LUCENE_30),
                true,
                new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH));
            //writer.UseCompoundFile = false;
            return writer;
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
            _completedEvent.Set();   
            if (_queueClient != null)
            {
                _queueClient.Close();
            }
            if (_indexWriter != null)
            {
                _indexWriter.Dispose();
            }
            if (_azureDirectory != null)
            {
                _azureDirectory.Dispose();
            }
            base.OnStop();
        }
    }
}
