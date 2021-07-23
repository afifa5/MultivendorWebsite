using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    

    [Table("TextTranslation")]
    public partial class TextTranslation
    {
        public int Id { get; set; }

        public int? TextId { get; set; }

        [StringLength(10)]
        public string LanguageCode { get; set; }

        public string Translation { get; set; }

        public virtual Text Text { get; set; }
    }
}
