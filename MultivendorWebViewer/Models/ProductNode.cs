using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class ProductNode
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int NodeId { get; set; }
        [ForeignKey("ProductId")]
        public  Product Product { get; set; }
        
        [ForeignKey("NodeId")]
        public  Node Node { get; set; }

    }
}
