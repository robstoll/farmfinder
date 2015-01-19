using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using CH.Tutteli.FarmFinder.Website.Models;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using WebApiContrib.Formatting.Jsonp;
using Version = Lucene.Net.Util.Version;

namespace CH.Tutteli.FarmFinder.SearchApi
{
    public class WebApiApplication : HttpApplication
    {
        public static AzureDirectory AzureDirectory { get; set; }

        public static readonly double MultiplyFactor = 1000000;

        protected async void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configuration.AddJsonpFormatter();

            CloudStorageAccount cloudStorageAccount;
            CloudStorageAccount.TryParse(CloudConfigurationManager.GetSetting("blobStorage"), out cloudStorageAccount);
            AzureDirectory = new AzureDirectory(cloudStorageAccount, "FarmCatalog");
            var indexWriter = new IndexWriter(
                AzureDirectory,
                new StandardAnalyzer(Version.LUCENE_30),
                true,
                new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH));
            

            var db = new ApplicationDbContext();
            var farms = await db.Farms.ToListAsync();
            foreach (var farm in farms)
            {
                var doc = new Document();
                doc.Add(new Field("id", farm.FarmId.ToString(), Field.Store.YES, Field.Index.NO, Field.TermVector.NO));
                doc.Add(new Field("name", farm.Name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
                //doc.Add(new NumericField("Latitude", 4, Field.Store.YES, false).SetIntValue((int)(farm.Latitude * MultiplyFactor)));
                //doc.Add(new NumericField("Longitude", 4, Field.Store.YES, false).SetIntValue((int)(farm.Longitude * MultiplyFactor)));
                doc.Add(new NumericField("latitude", Field.Store.YES, true).SetDoubleValue(farm.Latitude));
                doc.Add(new NumericField("longitude", Field.Store.YES, true).SetDoubleValue(farm.Longitude));
                indexWriter.AddDocument(doc);
            }

            indexWriter.Optimize();
            indexWriter.Dispose();
        }
    }
}