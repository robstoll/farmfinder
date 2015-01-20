using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Lucene.Net.Store.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using WebApiContrib.Formatting.Jsonp;

namespace CH.Tutteli.FarmFinder.SearchApi
{
    public class WebApiApplication : HttpApplication
    {
        public static AzureDirectory AzureDirectory { get; set; }

        protected async void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configuration.AddJsonpFormatter();

            CloudStorageAccount cloudStorageAccount;
            CloudStorageAccount.TryParse(CloudConfigurationManager.GetSetting("blobStorage"), out cloudStorageAccount);
            AzureDirectory = new AzureDirectory(cloudStorageAccount, "FarmCatalog");
        }
    }
}