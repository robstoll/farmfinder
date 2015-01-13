using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearchApi.Models;

namespace SearchApi.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TryIt()
        {
            return View(new QueryDto { Radius = 30, Latitude = 48.306940, Longitude = 14.285830 });
        }
    }
}