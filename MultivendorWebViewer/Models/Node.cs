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
        }
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }

        public int? NameId { get; set; }

        public int? DescriptionId { get; set; }

        [ForeignKey("NameId")]
        public virtual Text Name { get; set; }

        [ForeignKey("DescriptionId")]
        public virtual Text Description { get; set; }
        public virtual List<NodeImage> NodeImages { get; set; }
    }
}
