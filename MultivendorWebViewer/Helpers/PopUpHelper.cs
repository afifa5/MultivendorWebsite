#if NET5
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
#endif
using MultivendorWebViewer.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
#if NET452
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
#endif

namespace MultivendorWebViewer.Helpers
{
    public class PopUp
    {
        public string Container { get; set; }

        public string RelativeElement { get; set; }

        public dynamic Left { get; set; }

        public dynamic Top { get; set; }

        public dynamic Right { get; set; }

        public dynamic Bottom { get; set; }

        public dynamic Width { get; set; }

        public dynamic Height { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public bool? AutoClose { get; set; }

        public bool? CloseOnClick { get; set; }

        public string Url { get; set; }

        public bool? Fixed { get; set; }

        public bool? Modal { get; set; }

        public string ClassNames { get; set; }

        public object HtmlAttributes { get; set; }

        protected string SanitazeSelector(string selector)
        {
            if (selector != null)
            {
                if (selector.IndexOfAny(PopUp.SanitazeSelectorChars) == -1)
                {
                    return string.Concat("#", selector);
                }
            }
            return selector;
        }

        protected static char[] SanitazeSelectorChars = new char[] { ' ', '#', '.', ':', '>', '(', ')', '*', ',', '~', '^', '+', '[', ']' };

        public virtual IDictionary<string, object> ToAttributes()
        {
            var dictionary = new RouteValueDictionary();

            if (Url != null) dictionary["url"] = Url;
            if (Container != null) dictionary["container"] = SanitazeSelector(Container);
            if (RelativeElement != null) dictionary["relativeElement"] = SanitazeSelector(RelativeElement);
            if (Fixed.HasValue) dictionary["fixed"] = Fixed.Value;
            if (Width != null) dictionary["width"] = Width;
            if (Height != null) dictionary["height"] = Height;
            if (Left != null) dictionary["left"] = Left;
            if (Top != null) dictionary["top"] = Top;
            if (Right != null) dictionary["right"] = Right;
            if (Bottom != null) dictionary["bottom"] = Bottom;
            if (HorizontalAlignment != HorizontalAlignment.None) dictionary["horizontalAlign"] = HorizontalAlignment == HorizontalAlignment.Left ? "left" : HorizontalAlignment == HorizontalAlignment.Right ? "right" : "center";
            if (VerticalAlignment != VerticalAlignment.None) dictionary["verticalAlign"] = VerticalAlignment == VerticalAlignment.Top ? "top" : VerticalAlignment == VerticalAlignment.Bottom ? "bottom" : "center";
            if (AutoClose.HasValue) dictionary["autoClose"] = AutoClose.Value;
            if (Modal.HasValue) dictionary["modal"] = Modal.Value;
            if (CloseOnClick.HasValue) dictionary["closeOnClick"] = CloseOnClick.Value;
            if (ClassNames != null) dictionary["classNames"] = ClassNames;

            return dictionary;
        }

        public virtual string ToAttributesString()
        {
            return Utility.JsonEncode(this.ToAttributes());
        }
    }

    public enum HorizontalAlignment { None, Left, Center, Right }

    public enum VerticalAlignment { None, Top, Center, Bottom }

    public class PopupActivation
    {
        public string ActivationMode { get; set; }

        public string Activator { get; set; }

        public PopupSelectorMode ActivatorMode { get; set; }

        public string Target { get; set; }

        public PopupSelectorMode TargetMode { get; set; }

        public string Scope { get; set; }
    
        public IDictionary<string,object> ToAttributes()
        {
            var values = new Dictionary<string, object>();

            if (ActivationMode != null) values["data-popup-activation-mode"] = ActivationMode;
            if (Activator != null) values["data-popup-activator"] = CreateSelector(Activator, ActivatorMode);
            if (Target != null) values["data-popup"] = CreateSelector(Target, TargetMode);

            return values;
        }

        protected string CreateSelector(string selector, PopupSelectorMode mode)
        {
            switch (mode)
            {
                case PopupSelectorMode.Document:
                    return selector;
                case PopupSelectorMode.This:
                    return string.Concat("this:", selector);
                case PopupSelectorMode.Parents:
                    return string.Concat("parents:", selector);
                case PopupSelectorMode.Closest:
                    return string.Concat("closest:", selector);
            }
            return selector;
        }
    }

    public enum PopupSelectorMode { Document, This, Parents, Closest };

    public static class PopupActivationModes
    {
        static PopupActivationModes()
        {
            Click = "click";
            DoubleClick = "dblclick";
            MouseUp = "mouseup";
            MouseDown = "mousedown";
            MouseEnter = "mouseenter";
            MouseOver = "mouseover";
            MouseMove = "mousemove";
            Hover = "hover";
            Hover = "keydown";
        }

        public static string Click { get; set; }

        public static string DoubleClick { get; set; }

        public static string MouseUp { get; set; }

        public static string MouseDown { get; set; }

        public static string MouseEnter { get; set; }

        public static string MouseOver { get; set; }

        public static string MouseMove { get; set; }

        public static string Hover { get; set; }

        public static string KeyDown { get; set; }
    }

    public static class PopUpHelper
    {
        public static MvcHtmlString PopUp(this HtmlHelper htmlHelper, HtmlContent content = null, PopUp options = null, PopupActivation activation = null, object htmlAttributes = null)
        {
            using (var htmlWriter = new StringWriter(CultureInfo.CurrentUICulture))
            {
                PopUpHelper.PopUp(htmlHelper, htmlWriter, content, options, activation, htmlAttributes);

                return MvcHtmlString.Create(htmlWriter.ToString());
            }
        }

        public static void PopUp(this HtmlHelper htmlHelper, TextWriter htmlWriter, HtmlContent content = null, PopUp options = null, PopupActivation activation = null, object htmlAttributes = null)
        {
            if (options == null)
            {
                options = new PopUp();
            }

            var popUpTag = new TagBuilder("div");

            var attributeBuilder = AttributeBuilder.TryCreate(htmlHelper, htmlAttributes, options.HtmlAttributes);

            popUpTag.MergeAttributes(attributeBuilder);

            popUpTag.AddCssClass("closed");

            if (options.ClassNames != null)
            {
                popUpTag.AddCssClass(options.ClassNames);

                options.ClassNames = null;
            }

            popUpTag.MergeAttribute("data-popup-options", Utility.JsonEncode(options.ToAttributes()));

            if (activation != null)
            {
                popUpTag.MergeAttributes(activation.ToAttributes());
            }

            popUpTag.AddCssClass("multivendor-popup");

            popUpTag.Write(htmlWriter, TagRenderMode.StartTag);

            if (content != null)
            {
                htmlWriter.Write("<div>");
                content.WriteContentHtml(htmlHelper, htmlWriter);
                htmlWriter.Write("</div>");
            }

            popUpTag.Write(htmlWriter, TagRenderMode.EndTag);
        }
    
        //public static MvcHtmlString Dialog(this HtmlHelper htmlHelper)
        //{
        //    htmlHelper.
        //}
    }
}