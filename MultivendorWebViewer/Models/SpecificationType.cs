using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    

    [Table("SpecificationType")]
    public partial class SpecificationType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SpecificationType()
        {
        }

        public int Id { get; set; }

        [Column("SpecificationType")]
        public int? SpecificationTypeMode { get; set; }

        public int? NameId { get; set; }

        

        public virtual Text SpecificationTypeText { get; set; }
    }
}
