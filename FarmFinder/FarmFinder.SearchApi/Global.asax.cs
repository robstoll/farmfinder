using System;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using CH.Tutteli.FarmFinder.Website.Models;
using Lucene.Net;
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
            using (var indexWriter = new IndexWriter(
                AzureDirectory,
                new StandardAnalyzer(Version.LUCENE_30),
                true,
                new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH)))
            {


                var db = new ApplicationDbContext();
                var farms = await db.Farms.Include(f => f.Products).ToListAsync();
                foreach (var farm in farms)
                {
                    var doc = new Document();
                    doc.Add(new Field("id", farm.FarmId.ToString(), Field.Store.YES, Field.Index.NO, Field.TermVector.NO));
                    doc.Add(new Field("name", farm.Name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                    doc.Add(new NumericField("latitude", Field.Store.YES, true).SetDoubleValue(farm.Latitude));
                    doc.Add(new NumericField("longitude", Field.Store.YES, true).SetDoubleValue(farm.Longitude));
                    foreach (var product in farm.Products)
                    {
                        doc.Add(new Field("product_id", product.ProductId.ToString(), Field.Store.YES, Field.Index.NO,
                            Field.TermVector.NO));
                        doc.Add(new Field("product_name", product.Name, Field.Store.YES, Field.Index.ANALYZED,
                            Field.TermVector.NO));
                        if (!string.IsNullOrEmpty(product.Description))
                        {
                            doc.Add(new Field("product_description", product.Description, Field.Store.YES,
                                Field.Index.ANALYZED, Field.TermVector.NO));
                        }
                        doc.Add(new Field("product_inStock", product.InStock.ToString(), Field.Store.YES,
                            Field.Index.ANALYZED, Field.TermVector.NO));
                    }
                    indexWriter.AddDocument(doc);
                }

                indexWriter.Optimize();
            }
        }
    }
}