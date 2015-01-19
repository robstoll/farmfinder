using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CH.Tutteli.FarmFinder.Dtos;
using LonelySharp;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Version = Lucene.Net.Util.Version;

namespace CH.Tutteli.FarmFinder.SearchApi.Controllers
{
    public class SearchController : ApiController
    {
        private readonly double EARTH_RADIUS = 6371.01;

        /// <summary>
        /// Search farms around the point defined by Latitude and Longitude within the given Radius and filters the results according to the additional query parameters.
        /// </summary>
        /// <param name="queryDto"></param>
        /// <returns>The search results</returns>
        [ResponseType(typeof (FarmLocationDto))]
        public HttpResponseMessage GetFarms([FromUri] QueryDto queryDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var searcher = new IndexSearcher(WebApiApplication.AzureDirectory);
                    var hits = Query(queryDto, searcher);
                    var result = ConvertHitsToFarmLocationDtos(hits, searcher);
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                catch (ParseException ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        private static List<FarmLocationDto> ConvertHitsToFarmLocationDtos(ScoreDoc[] hits, IndexSearcher searcher)
        {
            var result = new List<FarmLocationDto>();
            for (int i = 0; i < hits.Length; ++i)
            {
                var doc = searcher.Doc(hits[i].Doc);

                double lat;
                double lng;
                if (double.TryParse(doc.Get("latitude"), out lat) && double.TryParse(doc.Get("longitude"), out lng))
                {
                    result.Add(new FarmLocationDto
                    {
                        Name = doc.Get("name"),
                        Latitude = lat,
                        Longitude = lng
                    });
                }
            }
            return result;
        }

        private static ScoreDoc[] Query(QueryDto queryDto, IndexSearcher searcher)
        {
            var geoLocation = GeoLocation.FromDegrees(queryDto.Latitude, queryDto.Longitude);
            var bounds = geoLocation.BoundingCoordinates(queryDto.Radius);
            var latFrom = bounds[0].getLatitudeInDegrees();
            var latTo = bounds[1].getLatitudeInDegrees();
            var lngFrom = bounds[0].getLongitudeInDegrees();
            var lngTo = bounds[1].getLongitudeInDegrees();
            var latQuery = NumericRangeQuery.NewDoubleRange("latitude", latFrom, latTo, true, true);
            var lngQuery = NumericRangeQuery.NewDoubleRange("longitude", lngFrom, lngTo, true, true);
            var query = new BooleanQuery
            {
                {latQuery, Occur.MUST},
                {lngQuery, Occur.MUST}
            };
            if (!string.IsNullOrEmpty(queryDto.Query))
            {
                var parser = new QueryParser(Version.LUCENE_30, "product", new StandardAnalyzer(Version.LUCENE_30));
                query.Add(parser.Parse(queryDto.Query), Occur.MUST);

            }
            var topDocs = searcher.Search(query, 100);
            var hits = topDocs.ScoreDocs;
            return hits;
        }
    }
}