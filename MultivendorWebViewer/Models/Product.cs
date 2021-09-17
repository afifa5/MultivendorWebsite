using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{

    public partial class Product
    {
        public Product()
        {
            
            ProductImages = new List<ProductImage>();
            ProductSpecifications = new List<ProductSpecification>();
        }
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }

        public int? NameId { get; set; }

        public int? DescriptionId { get; set; }

        [ForeignKey("NameId")]
        public virtual Text Name { get; set; }
        
        [ForeignKey("DescriptionId")]
        public virtual Text Description { get; set; }
        public virtual List<ProductImage> ProductImages { get; set; }
        public virtual List<ProductSpecification> ProductSpecifications { get; set; }
    }
}
