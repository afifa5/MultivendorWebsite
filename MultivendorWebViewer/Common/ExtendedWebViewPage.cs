using MultivendorWebViewer.Components;
using MultivendorWebViewer.Controllers;
using MultivendorWebViewer.Helpers;
using MultivendorWebViewer.Membership;
using MultivendorWebViewer.Server.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace MultivendorWebViewer.Common
{
    public abstract class ExtendedWebViewPage : WebViewPage
    {
        //public void RegisterGlobalSection(string name)
        //{
        //    GlobaleSectionManager.Register(name);
        //}

        //public HelperResult RenderGlobalSection(string name, bool required = true)
        //{
        //    return GlobaleSectionManager.Render(name, required);
        //}

        public virtual PrincipalManager CurrentUser
        {
            get { return base.User as PrincipalManager; }
        }
    }



    public abstract class ExtendedWebViewPage<TModel> : WebViewPage<TModel>
    {
        private ApplicationRequestContext applicationRequestContext;
        public ApplicationRequestContext ApplicationRequestContext
        {
            get
            {
                if (applicationRequestContext == null)
                {
                    var controller = ViewContext.Controller as BaseController;
                    applicationRequestContext = controller != null ? controller.ApplicationRequestContext : ApplicationRequestContext.GetContext(Context);
                }
                return applicationRequestContext;
            }
        }

        

        private State currentState;
        public State CurrentState
        {
            get
            {
                if (currentState != null && currentState.IsEmpty) currentState = null;
                return currentState ?? (currentState = ApplicationRequestContext.State ?? State.Empty);
            }
        }

        public User CurrentUser
        {
            get { return ApplicationRequestContext.User; }
        }

        private SessionData sessionData;
        public SessionData SessionData
        {
            get { return sessionData ?? (sessionData = ApplicationRequestContext.SessionData); }
        }

        public void RegisterGlobalSection(string name)
        {
            GlobaleSectionManager.Register(this, name);
        }

        public HelperResult RenderGlobalSection(string name, bool required = true)
        {
            return GlobaleSectionManager.Render(Context, name, required);
        }

        public void DefineSection(string name, Func<MvcHtmlString> sectionRenderer)
        {       
            DefineSection(name, () =>
            {
                var result = sectionRenderer();
                Output.Write(result.ToHtmlString());
            });
        }

        public void DefineSection(string name, Func<HelperResult> sectionRenderer)
        {
            DefineSection(name, () =>
            {
                var result = sectionRenderer();
                Output.Write(result);
            });
        }

    }

    public static class GlobaleSectionManager
    {
        private static Guid layoutSectionKey = Guid.NewGuid();

        private static IDictionary<string, string> GetLayoutSections(HttpContextBase context)
        {
            return RequestItemHelper.GetItem<IDictionary<string, string>>(context, GlobaleSectionManager.layoutSectionKey, () => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
        }

        public static void Register(string name)
        {
            Register(WebPageContext.Current.Page as WebPageBase, name);
        }

        public static void Register(WebPageBase page, string name)
        {
            if (page != null)
            {
                var section = page.RenderSection(name, false);

                if (section != null)
                {
                    GetLayoutSections(page.Context).Add(name, section.ToString());
                }
            }
        }

        public static HelperResult Render(HttpContextBase context, string name, bool required = true)
        {
            var sectionHtml = GetLayoutSections(context).TryGetValue(name);

            if (sectionHtml != null)
            {
                return new HelperResult(tw => tw.Write(sectionHtml));
            }
            else if (required == true)
            {
                throw new HttpException(String.Format(CultureInfo.InvariantCulture, string.Format("Global section {0} not registered", name)));
            }

            return null;
        }
    }
}