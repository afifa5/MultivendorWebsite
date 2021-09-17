using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    

    public partial class TextTranslation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TextId { get; set; }

        [StringLength(10)]
        public string LanguageCode { get; set; }
        public string Translation { get; set; }
        public  Text Text { get; set; }
    }
}
