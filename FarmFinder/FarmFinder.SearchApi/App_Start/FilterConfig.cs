using System.Web.Mvc;

namespace CH.Tutteli.FarmFinder.SearchApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
