using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   
    public partial class Category
    {
        public Category() {
            CategoryImages = new List<CategoryImage>();
            CategoryNodes = new List<CategoryNode>();
        }
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }

        public int? NameId { get; set; }

        public int? DescriptionId { get; set; }

        [StringLength(50)]
        public string Label { get; set; }
        
        [ForeignKey("NameId")]
        public virtual Text Name { get; set; }
        
        [ForeignKey("DescriptionId")]
        public virtual Text Description { get; set; }
        [ForeignKey("CategoryId")]
        public virtual List<CategoryImage> CategoryImages { get; set; }
        [ForeignKey("CategoryId")]
        public virtual List<CategoryNode> CategoryNodes { get; set; }
    }
}
