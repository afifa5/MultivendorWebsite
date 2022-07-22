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
        routes.MapRoute(
            name: "Order",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Order", action = "Index", id = UrlParameter.Optional }
        );
     routes.MapRoute(
        name: "User",
        url: "{controller}/{action}/{id}",
        defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional }
     );
    routes.MapRoute(
        name: "Admin",
        url: "{controller}/{action}/{id}",
        defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
    );
    //routes.MapRoute(
    //name: "WebAdmin",
    //url: "{controller}/{action}/{id}",
    //defaults: new { controller = "WebAdmin", action = "Index", id = UrlParameter.Optional }
    //);
        }
    }
}
