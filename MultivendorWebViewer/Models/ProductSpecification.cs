using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    
    public partial class ProductSpecification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int SpecificationId { get; set; }
        public  Specification Specification { get; set; }
        public  Product Product { get; set; }
    }
}
