using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{

   
    public partial class CategoryNode
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public int NodeId { get; set; }
        [ForeignKey("CategoryId")]
        public  Category Category { get; set; }
        [ForeignKey("NodeId")]
        public  Node Node { get; set; }
    }
}
