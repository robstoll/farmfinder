using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CH.Tutteli.FarmFinder.Website.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        public bool InStock { get; set; }

        [Required]
        public int FarmRefId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "DateTime2")]
        public virtual DateTime UpdateDateTime { get; set; }


        [Required]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "DateTime2")]
        public virtual DateTime IndexDateTime
        {
            get;
            set;
        }

        [ForeignKey("FarmRefId")]
        public virtual Farm Farm { get; set; }
    }
}