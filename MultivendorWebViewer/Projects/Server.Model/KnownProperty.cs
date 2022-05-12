using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.Server.Models
{

    public partial class KnownProperty
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string PropertyCode { get; set; }
        public string Text { get; set; }
        
        [Column(TypeName = "numeric")]
        public decimal Decimal { get; set; }

    }
}
