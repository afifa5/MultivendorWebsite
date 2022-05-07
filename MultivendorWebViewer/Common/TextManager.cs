using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Reflection;

namespace MultivendorWebViewer.Common
{
    public class TextManager:SingletonBase<TextManager>
    {
        public static ResourceManager ResourceManager { get { return new ResourceManager("MultivendorWebViewer.Resources.Strings",Assembly.GetExecutingAssembly()); } }
        public static TextManager Current
        {
            get
            {
                var requestContext = ApplicationRequestContext.GetContext(HttpContext.Current);
                if (requestContext == null) throw new ArgumentNullException("ApplicationRequestContext");
                return requestContext.TextManager;


            }
        }
        public virtual string GetText(string textkey)
        {
            if (textkey == null) return null;
            ApplicationRequestContext requestContext = ApplicationRequestContext.GetContext(HttpContext.Current);
            if (requestContext != null && ! string.IsNullOrEmpty(requestContext.SelectedCulture)) {
                return ResourceManager.GetString(textkey, new System.Globalization.CultureInfo(requestContext.SelectedCulture)) ?? textkey;
            }
            return ResourceManager.GetString(textkey, new System.Globalization.CultureInfo("en")) ?? textkey;
        }
    }
}