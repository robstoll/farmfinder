using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CH.Tutteli.FarmFinder.Dtos;
using LonelySharp;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Util;

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
        [ResponseType(typeof (FarmLocationDto))]
        public HttpResponseMessage GetFarms([FromUri] QueryDto queryDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ScoreDoc[] hits = Query(queryDto);
                    List<FarmLocationDto> result = ConvertHitsToFarmLocationDtos(hits);
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                catch (ParseException ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        private static ScoreDoc[] Query(QueryDto queryDto)
        {
            var searcher = WebApiApplication.Searcher;
            GeoLocation geoLocation = GeoLocation.FromDegrees(queryDto.Latitude, queryDto.Longitude);
            GeoLocation[] bounds = geoLocation.BoundingCoordinates(queryDto.Radius);
            double latFrom = bounds[0].getLatitudeInDegrees();
            double latTo = bounds[1].getLatitudeInDegrees();
            double lngFrom = bounds[0].getLongitudeInDegrees();
            double lngTo = bounds[1].getLongitudeInDegrees();
            NumericRangeQuery<double> latQuery = NumericRangeQuery.NewDoubleRange("latitude", latFrom, latTo, true, true);
            latQuery.Boost = 1.5f;
            NumericRangeQuery<double> lngQuery = NumericRangeQuery.NewDoubleRange("longitude", lngFrom, lngTo, true,
                true);
            lngQuery.Boost = 1.5f;
            var query = new BooleanQuery
            {
                {latQuery, Occur.MUST},
                {lngQuery, Occur.MUST}
            };
            if (!string.IsNullOrEmpty(queryDto.Query))
            {
                var standardAnalyzer = new StandardAnalyzer(Version.LUCENE_30);
                var innerQuery = new BooleanQuery
                {
                    {
                        new QueryParser(Version.LUCENE_30, "product_name", standardAnalyzer).Parse(queryDto.Query),
                        Occur.SHOULD
                    },
                    {
                        new QueryParser(Version.LUCENE_30, "product_description", standardAnalyzer).Parse(queryDto.Query),
                        Occur.SHOULD
                    },
                };
                if (!queryDto.Query.Contains("~"))
                {
                    Query q = new QueryParser(Version.LUCENE_30, "product_name", standardAnalyzer).Parse(queryDto.Query + "~");
                    q.Boost = 0.75f;
                    innerQuery.Add(q, Occur.SHOULD);
                    Query q2 = new QueryParser(Version.LUCENE_30, "product_description", standardAnalyzer).Parse(queryDto.Query + "~");
                    q2.Boost = 0.75f;
                    innerQuery.Add(q2, Occur.SHOULD);
                }
                query.Add(innerQuery, Occur.MUST);
            }
            TopDocs topDocs = searcher.Search(query, 100);
            ScoreDoc[] hits = topDocs.ScoreDocs;
            return hits;
        }

        private static List<FarmLocationDto> ConvertHitsToFarmLocationDtos(ScoreDoc[] hits)
        {
            var searcher = WebApiApplication.Searcher;
            var result = new List<FarmLocationDto>();
            foreach (ScoreDoc scoreDoc in hits)
            {
                var doc = searcher.Doc(scoreDoc.Doc);

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
    }
}