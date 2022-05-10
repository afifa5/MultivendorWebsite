using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int? SequenceNumber { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }

        public int? NameId { get; set; }

        public int? DescriptionId { get; set; }
       
        public bool? IsActive { get; set; }

        [ForeignKey("NameId")]
        public virtual Text Name { get; set; }
        
        [ForeignKey("DescriptionId")]
        public  Text Description { get; set; }
        [ForeignKey("ProductId")]
        public  List<ProductImage> ProductImages { get; set; }
        
        [ForeignKey("ProductId")]
        public List<ProductVideo> ProductVideos { get; set; }
        
        [ForeignKey("ProductId")]
        public  List<ProductSpecification> ProductSpecifications { get; set; }
    }
}
