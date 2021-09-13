using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;


namespace MultivendorWebViewer.Common
{
    public static class URLUtility
    {
        public static HttpContextBase CreateHttpContext()
        {
            //return new RouteHttpContext(url);

            var context = new HttpContextWrapper(System.Web.HttpContext.Current);
            context.Request.RequestContext.RouteData = RouteTable.Routes.GetRouteData(context);
            return context;
        }
    }
}