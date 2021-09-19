using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    public partial class Node
    {
        public Node() {
            NodeImages = new List<NodeImage>();
            SubNodes = new List<SubNode>();
            ProductNodes = new List<ProductNode>();
        }
        [Key]
        public int Id { get; set; }
        public int? SequenceNumber { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }

        public int? NameId { get; set; }

        public int? DescriptionId { get; set; }

        [ForeignKey("NameId")]
        public  Text Name { get; set; }

        [ForeignKey("DescriptionId")]
        public  Text Description { get; set; }
        public  List<NodeImage> NodeImages { get; set; }
        
        [ForeignKey("OriginalNodeId")]
        public  List<SubNode> SubNodes { get; set; }
        [ForeignKey("NodeId")]
        public  List<ProductNode> ProductNodes { get; set; }
    }
}
