using System.Web.Mvc;
using CH.Tutteli.FarmFinder.Dtos;

namespace CH.Tutteli.FarmFinder.Website.Controllers
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