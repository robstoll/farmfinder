using System.Web.Http;

namespace CH.Tutteli.FarmFinder.Website
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            config.Routes.MapHttpRoute(
                name: "SearchApi",
                routeTemplate: "api/{controller}/"
            );
        }
    }
}
