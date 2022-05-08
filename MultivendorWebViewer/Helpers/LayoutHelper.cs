#if NET5
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif
using MultivendorWebViewer.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
#if NET452
using System.Web.Mvc;
using System.Web.WebPages;
#endif

namespace MultivendorWebViewer.Helpers
{
    public class HtmlContent
    {
        public HtmlContent() { }

#if NET5
        public HtmlContent(IHtmlContent content)
#else
        public HtmlContent(IHtmlString content)
#endif
        {
            Content = content;
            type = ContentType.IHtmlString;
        }

        public HtmlContent(string content)
        {
            Content = content;
            type = ContentType.String;
        }

        public HtmlContent(Action<HtmlHelper, TextWriter> content)
        {
            Content = content;
            type = ContentType.TextWriterAction;
        }

        public HtmlContent(Action<HtmlHelper, TextWriter, object> content)
        {
            Content = content;
            type = ContentType.ItemTextWriterAction;
        }

        public HtmlContent(Action<HtmlHelper, StringBuilder> content)
        {
            Content = content;
            type = ContentType.StringBuilderAction;
        }

#if NET5
        public HtmlContent(Func<HtmlHelper, IHtmlContent> content)
#else
        public HtmlContent(Func<HtmlHelper, IHtmlString> content)
#endif
        {
            Content = content;
            type = ContentType.IHtmlStringAction;
        }

        public HtmlContent(Func<object, HelperResult> content)
        {
            Content = content;
            type = ContentType.HelperResultFunc;
        }

#if NET5
        public HtmlContent(Func<object, IHtmlContent> content)
#else
        public HtmlContent(Func<object, IHtmlString> content)
#endif
        {
            Content = content;
            type = ContentType.IHtmlStringFunc;
        }

        public HtmlContent(HelperResult content)
        {
            Content = content;
            type = ContentType.HelperResult;
        }

        public HtmlContent(object content)
        {
            Content = content;
            type = ContentType.HelperResult;
        }

        public HtmlContent(string renderPartialViewName, object model, ViewDataDictionary viewData = null)
        {
#if NET5
            Action<HtmlHelper, TextWriter> content = (HtmlHelper htmlHelper, TextWriter w) => htmlHelper.RenderPartial(renderPartialViewName, model: model, viewData: viewData);
#else
            Action<HtmlHelper, TextWriter> content = (htmlHelper, writer) => htmlHelper.RenderPartial(renderPartialViewName, writer, viewData: viewData);
#endif
            Content = content;
            type = ContentType.TextWriterAction;
        }

//        public static HtmlContent RenderPartial(string partialViewName, object model, ViewDataDictionary viewData = null)
//        {
//#if NET5
//            return new HtmlContent((HtmlHelper htmlHelper, TextWriter w) => htmlHelper.RenderPartial(partialViewName, model: model, viewData: viewData));
//#else
//            return new HtmlContent((htmlHelper, writer) => htmlHelper.RenderPartial(partialViewName, writer, viewData: viewData));
//#endif
//        }

//        public static HtmlContent Partial(string partialViewName, object model, ViewDataDictionary viewData = null)
//        {
//#if NET5
//            return new HtmlContent(htmlHelper => MvcHtmlString.Create(htmlHelper.Partial(partialViewName, viewData: viewData, model: model)));
//#else
//            return new HtmlContent(htmlHelper => MvcHtmlString.Create(htmlHelper.Partial(partialViewName, viewData: viewData, model: model)));
//#endif
//        }

        //public static HtmlContent HelperResult(Func<object, HelperResult> content)
        //{
        //    var instance = new HtmlContent();
        //    instance.Content = content;
        //    instance.type = ContentType.HelperResultFunc;
        //    return instance;
        //}

        public static HtmlContent<T> HelperResult<T>(Func<T, HelperResult> content)
        {
            Func<object, HelperResult> objectContent = o => content((T)o);
            var instance = new HtmlContent<T>();
            instance.Content = objectContent;
            instance.type = ContentType.HelperResultFunc;
            return instance;
        }

        public static HtmlContent HelperResult(HelperResult content)
        {
            var instance = new HtmlContent();
            instance.Content = content;
            instance.type = ContentType.HelperResult;
            return instance;
        }

        public object Content { get; protected set; }

        private enum ContentType { Object, IHtmlString, IHtmlStringFunc, String, HelperResult, HelperResultFunc, StringBuilderAction, TextWriterAction, IHtmlStringAction, ItemTextWriterAction }

        private ContentType type;

        public virtual string GetContentHtml(HtmlHelper htmlHelper, object model = null)
        {
            if (Content != null)
            {
                try
                {
                    switch (type)
                    {
                        case ContentType.Object: return Content.ToString();
                        case ContentType.String: return (string)Content;
#if NET5
                        case ContentType.IHtmlString: return ((IHtmlContent)Content).ToString();
                        case ContentType.IHtmlStringAction: return ((Func<HtmlHelper, IHtmlContent>)Content)(htmlHelper).ToString();
                        case ContentType.IHtmlStringFunc: return ((Func<object, IHtmlContent>)Content)(model ?? htmlHelper.ViewData.Model).ToString();
#else
                        case ContentType.IHtmlString: return ((IHtmlString)Content).ToString();
                        case ContentType.IHtmlStringAction: return ((Func<HtmlHelper, IHtmlString>)Content)(htmlHelper).ToString();
                        case ContentType.IHtmlStringFunc: return ((Func<object, IHtmlString>)Content)(model ?? htmlHelper.ViewData.Model).ToString();
#endif
                        case ContentType.StringBuilderAction:
                            var htmlBuilder = new StringBuilder();
                            ((Action<HtmlHelper, StringBuilder>)Content)(htmlHelper, htmlBuilder);
                            return htmlBuilder.ToString();
                        case ContentType.TextWriterAction:
                            using (var htmlWriter = new StringWriter(CultureInfo.CurrentCulture))
                            {
                                ((Action<HtmlHelper, TextWriter>)Content)(htmlHelper, htmlWriter);
                                return htmlWriter.ToString();
                            }
                        case ContentType.ItemTextWriterAction:
                            using (var htmlWriter = new StringWriter(CultureInfo.CurrentCulture))
                            {
                                ((Action<HtmlHelper, TextWriter, object>)Content)(htmlHelper, htmlWriter, model ?? htmlHelper.ViewData.Model);
                                return htmlWriter.ToString();
                            }
                        case ContentType.HelperResult:
                            return ((HelperResult)Content).ToString();
                        case ContentType.HelperResultFunc:
                            return ((Func<object, HelperResult>)Content)(model ?? htmlHelper.ViewData.Model).ToString();
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine("Failed to get layout content", exception.ToString());
                }
            }
            return null;
        }

#if NET5
        public virtual IHtmlContent RenderContentHtml(HtmlHelper htmlHelper, object model = null)
#else
        public virtual IHtmlString RenderContentHtml(HtmlHelper htmlHelper, object model = null)
#endif
        {
            if (Content != null)
            {
                try
                {
                    switch (type)
                    {
                        case ContentType.Object: return MvcHtmlString.Create(Content.ToString());
                        case ContentType.String: return MvcHtmlString.Create((string)Content);
#if NET5
                        case ContentType.IHtmlString: return (IHtmlContent)Content;
                        case ContentType.IHtmlStringAction: return ((Func<HtmlHelper, IHtmlContent>)Content)(htmlHelper);
                        case ContentType.IHtmlStringFunc: return ((Func<object, IHtmlContent>)Content)(model ?? htmlHelper.ViewData.Model);
#else
                        case ContentType.IHtmlString: return (IHtmlString)Content;
                        case ContentType.IHtmlStringAction: return ((Func<HtmlHelper, IHtmlString>)Content)(htmlHelper);
                        case ContentType.IHtmlStringFunc: return ((Func<object, IHtmlString>)Content)(model ?? htmlHelper.ViewData.Model);
#endif
                        case ContentType.HelperResult: return (HelperResult)Content;
                        default:
                            return MvcHtmlString.Create(GetContentHtml(htmlHelper, model ?? htmlHelper.ViewData.Model));
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine("Failed to get layout content", exception.ToString());
                }
            }
            return null;
        }

        public virtual void WriteContentHtml(HtmlHelper htmlHelper, StringBuilder htmlBuilder, object model = null)
        {
            if (type == ContentType.StringBuilderAction)
            {
                try
                {
                    if (Content != null)
                    {
                        ((Action<HtmlHelper, StringBuilder>)Content)(htmlHelper, htmlBuilder);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Failed to get layout content " + ex.ToString());
                }
            }
            else
            {
                htmlBuilder.Append(GetContentHtml(htmlHelper, model));
            }  
        }

        public virtual void WriteContentHtml(HtmlHelper htmlHelper, TextWriter htmlWriter, object model = null)
        {
            if (Content != null)
            {
                try
                {
                    switch (type)
                    {
                        case ContentType.Object:
                            htmlWriter.Write(Content);
                            break;
                        case ContentType.TextWriterAction:
                            ((Action<HtmlHelper, TextWriter>)Content)(htmlHelper, htmlWriter);
                            break;
                        case ContentType.ItemTextWriterAction:
                            ((Action<HtmlHelper, TextWriter, object>)Content)(htmlHelper, htmlWriter, model ?? htmlHelper.ViewData.Model);
                            break;
                        case ContentType.HelperResult:
#if NET5
                            ((HelperResult)Content).WriteTo(htmlWriter, System.Text.Encodings.Web.HtmlEncoder.Default);
#else
                            ((HelperResult)Content).WriteTo(htmlWriter);
#endif
                            break;
                        case ContentType.HelperResultFunc:
#if NET5
                            ((Func<object, HelperResult>)Content)(model ?? htmlHelper.ViewData.Model).WriteTo(htmlWriter, System.Text.Encodings.Web.HtmlEncoder.Default);
#else
                            ((Func<object, HelperResult>)Content)(model ?? htmlHelper.ViewData.Model).WriteTo(htmlWriter);
#endif
                            break;
                        case ContentType.IHtmlStringFunc:
#if NET5
                            htmlWriter.Write(((Func<object, IHtmlContent>)Content)(model ?? htmlHelper.ViewData.Model).ToString());
#else
                            htmlWriter.Write(((Func<object, IHtmlString>)Content)(model ?? htmlHelper.ViewData.Model).ToString());
#endif
                            break;
                        case ContentType.String:
                            htmlWriter.Write((string)Content);
                            break;
                        case ContentType.IHtmlString:
#if NET5
                            htmlWriter.Write(((IHtmlContent)Content).ToString());
#else
                            htmlWriter.Write(((IHtmlString)Content).ToString());
#endif
                            break;
                        case ContentType.IHtmlStringAction:
#if NET5
                            htmlWriter.Write(((Func<HtmlHelper, IHtmlContent>)Content)(htmlHelper).ToString());
#else
                            htmlWriter.Write(((Func<HtmlHelper, IHtmlString>)Content)(htmlHelper).ToString());
#endif
                            break;
                        case ContentType.StringBuilderAction:
                            var stringWriter = htmlWriter as StringWriter;
                            if (stringWriter != null)
                            {
                                ((Action<HtmlHelper, StringBuilder>)Content)(htmlHelper, stringWriter.GetStringBuilder());
                            }
                            else
                            {
                                var htmlBuilder = new StringBuilder();
                                ((Action<HtmlHelper, StringBuilder>)Content)(htmlHelper, htmlBuilder);
                                htmlWriter.Write(htmlBuilder.ToString());
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Failed to get layout content " + ex.ToString());
                }
            }         
        }

        public static implicit operator HtmlContent(Func<object, HelperResult> content)
        {
            return new HtmlContent(content);
        }

        public static implicit operator HtmlContent(Action<HtmlHelper, StringBuilder> content)
        {
            return new HtmlContent(content);
        }

        public static implicit operator HtmlContent(string content)
        {
            return new HtmlContent(content);
        }

        public static implicit operator HtmlContent(HelperResult content)
        {
            return new HtmlContent(content);
        }

        public static implicit operator HtmlContent(MvcHtmlString content)
        {
            return new HtmlContent(content);
        }

        public static implicit operator HtmlContent(ValueType content)
        {
            return new HtmlContent(content.ToString());
        }
    }

    public class HtmlContent<T> : HtmlContent
    {
        public HtmlContent() { }

#if NET5
        public HtmlContent(Func<T, IHtmlContent> content)
#else
        public HtmlContent(Func<T, IHtmlString> content)
#endif
            : base((object m) => content((T)m))
        {
        }

        public HtmlContent(Action<HtmlHelper, TextWriter, T> content)
            : base((HtmlHelper h, TextWriter w, object i) => content(h, w, (T)i))
        {
        }
    }

    public class LayoutDefinition 
    {   
        public LayoutDefinition()
        {
            OmitEmpty = true;
        }

        public bool Ignore { get; set; }

        public string LayoutId { get; set; }

        public HtmlContent Content { get; set; }

        public Func<object, HelperResult> ContentTemplate
        {
            set { Content = new HtmlContent(value); }
        }

        public object HtmlAttributes { get; set; }

        public LayoutScrollMode ScrollMode { get; set; }

        public object Model { get; set; }

        protected virtual string Class { get { return null; } }

        public int? FillWeight { get; set; }

        public bool OmitEmpty { get; set; }

        public string ClassNames { get; set; }

        public LayoutCollapseMode CollapseMode { get; set; }

        public bool Collapsed { get; set; }

        public virtual bool RenderHtml(HtmlHelper htmlHelper, TextWriter htmlWriter)
        {
            string contentHtmlString = null;
            if (OmitEmpty == true)
            {
                if (Content == null)
                {
                    return false;
                }
                
                contentHtmlString = Content.GetContentHtml(htmlHelper, Model);
                
                if (string.IsNullOrEmpty(contentHtmlString) == true)
                {
                    return false;
                }
            }

            var tag = GetTag();

            if (FillWeight.HasValue == true)
            {
                tag.MergeAttribute("data-fill", Math.Min(10, Math.Max(0, FillWeight.Value)).ToString());
            }

            tag.MergeAttributes(HtmlAttributes);

            if (CollapseMode != LayoutCollapseMode.None)
            {
                tag.MergeAttribute("data-collapse", CollapseMode.ToString().ToLowerInvariant());
            }

            if (ScrollMode.HasFlag(LayoutScrollMode.ScrollX) == true)
            {
                tag.AddCssClass(ScrollXClass);
            }

            if (ScrollMode.HasFlag(LayoutScrollMode.ScrollY) == true)
            {
                tag.AddCssClass(ScrollYClass);
            }

            string className = Class;
            if (className != null)
            {
                tag.AddCssClass(className);
            }

            if (ClassNames != null)
            {
                tag.AddCssClass(ClassNames);
            }

            if (Collapsed == true)
            {
                tag.AddCssClass(CollapsedClass);
            }

            tag.Write(htmlWriter, TagRenderMode.StartTag);

            if (contentHtmlString == null)
            {
                if (Content != null)
                {
                    Content.WriteContentHtml(htmlHelper, htmlWriter, Model);
                }
            }
            else
            {
                htmlWriter.Write(contentHtmlString);
            }

            tag.Write(htmlWriter, TagRenderMode.EndTag);

            return true;
        }

        public string GetHtml(HtmlHelper htmlHelper, IFormatProvider formatProvider = null)
        {
            using (var htmlWriter = new StringWriter(formatProvider ?? CultureInfo.InvariantCulture))
            {
                if (RenderHtml(htmlHelper, htmlWriter) == false)
                {
                    return null;
                }
                return htmlWriter.ToString();
            }
        }

        protected virtual TagBuilder GetTag()
        {
            return new TagBuilder("section");
        }

        public static void AddFillWeight(TagBuilder tagBuilder, int fillWeight)
        {
            tagBuilder.MergeAttribute("data-fill", Math.Min(10, Math.Max(0, fillWeight)).ToString());
        }

        public const string ScrollXClass = "scroll-x";

        public const string ScrollYClass = "scroll-y";

        public const string CollapsedClass = "collapsed";
    }

    public class LayoutHeader : LayoutDefinition
    {
        protected override TagBuilder GetTag()
        {
            return new TagBuilder("header");
        }
    }

    public class LayoutFooter : LayoutDefinition
    {
        protected override TagBuilder GetTag()
        {
            return new TagBuilder("footer");
        }
    }

    public class LayoutBody : LayoutDefinition
    {
        public LayoutBody() 
        { 
            ScrollMode = LayoutScrollMode.ScrollY;
            FillWeight = 1;
        }
    }

    public class LayoutSideBar : LayoutDefinition
    {
        protected override TagBuilder GetTag()
        {
            return new TagBuilder("aside");
        }
    }

    public class LayoutNavigation : LayoutDefinition
    {
        protected override TagBuilder GetTag()
        {
            return new TagBuilder("nav");
        }
    }

    public class LayoutSplitter : LayoutDefinition
    {
        public string Icon { get; set; }

        protected override TagBuilder GetTag()
        {
            return new TagBuilder("div");
        }

        protected override string Class
        {
            get { return "splitter"; }
        }

        public override bool RenderHtml(HtmlHelper htmlHelper, TextWriter htmlWriter)
        {
            var tag = GetTag();

            tag.MergeAttributes(HtmlAttributes);

            string className = Class;
            if (className != null)
            {
                tag.AddCssClass(className);
            }

            if (ClassNames != null)
            {
                tag.AddCssClass(ClassNames);
            }

            tag.Write(htmlWriter, TagRenderMode.StartTag);

            //if (Icon != null)
            //{
            //    Icon.WriteHtml(htmlWriter);
            //}

            if (Content != null)
            {
                Content.WriteContentHtml(htmlHelper, htmlWriter, Model);
            }
            //else
            //{
            //    new FontAwesomeIconDescriptor("").WriteHtml(htmlWriter);
            //}

            tag.Write(htmlWriter, TagRenderMode.EndTag);

            return true;
        }
    }

    public class LayoutContent : LayoutDefinition
    {
        public override bool RenderHtml(HtmlHelper htmlHelper, TextWriter htmlWriter)
        {
            if (Content != null)
            {
                Content.WriteContentHtml(htmlHelper, htmlWriter, Model);
            }

            return true;
        }
    }

    public enum LayoutCollapseMode { None, Click, Programmatically }

    public class LayoutSettings
    {
        public LayoutDirection Direction { get; set; }

        public LayoutAlign Align { get; set; }

        static LayoutSettings()
        {
            LayoutSettings.Default = new LayoutSettings { Direction = LayoutDirection.Vertical, Align = LayoutAlign.All };
        }

        public static LayoutSettings Default { get; set; } 
    }

    [Flags]
    public enum LayoutScrollMode { None = 0x0, ScrollX = 0x1, ScrollY = 0x2, ScrollBoth = 0x3 }

    public enum LayoutDirection { Horizontal, Vertical };

    [Flags]
    public enum LayoutAlign { None = 0x0, Top = 0x1, Right = 0x2, Bottom = 0x4, Left = 0x8, Vertical = 0x5, Horizontal = 0xA, All = 0xF };

    public static class LayoutHelper
    {
        public static MvcHtmlString Layout(this HtmlHelper htmlHelper, IEnumerable<LayoutDefinition> definitions, LayoutSettings settings = null, object htmlAttributes = null)
        {
            var htmlBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(htmlBuilder, CultureInfo.InvariantCulture))
            {
                LayoutHelper.Layout(htmlHelper, stringWriter, definitions, settings, htmlAttributes);
            }
            return MvcHtmlString.Create(htmlBuilder.ToString());
        }

        public static void Layout(this HtmlHelper htmlHelper, StringBuilder htmlBuilder, IEnumerable<LayoutDefinition> definitions, LayoutSettings settings = null, object htmlAttributes = null)
        {
            using (var stringWriter = new StringWriter(htmlBuilder, CultureInfo.InvariantCulture))
            {
                LayoutHelper.Layout(htmlHelper, stringWriter, definitions, settings, htmlAttributes);
            }
        }

        public static void Layout(this HtmlHelper htmlHelper, TextWriter htmlWriter, IEnumerable<LayoutDefinition> definitions, LayoutSettings settings = null, object htmlAttributes = null)
        {
            if (settings == null)
            {
                settings = LayoutSettings.Default;
            }

            var layoutTag = new TagBuilder("div");

            layoutTag.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            //layoutTag.MergeAttribute("data-align", string.Join(" ", settings.Align.GetFlaggedValues().Select(v => v.ToString().ToLowerInvariant())));

            layoutTag.AddCssClass(settings.Direction == LayoutDirection.Horizontal ? "horizontal" : "vertical");
            
            layoutTag.AddCssClass("multivendor-web-layout");


            layoutTag.Write(htmlWriter, TagRenderMode.StartTag);

            var definitionsList = definitions.Where(d => d.Ignore == false).ToArray();
            bool previousRendered = true;
            for (int index = 0; index < definitionsList.Length; index++)
            {
                var definition = definitionsList[index];
                if (definition is LayoutSplitter)
                {
                    if (index > 0 && definitionsList[index - 1].CollapseMode != LayoutCollapseMode.None)
                    {
                        // Skip splitter if the collapsing pane has no content
                        if (previousRendered == false) continue;
                    }
                    else if (index < definitionsList.Length - 1 && definitionsList[index + 1].CollapseMode != LayoutCollapseMode.None)
                    {
                        index++;
                        var nextDefinition = definitionsList[index];

                        // Check if there is content in next layout definition
                        string content = nextDefinition.GetHtml(htmlHelper, htmlWriter.FormatProvider);
                        if (string.IsNullOrEmpty(content) == false)
                        {
                            // Write splitter
                            previousRendered = definition.RenderHtml(htmlHelper, htmlWriter);
                            // Wrtie content
                            htmlWriter.Write(content);
                        }

                        continue;
                    }
                }

                previousRendered = definition.RenderHtml(htmlHelper, htmlWriter);
                
            }

            layoutTag.Write(htmlWriter, TagRenderMode.EndTag);
        }
    }
}