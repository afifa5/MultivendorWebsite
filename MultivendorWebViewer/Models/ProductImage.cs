using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class ProductImage
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }

        public int ImageId { get; set; }

        //Get a Image for each selection of specification
        public string SpecificationIds { get; set; }

        [ForeignKey("ImageId")]
        public  Image Image { get; set; }
       
        [ForeignKey("ProductId")]
        public  Product Product { get; set; }
    }
}
