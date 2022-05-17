using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using MultivendorWebViewer.Components;
using MultivendorWebViewer.Helpers;
using MultivendorWebViewer.Manager;
using MultivendorWebViewer.Server.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;


namespace MultivendorWebViewer.Common
{
    [NotMapped]
    public class ApplicationRequestContext
    {

        public ApplicationRequestContext(RequestContext requestContext)
        {
            RequestContext = requestContext;
            CategoryManager = CategoryManager.Default;
            TextManager =TextManager.Default;
            Configuration = ConfigurationManager.Default;
            ImageManager =ImageManager.Default;
            ProductManager = ProductManager.Default;
            OrderManager =OrderManager.Default;
            UserDBManager = UserDBManager.Default;

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
        public UserDBManager UserDBManager { get; set; }
        public OrderManager OrderManager { get; set; }
        public ProductManager ProductManager { get; set; }
        public ImageManager ImageManager { get; set; }
        public  TextManager TextManager { get; set; }
        public CookieUserSettingProvider UserSettingProvider { get { return new CookieUserSettingProvider(); } }
        public ConfigurationManager Configuration { get; set; }

        public System.Web.Routing.RequestContext RequestContext { get; set; }
        public HttpContextBase HttpContext { get { return RequestContext != null ? RequestContext.HttpContext : null; } }

        public HttpRequestBase HttpRequest { get { return RequestContext != null && RequestContext.HttpContext != null ? RequestContext.HttpContext.Request : null; } }

        private ApplicationUserManager userManager;
        public ApplicationUserManager UserManager
        {
            get { return userManager!= null? userManager : HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { userManager = value; }
        }
        public void ClearUser()
        {
            RemoveObject("User");
        }

        public User User
        {
            get
            {
                return GetObject<User>("User", () =>
                {
                    User multivendorUser = null;

                    if (HttpContext != null && HttpContext.User != null && HttpContext.User.Identity != null && HttpContext.User.Identity.AuthenticationType == "Federation")
                    {
                        multivendorUser = HttpContext.User != null ? MultivendorUserCreator.Create(HttpContext.User.Identity) : null;
                    }

                    if (multivendorUser == null)
                    {
                        string id = HttpContext.User.Identity.GetUserId();
                        if (String.IsNullOrEmpty(id) == false)
                        {
                           ApplicationUser applicationUser = UserManager.FindByIdAsync(id).Result;
                            if (applicationUser != null) multivendorUser = applicationUser.MultivendorUser;
                        }
                    }

                    if (multivendorUser == null)
                    {
                        string name = HttpContext.User.Identity.GetUserName();
                        if (String.IsNullOrEmpty(name) == false)
                        {
                           ApplicationUser applicationUser = UserManager.FindByNameAsync(name).Result;
                            if (applicationUser != null) multivendorUser = applicationUser.MultivendorUser;
                        }
                    }

                    //if (multivendorUser == null && SessionData != null && SessionData.User != null)
                    //{
                    //    // Used i.e. in Offline deploy scenario
                    //    multivendorUser = SessionData.User;
                    //}
                    return multivendorUser;
                });
            }
        }

        private State state;
        public State State
        {
            get
            {
                // The create state procedure contains autoset (eg site if wrong) logic. Must be moved.
                // TODO Should this create the State? Not as tied to the filter as now
                if (state == null)
                {
                    //state = StateRouteProvider.Default.GetState(this);
                }
                return state;
            }
            set { state = value; }
        }
        public HashSet<string> ViewContext { get; private set; }
        private Dictionary<string, object> customContext;
        protected Dictionary<string, object> CustomContext
        {
            get { return customContext ?? (customContext = new Dictionary<string, object>()); }
        }
        public bool HasCustomContext(string id)
        {
            return CustomContext.ContainsKey(id);
        }
        public IconDescriptor GetIcon(string name, string context = null)
        {
            return IconsManager.Default.GetIcon(name, context);
        }
        //public HttpRequestBase HttpRequest { get { return RequestContext != null && RequestContext.HttpContext != null ? RequestContext.HttpContext.Request : null; } }
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

        public string GetApplicationTextTranslation(string text)
        {
            return TextManager.Current.GetText(text);
        }
        public T GetObject<T>(object key, Func<T> objectFactory = null)
        {
            return RequestItemHelper.GetItem<T>(HttpContext, key, objectFactory);
        }
        public void RemoveObject(object key)
        {
            RequestItemHelper.RemoveItem(HttpContext, key);
        }
    }
}