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

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            QueueHelper.Initialise();
        }

        protected void Application_End()
        {
            QueueHelper.Dispose();
        }
    }
}
