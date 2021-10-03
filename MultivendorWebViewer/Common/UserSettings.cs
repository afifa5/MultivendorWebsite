#if NET5
using Microsoft.AspNetCore.Http;
#endif
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Common
{
    public class UserSettings
    {
        public UserSettings()
        {

            UICulture = "en-GB";
        }
        public string UICulture { get; set; }
    }
   
    //public abstract class UserSettingsProvider
    //{
     
    //    public virtual UserSettings Load(ApplicationRequestContext requestContext)
    //    {
    //        try
    //        {
    //            return LoadCore(requestContext) ?? new UserSettings();
    //        }
    //        catch (Exception exception)
    //        {
    //            return null;
    //        }
    //    }

    //    public virtual void Store(ApplicationRequestContext requestContext, UserSettings settings)
    //    {
    //        if (settings != null)
    //        {
    //            try
    //            {
    //                StoreCore(requestContext, settings);

    //                if (requestContext.HttpContext != null)
    //                {
    //                    //SetSettings(requestContext.HttpContext, settings);
    //                }
    //            }
    //            catch (Exception exception)
    //            {
    //            }
    //        }
    //        else
    //        {
    //            Clear(requestContext);
    //        }
    //    }

    //    public virtual void Clear(ApplicationRequestContext requestContext)
    //    {
    //        try
    //        {
    //            ClearCore(requestContext);

    //            if (requestContext.HttpContext != null)
    //            {
    //                //SetSettings(requestContext.HttpContext, null);
    //            }
    //        }
    //        catch (Exception exception)
    //        {
    //        }
    //    }

    //    protected abstract UserSettings LoadCore(ApplicationRequestContext requestContext);

    //    protected abstract void StoreCore(ApplicationRequestContext requestContext, UserSettings settings);

    //    protected abstract void ClearCore(ApplicationRequestContext requestContext);
    //}

    public class CookieUserSettingProvider /*: UserSettingsProvider*/
    {
        public CookieUserSettingProvider() {
            DefaultUserSetting = new UserSettings();
        }
        [XmlAttribute("cookie-name")]
        public string CookieName { get; set; }

        public UserSettings DefaultUserSetting { get; set; }
        public virtual string GetCookieName(ApplicationRequestContext requestContext)
        {
            if (CookieName != null) return CookieName;
            return "multivendor-web-user-settings"; 
        }

        public  UserSettings Load(ApplicationRequestContext requestContext)
        {
            if (requestContext != null && requestContext.HttpRequest != null)
            {
                var userSettingsCookie = requestContext.HttpRequest.Cookies[GetCookieName(requestContext)];

                if (userSettingsCookie != null && userSettingsCookie.Value != null)
                {
                    return JsonConvert.DeserializeObject(userSettingsCookie.Value, typeof(UserSettings)) as UserSettings;
                }
            }

            return null;
        }

        public void Store(ApplicationRequestContext requestContext, UserSettings settings)
        {
            if (requestContext != null && requestContext.HttpRequest != null)
            {
                /*Market Setting*/
               
                var cookie = new HttpCookie(GetCookieName(requestContext))
                {
                    Value = JsonConvert.SerializeObject(settings),
                    Expires = DateTime.Now.AddYears(1)
                };

                RequestItemHelper.SetItem(requestContext.HttpContext, "UserSettings", settings);

                requestContext.HttpContext.Response.SetCookie(cookie);
            }
        }

        public void Clear(ApplicationRequestContext requestContext)
        {
            if (requestContext != null && requestContext.HttpRequest != null)
            {
                requestContext.HttpRequest.Cookies.Remove(GetCookieName(requestContext));
            }
        }
    }

 
}