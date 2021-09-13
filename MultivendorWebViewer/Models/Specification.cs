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

        public int Id { get; set; }
        [StringLength(50)]
        public string Identity { get; set; }
        public int? SpecificationTypeId { get; set; }

        public int? NameId { get; set; }

        public int SpecificationMode { get; set; }

        [ForeignKey("SpecificationTypeId")]
        public  SpecificationType SpecificationType { get; set; }

        [ForeignKey("NameId")]
        public  Text SpecificationText { get; set; }
    }
}
