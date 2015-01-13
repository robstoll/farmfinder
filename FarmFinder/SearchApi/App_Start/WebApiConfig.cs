using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SearchApi
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
