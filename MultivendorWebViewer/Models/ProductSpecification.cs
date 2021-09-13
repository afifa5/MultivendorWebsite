using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    
    public partial class ProductSpecification
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int SpecificationId { get; set; }

        [ForeignKey("ProductId")]
        public  Product Product { get; set; }
        
        [ForeignKey("SpecificationId")]
        public  Specification Specification { get; set; }
    }
}
