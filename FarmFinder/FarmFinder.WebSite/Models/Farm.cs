using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CH.Tutteli.FarmFinder.Website.Models
{
    public class Farm
    {
        public int FarmId { get; set; }

        /// <summary>
        ///     Name of the farm as registered in farm finder.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Latitude of the farm using -90/90 convention.
        /// </summary>
        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        /// <summary>
        ///     Longitude of the farm using -180/180 convention.
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

        [Required]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "DateTime2")]
        public virtual DateTime UpdateDateTime
        {
            get; set;
        }

        /// <summary>
        /// Represents the DateTime when this entry was added to the lucene index last time, updated respectively.
        /// </summary>
        /// <remarks>
        /// Discrepancies between IndexDateTime and UpdateDateTime indicate that this entry is not up-to-date within the lucene index.
        /// Should the UpdateIndexingQueue-WorkerRole crash for whatever reason and the queue not get the corresponding message (because it was not available at the time).
        /// Then the UpdateIndexingQueue-WorkerRole will update the index upon next restart without the need of recreating the index entirely.
        /// </remarks>
        [Required]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "DateTime2")]
        public virtual DateTime IndexDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether this entry should be removed after the index was updated accordingly.
        /// </summary>
        /// <remarks>
        /// The field's mainly purpose is for crash-recovery. Instead of deleting an entity immediatly it is only marked as deleted as first step.
        /// The UpdateIndexingQueue-WorkerRole will delete this entry as soon as the lucene index is updated accordingly.
        /// In the case the queue would be full and the worker role would need to be restarted, then this flag indicates that the index needs to be updates accordingly.
        /// </remarks>
        [Required]
        public bool DeleteWhenRemovedFromIndex { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public Farm()
        {

            Products = new List<Product>();
        }
    }
}