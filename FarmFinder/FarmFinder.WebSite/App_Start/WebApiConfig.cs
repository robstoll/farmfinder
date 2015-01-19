using System.Web.Http;

namespace CH.Tutteli.FarmFinder.Website
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("SearchApi", "search/", new {controller = "Search"});
        }
    }
}