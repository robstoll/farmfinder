using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CH.Tutteli.FarmFinder.Dtos;

namespace CH.Tutteli.FarmFinder.SearchApi.Controllers
{
    public class SearchController : ApiController
    {
        /// <summary>
        ///     Search farms around the point defined by Latitude and Longitude within the given Radius and filters the results
        ///     according to the additional query parameters.
        /// </summary>
        /// <param name="queryDto"></param>
        /// <returns>The search results</returns>
        [ResponseType(typeof(FarmLocationDto))]
        public HttpResponseMessage GetFarms([FromUri] QueryDto queryDto)
        {
            return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                "only here for documentation purposes");
        }
    }
}