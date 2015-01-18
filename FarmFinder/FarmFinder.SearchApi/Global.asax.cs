﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace CH.Tutteli.FarmFinder.SearchApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            GlobalConfiguration
        .Configuration
        .Formatters
        .Insert(0, new Westwind.Web.WebApi.JsonpFormatter());
        }
    }
}
