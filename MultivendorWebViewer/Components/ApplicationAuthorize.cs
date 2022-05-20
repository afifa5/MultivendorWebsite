using MultivendorWebViewer;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Server.Models;
using MultivendorWebViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MultivendorWebViewer.Common
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        public PermissionAuthorizeAttribute()
        {
            Permissions = new string[0];
        }

        public PermissionAuthorizeAttribute(params string[] permissions)
        {
            if (permissions != null)
            {
                Permissions = permissions;
            }
        }

        public bool AlwaysRequire { get; set; }

        public string[] Permissions { get; private set; }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            var requestContext = httpContext.GetApplicationRequestContext();

            if (requestContext == null) return false;
            var configuration = requestContext.Configuration;

            if (configuration == null) return false;

            var user = httpContext.User;

            if (user == null )
            {
                return false;
            }
            // Check permissions when declared for the controller/call
            if (user.Identity.IsAuthenticated == true)
            {
                var multivendorUser = requestContext.UserDBManager.FindUserByName(user.Identity.Name);
                if (Permissions.Any() == true && multivendorUser != null)
                {
                    var hasPermission = Permissions.Any(p => p == multivendorUser.UserRole);
                    return hasPermission;
                }
            }
          
          

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.IsChildAction == false && filterContext.HttpContext.Request.IsAjaxRequest() == false)
            {
                var requestContext = filterContext.GetApplicationRequestContext();

                var routeValues = new RouteValueDictionary();
                routeValues.Add("area", (string)null);

                if (filterContext.HttpContext.Session != null && filterContext.Controller.TempData.ContainsKey("loginReturnUrl") == false)
                {
                    filterContext.Controller.TempData.Add("loginReturnUrl", filterContext.HttpContext.Request.Url.AbsolutePath);
                }
                else if (filterContext.HttpContext.Request != null && routeValues.ContainsKey("returnUrl") == false)
                {
                    routeValues.Add("returnUrl", filterContext.HttpContext.Request.Url.ToString());
                }
                filterContext.Result = new RedirectResult(UrlUtility.Action(requestContext, filterContext.HttpContext.User.Identity.IsAuthenticated == false ? "Unauthenticated" : "Unauthorized", "User", routeValues: routeValues));

                return;
            }
            else
            {
                filterContext.Result = new EmptyResult();
            }

            filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
        }

    }
}
