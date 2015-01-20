using System.Net;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace CH.Tutteli.FarmFinder.Website
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        const string QueueName = "farmfinder";

        public static QueueClient QueueClient { get; set; }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ServicePointManager.DefaultConnectionLimit = 12;

            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            QueueClient = QueueClient.CreateFromConnectionString(connectionString, QueueName);

        }

        protected void Application_End()
        {
            if (QueueClient != null)
            {
               QueueClient.Close();
            }
        }
    }
}
