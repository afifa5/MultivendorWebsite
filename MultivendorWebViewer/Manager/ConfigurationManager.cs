using MultivendorWebViewer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using MultivendorWebViewer;
using System.Globalization;

namespace MultivendorWebViewer.Manager
{
    public class ConfigurationManager:SingletonBase<ConfigurationManager>
    {
        
        public  ProfileSetting SiteProfile { get { return GetProfileSettings(Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data\\profile.config")); } }
        public virtual string GetHeaderLogoUrl(ApplicationRequestContext requsetContext,bool isSmall)
        {
            return UrlUtility.Action(requsetContext, "Image", "Content", new { imageId = 0, fileName = isSmall? SiteProfile.HeaderLogoSmall : SiteProfile.HeaderLogo });
        }
        public virtual string GetApplicationImage(ApplicationRequestContext requsetContext, string applicationImageName)
        {
            return UrlUtility.Action(requsetContext, "Image", "Content", new { imageId = 0, fileName = applicationImageName });
        }
        public virtual string GetCountryFlagUrlByCulture(string culturename)
        {
            string flagPath = "~/Content/Flag/";
            return string.Format("{0}{1}.svg", flagPath, culturename);


        }
        private static ProfileSetting GetProfileSettings(string xmlFilename)
        {
            return CacheManager.Default.Get<ProfileSetting>(string.Concat("ProfileSetting@", "MultivendorWeb"), CacheLocation.Application, () =>
            {
                if (string.IsNullOrEmpty(xmlFilename))
                {
                    return null;
                }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ProfileSetting));

                    using (FileStream stream = File.Open(xmlFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return (ProfileSetting)serializer.Deserialize(reader);
                        }
                    }

                }
            });
          
        }
    }
}