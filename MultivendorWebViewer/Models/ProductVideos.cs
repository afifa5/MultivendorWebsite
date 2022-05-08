using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class ProductVideo
    {
        [Key]
        public int Id { get; set; }
        [Required]
      
        public int ProductId { get; set; }
        [Required]
      
        public int VideoId { get; set; }

        //Get a Image for each selection of specification
        public string SpecificationIds { get; set; }
        [ForeignKey("VideoId")]
        public  Video Image { get; set; }
        [ForeignKey("ProductId")]
        public  Product Product { get; set; }
    }
}
