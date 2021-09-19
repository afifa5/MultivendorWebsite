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
            SelectedCulture = "en-GB";
            CategoryManager = new CategoryManager();
            TextManager = new TextManager();
            Configuration = new ConfigurationManager();
            ImageManager = new ImageManager();
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
        public ImageManager ImageManager { get; set; }
        public  TextManager TextManager { get; set; }

        public ConfigurationManager Configuration { get; set; }
        public RequestContext RequestContext { get; set; }
        public string SelectedCulture { get; set; }

    }
}