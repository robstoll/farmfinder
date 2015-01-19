using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CH.Tutteli.FarmFinder.Website.Models
{
    public class Farm
    {
        public int FarmId { get; set; }

        /// <summary>
        /// Name of the farm as registered in farm finder.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Latitude of the farm using -90/90 convention.
        /// </summary>
        [Required]
        [Range(-90,90)]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude of the farm using -180/180 convention.
        /// </summary>
        [Required]
        [Range(-180, 180)]
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

        public virtual ICollection<Product> Products { get; set; }

        public Farm()
        {
            Products = new List<Product>();
        }
    }
}