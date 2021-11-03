using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MultivendorWebViewer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Category",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Category", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
            name: "Content",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Content", action = "Index", id = UrlParameter.Optional }
        );
            routes.MapRoute(
            name: "Security",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Security", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
