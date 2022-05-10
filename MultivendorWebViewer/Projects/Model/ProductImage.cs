using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.Models
{
   

    public partial class ProductImage
    {
        [Key]
        public int Id { get; set; }
        [Required]
      
        public int ProductId { get; set; }
        [Required]
      
        public int ImageId { get; set; }

        //Get a Image for each selection of specification
        public string SpecificationIds { get; set; }
        [ForeignKey("ImageId")]
        public  Image Image { get; set; }
        [ForeignKey("ProductId")]
        public  Product Product { get; set; }
    }
}
