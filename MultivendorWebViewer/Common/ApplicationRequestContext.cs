using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using MultivendorWebViewer.Manager;

namespace MultivendorWebViewer.Common
{
    [NotMapped]
    public class ApplicationRequestContext
    {

        public ApplicationRequestContext(RequestContext requestContext)
        {
            RequestContext = requestContext;
            CategoryManager = new CategoryManager();
            TextManager = new TextManager();
            Configuration = new ConfigurationManager();
            ImageManager = new ImageManager();
            ProductManager = new ProductManager();


        }
        public static ApplicationRequestContext GetContext(HttpContextBase context)
        {
            return context != null && context.Handler != null ?  new ApplicationRequestContext(context.Request.RequestContext) : null;
        }
        public static ApplicationRequestContext GetContext(HttpContext context)
        {
            return context != null && context.Handler != null ? new ApplicationRequestContext(context.Request.RequestContext) : null;
        }
        public  CategoryManager CategoryManager { get; set; }
        public ProductManager ProductManager { get; set; }
        public ImageManager ImageManager { get; set; }
        public  TextManager TextManager { get; set; }
        public CookieUserSettingProvider UserSettingProvider { get { return new CookieUserSettingProvider(); } }
        public ConfigurationManager Configuration { get; set; }

        public RequestContext RequestContext { get; set; }
        public HttpContextBase HttpContext { get { return RequestContext != null ? RequestContext.HttpContext : null; } }

        public HttpRequestBase HttpRequest { get { return RequestContext != null && RequestContext.HttpContext != null ? RequestContext.HttpContext.Request : null; } }
        public string SelectedCulture { get { return UserSettingProvider.Load(this) !=null? UserSettingProvider.Load(this).UICulture : UserSettingProvider.DefaultUserSetting.UICulture; } }

        private SessionData sessionData;
        public SessionData SessionData
        {
            get
            {
                if (sessionData == null)
                {
                    if (HttpRequest == null) return null;
                    sessionData = SessionData.GetSessionData(HttpRequest.RequestContext.HttpContext);
                }
                return sessionData;
            }
            set { sessionData = value; }
        }
    }
}