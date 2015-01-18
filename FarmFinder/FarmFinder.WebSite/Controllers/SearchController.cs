using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CH.Tutteli.FarmFinder.Dtos;

namespace CH.Tutteli.FarmFinder.Website.Controllers
{
    public class SearchController : ApiController
    {
        /// <summary>
        /// Search farms around the point defined by Latitude and Longitude within the given Radius and filters the results according to the additional query parameters.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="dto"></param>
        /// <returns>The search results</returns>
        [ResponseType(typeof(FarmLocationDto))]
        public HttpResponseMessage GetFarms([FromUri] QueryDto queryDto)
        {
            if (ModelState.IsValid)
            {
                var responseDto = new FarmLocationDto
                {
                    Name = "Dummy",
                    Latitude = queryDto.Latitude - 0.001,
                    Longitude = queryDto.Longitude + 0.001
                };
                return Request.CreateResponse(HttpStatusCode.OK, new[] { responseDto, responseDto });
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }
    }
}