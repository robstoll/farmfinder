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
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private AzureDirectory _azureDirectory;

        private DateTime _lastUpdate;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            _client.OnMessage(async receivedMessage =>
                {
                    try
                    {
                        //todo wait between updates
                        var last = _lastUpdate.AddMinutes(1);
                        if(last > DateTime.Now)
                        {
                            Thread.Sleep((int)(last - DateTime.Now).TotalMilliseconds);
                        }
                        _lastUpdate = DateTime.Now;
                        var dto = receivedMessage.GetBody<UpdateIndexDto>();
                        await  HandleMessage(dto);
                        receivedMessage.Complete();  
                    }
                    catch(Exception ex)
                    {
                        //no idea why it does not work but well, log it and abandon the message
                        Trace.TraceWarning("Exception occurred during the read of message '" + receivedMessage.SequenceNumber + "': " + ex.Message);
                        //todo mabye need to abandon messages in the future, not yet sure when it makes sense
                        //receivedMessage.Abandon(); 
                    }
                });

            _completedEvent.WaitOne();
        }

        private async Task HandleMessage(UpdateIndexDto dto)
        {
            using (var indexWriter = CreateIndexWriter())
            {
                Farm farm;
                switch (dto.UpdateMethod)
                {
                    case EUpdateMethod.Create:
                        farm = await _db.Farms.Include(f => f.Products).Where(f => f.FarmId == dto.FarmId).FirstAsync();
                        if (farm != null && farm.UpdateDateTime == dto.UpdateTime)
                        {
                            AddDocToIndex(farm, indexWriter);
                        }
                        break;
                    case EUpdateMethod.Update:
                        farm = await _db.Farms.Include(f => f.Products).Where(f => f.FarmId == dto.FarmId).FirstAsync();
                        if (farm != null && farm.UpdateDateTime == dto.UpdateTime)
                        {
                            ModifyDocInIndex(farm, indexWriter);
                        }
                        break;
                    case EUpdateMethod.Delete:
                        farm = await _db.Farms.FindAsync(dto.FarmId);
                        if (farm != null)
                        {
                            indexWriter.DeleteDocuments(new Term("id", dto.FarmId.ToString()));
                            _db.Farms.Remove(farm);
                            await _db.SaveChangesAsync();
                        }
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

            Task.Run(async () => await InitialiseAzureDirectory()).Wait();

            return base.OnStart();
        }

        private async Task InitialiseAzureDirectory()
        {
            CloudStorageAccount cloudStorageAccount;
            CloudStorageAccount.TryParse(CloudConfigurationManager.GetSetting("blobStorage"), out cloudStorageAccount);
            _azureDirectory = new AzureDirectory(cloudStorageAccount, "FarmCatalog");


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
                        AddDocToIndex(farm, indexWriter);
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
                        ModifyDocInIndex(farm, indexWriter);
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
                        DeleteDocFromIndex(indexWriter, farm);
                    }
                    catch (Exception e)
                    {
                        //TODO error handling. Should certainly not abort the adddition of documents
                    }
                });

                indexWriter.Optimize();
            }

            await _db.SaveChangesAsync();
        }
        private void AddDocToIndex(Farm farm, IndexWriter indexWriter)
        {
            var doc = CreateDocumentFromFarm(farm);
            indexWriter.AddDocument(doc);
            _db.Entry(farm).State = EntityState.Modified;
            farm.IndexDateTime = DateTime.Now;
        }


        private void ModifyDocInIndex(Farm farm, IndexWriter indexWriter)
        {
            var doc = CreateDocumentFromFarm(farm);
            indexWriter.UpdateDocument(new Term("id", farm.FarmId.ToString()), doc);
            farm.IndexDateTime = DateTime.Now;
        }

        private void DeleteDocFromIndex(IndexWriter indexWriter, Farm farm)
        {
            indexWriter.DeleteDocuments(new Term("id", farm.FarmId.ToString()));
            _db.Farms.Remove(farm);
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
                        AddDocToIndex(farm, indexWriter);
                    }
                    catch (Exception e)
                    {
                        //TODO error handling. Should certainly not abort the adddition of documents
                    }
                });


                indexWriter.Optimize();
            }

            await _db.SaveChangesAsync();
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
