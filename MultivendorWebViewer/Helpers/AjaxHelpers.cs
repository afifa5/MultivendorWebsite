using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;


namespace MultivendorWebViewer.Helpers
{
    public static class AjaxHelpers
    {
        private const string LinkOnClickFormat = "Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), {0});";

        public static MvcHtmlString ActionContentLink(this AjaxHelper ajaxHelper, string linkInnerHtml, string actionName, string controllerName = null, object routeValues = null, AjaxOptions ajaxOptions = null, object htmlAttributes = null)
        {
            RouteValueDictionary newValues = new RouteValueDictionary(routeValues);
            RouteValueDictionary newAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return ActionContentLink(ajaxHelper, linkInnerHtml, actionName, controllerName, newValues, ajaxOptions, newAttributes);
        }

        public static MvcHtmlString ActionContentLink(this AjaxHelper ajaxHelper, string linkInnerHtml, string actionName, string controllerName = null, RouteValueDictionary routeValues = null, AjaxOptions ajaxOptions = null, IDictionary<string, object> htmlAttributes = null)
        {
            //if (string.IsNullOrEmpty(linkInnerHtml))
            //{
            //    throw new ArgumentException("linkInnerHtml");
            //}

            string targetUrl = UrlHelper.GenerateUrl(null, actionName, controllerName, routeValues, ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true /* includeImplicitMvcValues */);

            return MvcHtmlString.Create(GenerateLink(ajaxHelper, linkInnerHtml, targetUrl, ajaxOptions ?? new AjaxOptions(), htmlAttributes));
        }

        private static string GenerateLink(AjaxHelper ajaxHelper, string linkInnerHtml, string targetUrl, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            var tag = new TagBuilder("a")
            {
                InnerHtml = linkInnerHtml //HttpUtility.HtmlEncode();
            };

            tag.MergeAttributes(htmlAttributes);
            tag.MergeAttribute("href", targetUrl);

            if (ajaxHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
            {
                tag.MergeAttributes(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            }
            else
            {
                throw new ApplicationException("Javascript exception");
            }

            return tag.ToString(TagRenderMode.Normal);
        }

        public static MvcHtmlString AjaxAction(this AjaxHelper ajaxHelper, string actionName, string controllerName = null, object routeValues = null)
        {
            var loaderTag = new TagBuilder("div");

            var urlHelper = new UrlHelper(ajaxHelper.ViewContext.RequestContext);

            var url = urlHelper.Action(actionName, controllerName, routeValues);

            loaderTag.MergeAttribute("data-url", url);
            loaderTag.AddCssClass("multivendor-async-view");

            return MvcHtmlString.Create(loaderTag.ToString());
        }

    }
}