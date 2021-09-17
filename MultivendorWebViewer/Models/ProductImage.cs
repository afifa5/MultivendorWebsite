using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

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
        public  Image Image { get; set; }
        public  Product Product { get; set; }
    }
}
