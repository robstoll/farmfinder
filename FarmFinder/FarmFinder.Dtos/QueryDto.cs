using System.ComponentModel.DataAnnotations;

namespace CH.Tutteli.FarmFinder.Dtos
{
    public class QueryDto
    {
        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        [Required]
        [Range(1, 100)]
        public int Radius { get; set; }

        public string Query { get; set; }
    }
}