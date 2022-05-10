using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace MultivendorWebViewer.Models
{
    

    public partial class Text
    {
        public Text() {
            TextTranslations = new List<TextTranslation>();
        }
       
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }
        [ForeignKey("TextId")]
        public  List<TextTranslation> TextTranslations { get; set; }

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
