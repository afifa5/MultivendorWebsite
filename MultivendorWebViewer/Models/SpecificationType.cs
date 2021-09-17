using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    

    public partial class SpecificationType
    {
        public SpecificationType()
        {
        }
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Identity { get; set; }

        public int? SpecificationTypeMode { get; set; }
      
        public int? NameId { get; set; }

        [ForeignKey("NameId")]
        public virtual Text SpecificationTypeText { get; set; }
    }
}
