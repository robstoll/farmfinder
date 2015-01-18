using System.Net.Http.Headers;
using System.Web.Http;

namespace CH.Tutteli.FarmFinder.SearchApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            config.Routes.MapHttpRoute(
                name: "SearchApi",
                routeTemplate: "{controller}/"
            );
        }
    }
}
