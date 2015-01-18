using System.ComponentModel.DataAnnotations;

namespace CH.Tutteli.FarmFinder.Website.Models
{
    public class Farm
    {
        public int FarmId { get; set; }

        [Required]
        /// <summary>
        /// Name of the farm as registered in farm finder.
        /// </summary>
        public string Name { get; set; }

        [Required]
        [Range(-90,90)]
        /// <summary>
        /// Latitude of the farm using -90/90 convention.
        /// </summary>
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        /// <summary>
        /// Longitude of the farm using -180/180 convention.
        /// </summary>
        public double Longitude { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string Zip { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [DataType(DataType.Url)]
        public string Website { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

    }
}