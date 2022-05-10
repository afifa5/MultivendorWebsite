using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.Models
{

    public partial class SubNode
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OriginalNodeId { get; set; }
        [Required]
        public int SubNodeItemId { get; set; }
        
        [ForeignKey("SubNodeItemId")]
        public  Node SubNodeItem { get; set; }
       
    }
}
