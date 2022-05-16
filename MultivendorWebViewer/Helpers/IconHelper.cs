#if NET5
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
#if NET452
using System.Web.Mvc;
using System.Web.Routing;
#endif
using System.Xml.Serialization;

namespace MultivendorWebViewer.Helpers
{
    public enum IconSize { Default, S50, S110, S125, S133, S150, S200, S300, S400, S500 };

    public class MultiIconDescriptor : IconDescriptor
    {
        public MultiIconDescriptor(IEnumerable<IconDescriptor> icons)
        {
            Icons = icons.ToArray();
        }

        public ICollection<IconDescriptor> Icons { get; set; }

        public override void WriteHtml(TextWriter htmlWriter, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
        {
            foreach (var icon in Icons)
            {
                icon.WriteHtml(htmlWriter, htmlAttributes, classNames, size);
            }
        }
    }

    [XmlInclude(typeof(IcomoonIconDescriptor))]
    [XmlInclude(typeof(GlyphIconDescriptor))]
    [XmlInclude(typeof(FontAwesomeIconDescriptor))]
    public class IconDescriptor
    {
        public IconDescriptor() { }

        public IconDescriptor(string id, string name)
        {
            Id = id;
            Name = name;
        }
    
        public IconDescriptor(params IconDescriptor[] prototype)
        {
            Merge(prototype);
        }

        [XmlAttribute("id")]
        public string Id { get; set; }

        //public string Context { get; private set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("InnerText")]
        public string InnerText { get; set; }

        private string css;
        [XmlIgnore]
        public string Css { get { return css ?? (css = GetCss()); } }

        [XmlAttribute("css")]
        public string CustomCss { get; set; }

        public string GetHtml(object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
        {
            var stringBuilder = new StringBuilder();
            WriteHtml(stringBuilder, htmlAttributes, classNames, size);
            return stringBuilder.ToString();
        }

        public void WriteHtml(StringBuilder builder, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
        {
            using (var htmlWriter = new StringWriter(builder))
            {
                WriteHtml(htmlWriter, htmlAttributes, classNames, size);
            }
        }

        public virtual void WriteHtml(TextWriter htmlWriter, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
        {
            if (htmlAttributes == null)
            {
                htmlWriter.Write("<span class=\"");
                htmlWriter.Write(Css);
                if (CustomCss != null)
                {
                    htmlWriter.Write(" ");
                    htmlWriter.Write(CustomCss);
                }
                if (classNames != null)
                {
                    htmlWriter.Write(" ");
                    htmlWriter.Write(classNames);
                }
                htmlWriter.Write(GetSizeClass(size));
                htmlWriter.Write("\">"+(string.IsNullOrEmpty( InnerText) ? string.Empty: InnerText .Trim())+ "</span>");

            }
            else
            {
                var iconTag = new TagBuilder("span");
                var attributeBuilder = htmlAttributes as AttributeBuilder;
                if (attributeBuilder != null)
                {
                    iconTag.MergeAttributes(attributeBuilder);
                }
                else
                {
                    iconTag.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                }
                iconTag.AddCssClass(Css);
                if (CustomCss != null) iconTag.AddCssClass(CustomCss);
                iconTag.AddCssClass(GetSizeClass(size));
                if (classNames != null)
                {
                    iconTag.AddCssClass(classNames);
                }
#if NET5
                iconTag.Write(htmlWriter, TagRenderMode.StartTag);
                if (InnerText != null)
                {
                    string trimmedInnerText = InnerText.Trim();
                    if (InnerText.Length > 0)
                    {
                        htmlWriter.Write(InnerText);
                    }
                }
#else
                iconTag.SetInnerText((string.IsNullOrEmpty(InnerText) ? string.Empty : InnerText.Trim()));
                iconTag.Write(htmlWriter);
#endif
            }
        }

        public void Merge(params IconDescriptor[] others)
        {
            others.ForEach(o => Merge(o));
        }

        public virtual void Merge(IconDescriptor other)
        {
            if (Name == null) Name = other.Name;
            if (CustomCss == null) CustomCss = other.CustomCss;
        }

        protected virtual string GetCss()
        {
            return string.Empty;
        }

        protected virtual string GetSizeClass(IconSize size)
        {
            return string.Empty;
        }
    }

    //public class ContentFileDescriptor : IconDescriptor
    //{
    //    public int? Height { get; set; }
    //    public int? Width { get; set; }

    //    public ContentFileDescriptor(string id, string name)
    //        : base(id, name)
    //    {
    //    }

    //    public ContentFileDescriptor(string id, string name, int width, int height)
    //        : base(id, name)
    //    {
    //        Height = height;
    //        Width = width;
    //    }

    //    public override void WriteHtml(TextWriter htmlWriter, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
    //    {
    //        var routeValues = new RouteValueDictionary();
    //        routeValues.Add("id", 0);
    //        if (Width.HasValue) routeValues.Add("width", Width.Value);
    //        if (Height.HasValue) routeValues.Add("height", Height.Value);
    //        routeValues.Add("fileName", String.Format("~/Content/Images/{0}", Name));

    //        string url = UrlUtility.Action(ApplicationRequestContext, "Image", "Content", routeValues);

    //        var iconTag = new TagBuilder("img");
    //        iconTag.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    //        iconTag.AddCssClass(Css);
    //        if (Width.HasValue) iconTag.MergeAttribute("width", Width.Value.ToString());
    //        if (Height.HasValue) iconTag.MergeAttribute("height", Height.Value.ToString());
    //        iconTag.MergeAttribute("src", url);
    //        iconTag.Write(htmlWriter, TagRenderMode.SelfClosing);
    //    }
    //}
    public static class IconRendererProvider
    {
        private static IDictionary<Type, IconRenderer> renderers = new Dictionary<Type, IconRenderer>();

        public static IconRenderer GetRenderer(IconDescriptor iconDescriptor)
        {
            IconRenderer renderer;
            Type iconDescriptorType = iconDescriptor.GetType();
            if (renderers.TryGetValue(iconDescriptorType, out renderer) == true) return renderer;

            var rendererTypeName = iconDescriptorType.Name.Replace("Descriptor", "Renderer");
            var rendererType = Type.GetType("MultivendorWebViewer.Helpers." + rendererTypeName);
            if (rendererType != null)
            {
                renderer = Instance.Create<IconRenderer>(rendererType);
            }
            else
            {
                renderer = new IconRenderer();
            }

            var list = renderers.ToList();
            list.Add(new KeyValuePair<Type, IconRenderer>(iconDescriptorType, renderer));
            renderers = list.ToDictionaryOptimized(i => i.Key, i => i.Value);
            return renderer;
        }
    }
    public interface IIconRenderer
    {
        string GetHtml(IconDescriptor icon, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default);
        void WriteHtml(IconDescriptor icon, StringBuilder builder, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default);
        void WriteHtml(IconDescriptor icon, TextWriter htmlWriter, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default);
    }

    [XmlInclude(typeof(FontAwesomeIconDescriptor))]
    public class IconRenderer
    {
        static IconRenderer()
        {

        }

        [XmlElement("StaticWidth")]
        public bool? StaticWidth { get; set; }

        [XmlElement("FontAwsome5")]
        public bool? FontAwsome5 { get; set; }
        private static class RendererMap<T>
        {
            public static IconRenderer renderer;
        }

        public static IconRenderer GetRenderer<T>(T descriptor)
            where T : IconDescriptor
        {
            return RendererMap<T>.renderer;
        }

        protected virtual string GetCss(IconDescriptor icon)
        {
            var iconItem = icon as FontAwesomeIconDescriptor;
            if (FontAwsome5 == true)
            {
                return StaticWidth != true ? string.Concat("icon far fa-", iconItem.Name) : string.Concat("icon far fa-fw fa-", iconItem.Name);
            }
            return StaticWidth != true ? string.Concat("icon fa fa-", iconItem.Name) : string.Concat("icon fa fa-fw fa-", iconItem.Name);

        }

        protected virtual string GetSizeClass(IconSize size)
        {
            return string.Empty;
        }

        public string GetHtml(IconDescriptor icon, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
        {
            var stringBuilder = new StringBuilder();
            WriteHtml(icon, stringBuilder, htmlAttributes, classNames, size);
            return stringBuilder.ToString();
        }

        public void WriteHtml(IconDescriptor icon, StringBuilder builder, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
        {
            using (var htmlWriter = new StringWriter(builder))
            {
                WriteHtml(icon, htmlWriter, htmlAttributes, classNames, size);
            }
        }

        public virtual void WriteHtml(IconDescriptor icon, TextWriter htmlWriter, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
        {
            var iconItem = icon as FontAwesomeIconDescriptor;
            if (htmlAttributes == null)
            {
                
                htmlWriter.Write("<span class=\"");
                htmlWriter.Write(GetCss(iconItem));
                if (icon.CustomCss != null)
                {
                    htmlWriter.Write(" ");
                    htmlWriter.Write(iconItem.CustomCss);
                }
                if (classNames != null)
                {
                    htmlWriter.Write(" ");
                    htmlWriter.Write(classNames);
                }
                htmlWriter.Write(GetSizeClass(size));
                htmlWriter.Write("\">" + (string.IsNullOrEmpty(iconItem.InnerText) ? string.Empty : icon.InnerText.Trim()) + "</span>");

            }
            else
            {
                var iconTag = new TagBuilder("span");
                var attributeBuilder = htmlAttributes as AttributeBuilder;
                if (attributeBuilder != null)
                {
                    iconTag.MergeAttributes(attributeBuilder);
                }
                else
                {
                    iconTag.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                }
                iconTag.AddCssClass(GetCss(iconItem));
                if (icon.CustomCss != null) iconTag.AddCssClass(iconItem.CustomCss);
                iconTag.AddCssClass(GetSizeClass(size));
                if (classNames != null)
                {
                    iconTag.AddCssClass(classNames);
                }

                iconTag.SetInnerText((string.IsNullOrEmpty(iconItem.InnerText) ? string.Empty : icon.InnerText.Trim()));
                iconTag.Write(htmlWriter);

            }
        }

    }

    public class GlyphIconDescriptor : IconDescriptor
    {
        public GlyphIconDescriptor() { }

        public GlyphIconDescriptor(string id, string name) : base(id, name) { }

        protected override string GetCss()
        {
            return string.Concat("icon glyphicon glyphicon-", Name);
        }

        protected override string GetSizeClass(IconSize size)
        {
            switch (size)
            {//Default, S50, S110, S125, S133, S150, S200, S300, S400, S500
                case IconSize.S50: return " glyphicon-05x";
                case IconSize.S110: return " glyphicon-110x";
                case IconSize.S125: return " glyphicon-125x";
                case IconSize.S133: return " glyphicon-lg";
                case IconSize.S150: return " glyphicon-150x";
                case IconSize.S200: return " glyphicon-2x";
                case IconSize.S300: return " glyphicon-3x";
                case IconSize.S400: return " glyphicon-4x";
                case IconSize.S500: return " glyphicon-5x";
                default: return string.Empty;
            }
        }
    }
	
	public class MaterialIconDescriptor : IconDescriptor
    {
        public MaterialIconDescriptor() { }

        public MaterialIconDescriptor(string id, string name) : base(id, name) {
            //InnerText = Name;
        }
        protected override string GetCss()
        {
            return "icon material-icons";
        }
        public override void WriteHtml(TextWriter htmlWriter, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
        {
            var materialAttribute = htmlAttributes == null ? new Dictionary<string, object>() : (htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            materialAttribute.Add("icon-name", Name);
            base.WriteHtml(htmlWriter, materialAttribute, classNames, size);
        }
        protected override string GetSizeClass(IconSize size)
        {
            switch (size)
            {//Default, S50, S110, S125, S133, S150, S200, S300, S400, S500
                case IconSize.S50: return " material-05x";
                case IconSize.S110: return " material-110x";
                case IconSize.S125: return " material-125x";
                case IconSize.S133: return " material-lg";
                case IconSize.S150: return " material-150x";
                case IconSize.S200: return " material-2x";
                case IconSize.S300: return " material-3x";
                case IconSize.S400: return " material-4x";
                case IconSize.S500: return " material-5x";
                default: return string.Empty;
            }
        }

    }

    public class IcomoonIconDescriptor : IconDescriptor
    {
        public IcomoonIconDescriptor() { }

        public IcomoonIconDescriptor(string id, string name) : base(id, name) { }

        protected override string GetCss()
        {
            return string.Concat("icon icomoon ", Name);
        }

        protected override string GetSizeClass(IconSize size)
        {
            switch (size)
            {//Default, S50, S110, S125, S133, S150, S200, S300, S400, S500
                case IconSize.S50: return " icomoon-05x";
                case IconSize.S110: return " icomoon-110x";
                case IconSize.S125: return " icomoon-125x";
                case IconSize.S133: return " icomoon-lg";
                case IconSize.S150: return " icomoon-150x";
                case IconSize.S200: return " icomoon-2x";
                case IconSize.S300: return " icomoon-3x";
                case IconSize.S400: return " icomoon-4x";
                case IconSize.S500: return " icomoon-5x";
                default: return string.Empty;
            }
        }

    }


    public class FontAwesomeIconDescriptor : IconDescriptor
    {
        public FontAwesomeIconDescriptor() { }

        public FontAwesomeIconDescriptor(string id, string name, bool staticWidth = true, bool fontAwsome5 = false)
            : base(id, name)
        {
            //StaticWidth = staticWidth;
            FontAwsome5 = fontAwsome5;
        }

        [XmlElement("StaticWidth")]
        public bool? StaticWidth { get; set; }

        [XmlElement("FontAwsome5")]
        public bool? FontAwsome5 { get; set; }

        protected override string GetCss()
        {
            if(FontAwsome5 == true)
            {
                return StaticWidth != true ? string.Concat("icon far fa-", Name) : string.Concat("icon far fa-fw fa-", Name);
            }
            return StaticWidth != true ? string.Concat("icon fa fa-", Name) : string.Concat("icon fa fa-fw fa-", Name);
        }

        //public override void Merge(IconDescriptor other)
        //{
        //    base.Merge(other);

        //    var faOther = other as FontAwesomeIconDescriptor;
        //    if (faOther != null)
        //    {
        //        if (faOther.StaticWidth.HasValue == true) StaticWidth = faOther.StaticWidth;
        //    }
        //} 

        protected override string GetSizeClass(IconSize size)
        {
            switch (size)
            {
                case IconSize.S50: return " fa-05x";
                case IconSize.S110: return " fa-110x";
                case IconSize.S125: return " fa-125x";
                case IconSize.S133: return " fa-lg";
                case IconSize.S150: return " fa-150x";
                case IconSize.S200: return " fa-2x";
                case IconSize.S300: return " fa-3x";
                case IconSize.S400: return " fa-4x";
                case IconSize.S500: return " fa-5x";
                default: return string.Empty;
            }
        }

    }

    //public class StackedFontAwesomeIconDescriptor : IconDescriptor
    //{
    //    public StackedFontAwesomeIconDescriptor(params FontAwesomeIconDescriptor [] icons)
    //    {
    //        Icons = icons;
    //    }

    //    public StackedFontAwesomeIconDescriptor(IEnumerable<FontAwesomeIconDescriptor> icons)
    //    {
    //        Icons = icons.ToArray();
    //    }

    //    public FontAwesomeIconDescriptor[] Icons { get; private set; }

    //    public override void WriteHtml(StringBuilder builder, object htmlAttributes)
    //    {
    //        var stackTag = new TagBuilder("span");
    //        if (htmlAttributes != null)
    //        {
    //            stackTag.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    //        }
    //        stackTag.AddCssClass("fa-stack");

    //        stackTag.Write(builder, TagRenderMode.StartTag);

    //        foreach(FontAwesomeIconDescriptor icon in Icons)
    //        {
    //            icon.WriteHtml(builder, null);
    //        }

    //        stackTag.Write(builder, TagRenderMode.EndTag);
    //    }
    //}

    public static class IconHelper
    {
        //public static MvcHtmlString Icon(this HtmlHelper helper, IconDescriptor icon, object htmlAttributes = null)
        //{
        //    return MvcHtmlString.Create(IconHelper.IconInternal(icon, htmlAttributes));
        //}

        public static MvcHtmlString Icon(this HtmlHelper helper, string iconId, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default, string context = null)
        {
            var applicationRequestContext = helper.ViewContext.GetApplicationRequestContext();
            var icon = applicationRequestContext.GetIcon(iconId, context);
            return MvcHtmlString.Create(IconHelper.IconInternal(icon, htmlAttributes, classNames, size));
        }

        public static MvcHtmlString Icon(this HtmlHelper helper, IconDescriptor icon, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
        {
            return MvcHtmlString.Create(IconHelper.IconInternal(icon, htmlAttributes, classNames, size));
        }

        public static string IconInternal(IconDescriptor icon, object htmlAttributes = null, string classNames = null, IconSize size = IconSize.Default)
        {
            return icon.GetHtml(htmlAttributes, classNames, size);
        }

        public static void IconInternal(IconDescriptor icon, StringBuilder stringBuilder, object htmlAttributes = null, string @class = null, IconSize size = IconSize.Default)
        {
            icon.WriteHtml(stringBuilder, htmlAttributes, @class, size);
        }

        public static void IconInternal(IconDescriptor icon, TextWriter writer, object htmlAttributes = null, string @class = null, IconSize size = IconSize.Default)
        {
            icon.WriteHtml(writer, htmlAttributes, @class, size);
        }

        public static void RenderIcon(this HtmlHelper helper, IconDescriptor icon, object htmlAttributes = null, string @class = null, IconSize size = IconSize.Default)
        {
            IconInternal(icon, helper.ViewContext.Writer, htmlAttributes, @class, size);
        }
    }
}