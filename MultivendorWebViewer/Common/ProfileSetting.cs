using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Common
{
    public class ProfileSetting
    {
       public ProfileSetting() {
            PriceCurrency = "BDT";
        }
        [XmlElement("StartCatalogueId")]
        public int? StartCatalogueId { get; set; }
       
        [XmlElement("DataImageLocation")]
        public string ImageLocation { get; set; }
        
        [XmlElement("HeaderLogo")]
        public string HeaderLogo { get; set; }
        [XmlElement("HeaderLogoSmall")]
        public string HeaderLogoSmall { get; set; }
       
        [XmlElement("AvailableLanguage")]
        public string AvailableLanguage { get; set; }

        [XmlElement("PriceCurrency")]
        public string PriceCurrency { get; set; }


    }
}