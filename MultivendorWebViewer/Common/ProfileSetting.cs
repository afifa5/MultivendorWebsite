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
            OrderProcess = "Product,ShippingBilling,Payment";
        }
        [XmlElement("StartCatalogueId")]
        public int? StartCatalogueId { get; set; }
       
        [XmlElement("DataImageLocation")]
        public string ImageLocation { get; set; }
        
        [XmlElement("HeaderLogo")]
        public string HeaderLogo { get; set; }
        [XmlElement("HeaderLogoSmall")]
        public string HeaderLogoSmall { get; set; }
        [XmlElement("LoginBackgroundImage")]
        public string LoginBackgroundImage { get; set; }
        [XmlElement("DefaultLoggedInAvatar")]
        public string DefaultLoggedInAvatar { get; set; }

        [XmlElement("AvailableLanguage")]
        public string AvailableLanguage { get; set; }

        [XmlElement("PriceCurrency")]
        public string PriceCurrency { get; set; }
        [XmlElement("OrderProcess")]
        public string OrderProcess { get; set; }
        
        [XmlElement("DisplayLocation")]
        public bool DisplayLocation { get; set; }
        
        [XmlElement("DisplayContact")]
        public bool DisplayContact { get; set; }

    }
    public static class KnownPropertyCodes
    {
        public const string AutoOrderNumberIterator = "_AUTOORDERNUMBERITERATOR";

    }
    public class AutoOrderNumberSettings
    {

        public bool AutoOrderNumberEnabled { get; set; } = true;
        public string Pattern { get; set; } = "DHM-{0:D18}";
        public int StartSequnceNumber { get; set; } = 1790;
    }
}