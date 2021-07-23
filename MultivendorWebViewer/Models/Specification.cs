using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    [Table("Specification")]
    public partial class Specification
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Specification()
        {
        }

        public int Id { get; set; }

        public int? SpecificationTypeId { get; set; }

        public int? NameId { get; set; }

        public int SpecificationMode { get; set; }

       

        public virtual SpecificationType SpecificationType { get; set; }

        public virtual Text SpecificationText { get; set; }
    }
}
