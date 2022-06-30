using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MultivendorWebViewer.Common;


namespace MultivendorWebViewer.Membership
{
    public class CustomAuthenticationAttribute : ActionFilterAttribute
    {
        public bool Disable { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //bool result = Configuration.AuthenticationNeeded;
            //if (Disable == false && Configuration.AuthenticationNeeded == true)
            //{
            //    var u = filterContext.HttpContext.User as PrincipalManager;
            //    string sitename=filterContext.RequestContext.RouteData.Values["site"] as string;
            //    if (u == null)
            //    {
            //        if (filterContext.HttpContext.Request.IsAjaxRequest() == false)
            //        {
            //            var values = new RouteValueDictionary();
            //            values["culture"] = filterContext.RouteData.Values["culture"];
            //            values["site"] = filterContext.RouteData.Values["site"];
            //            values["controller"] = "Account";
            //            values["action"] = "Login";

            //            filterContext.HttpContext.Session["loginReturnUrl"] = filterContext.HttpContext.Request.RawUrl;
            //            //SessionItemHelper.SetItem("loginReturnUrl", filterContext.HttpContext.Request.RawUrl);

            //            filterContext.Result = new RedirectToRouteResult(values);
            //        }
            //        else
            //        {
            //            filterContext.Result = new EmptyResult();
            //        }
            //    }
            //}

            base.OnActionExecuting(filterContext);
        }
    }
}