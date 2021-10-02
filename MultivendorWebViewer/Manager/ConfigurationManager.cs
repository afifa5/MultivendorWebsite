using MultivendorWebViewer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using MultivendorWebViewer;

namespace MultivendorWebViewer.Manager
{
    public class ConfigurationManager
    {
        
        public  ProfileSetting SiteProfile { get { return GetProfileSettings(Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data\\profile.config")); } }
        public virtual string GetHeaderLogoUrl(ApplicationRequestContext requsetContext,bool isSmall)
        {
            return UrlUtility.Action(requsetContext, "Image", "Content", new { imageId = 0, fileName = isSmall? SiteProfile.HeaderLogoSmall : SiteProfile.HeaderLogo });
        }
        private static ProfileSetting GetProfileSettings(string xmlFilename)
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
                //using (Stream reader = new FileStream(xmlFilename, FileMode.Open))
                //{
                //    // Call the Deserialize method to restore the object's state.
                //    return (ProfileSetting)serializer.Deserialize(reader);
                //}

            }
        }
    }
}