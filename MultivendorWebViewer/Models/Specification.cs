using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class Specification
    {
        public Specification()
        {
        }
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Identity { get; set; }
        [Required]
        public int SpecificationTypeId { get; set; }

        public int? NameId { get; set; }

        public int SpecificationMode { get; set; }
        public virtual SpecificationType SpecificationType { get; set; }

        [ForeignKey("NameId")]
        public virtual Text SpecificationText { get; set; }
    }
}
