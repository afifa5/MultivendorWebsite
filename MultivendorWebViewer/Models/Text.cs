using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;


namespace MultivendorWebViewer.Models
{
    

    [Table("Text")]
    public partial class Text
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Text()
        {
            TextTranslations = new HashSet<TextTranslation>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }

       
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TextTranslation> TextTranslations { get; set; }

        public string GetTranslation(string cultureCode) {
            var translatedSelection = TextTranslations.Where(i => i.LanguageCode == cultureCode).FirstOrDefault();
            if (translatedSelection == null)
            {
                if (TextTranslations.Count() > 0)
                {
                    translatedSelection = TextTranslations.Where(i => i.LanguageCode.Contains("en-")).FirstOrDefault();
                    if (translatedSelection == null)
                    {
                        return null;
                    }
                    else
                    {
                        return translatedSelection.Translation;
                    }

                }

            }
            else
            {
                return translatedSelection.Translation;
            }
            return null;
        }
    }
}
