using MultivendorWebViewer.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
#if NET452
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
#endif
using System.Xml.Serialization;
#if NET5
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif

namespace MultivendorWebViewer.Helpers
{
    public class ToolBarSettings
    {
        public ToolBarSettings()
        {
            Direction = LayoutDirection.Horizontal;
            ContentDirection = LayoutDirection.Vertical;
            ShowToolTip = true;
            Size = ToolBarSize.Medium;
        }

        public bool? DisplayLabels { get; set; }

        public bool? DisplayContent { get; set; }

        public LayoutDirection Direction { get; set; }

        public LayoutDirection ContentDirection { get; set; }

        public bool Compact { get; set; }

        public ToolBarSize Size { get; set; }

        public bool Highlight { get; set; }

        public bool ShowToolTip { get; set; }

        public bool NewLayout { get; set; }

        static ToolBarSettings()
        {
            ToolBarSettings.Default = new ToolBarSettings();
        }

        public static ToolBarSettings Default { get; set; }
    }

    public enum ToolBarSize { NotSet, Small, Medium, Large }

    [Flags]
    public enum CheckType { None = 0x0, Normal = 0x1, Toggle = 0x2, Radio = 0x4 }

    public class ToolBarItem
    {
        public ToolBarItem()
        {
            TextAlignment = ToolBarAlignment.Right;
            Clickable = true;
        }

        public ToolBarItem(string url) : this()
        {
            Url = url;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public virtual string Label { get; set; }

        private string toolTip;

        public string ToolTip
        {
            get { return string.IsNullOrEmpty(toolTip) ? Label : toolTip; }
            set { toolTip = value; }
        }

        public bool? ShowTooltip { get; set; }

        public string ImageUrl { get; set; }

        public IconDescriptor Icon { get; set; }

        public IconSize Size { get; set; }

        public ToolBarAlignment Alignment { get; set; }

        public ToolBarAlignment TextAlignment { get; set; }

        public LayoutDirection? ContentDirection { get; set; }

        public string ClassNames { get; set; }

        public object HtmlAttributes { get; set; }

        public object AnchorTagHtmlAttributes { get; set; }

        #region Check

        public CheckType CheckType { get; set; }

        public bool? Checked { get; set; }

        public string CheckGroup { get; set; }

        #endregion

        #region Drop Down / Menu

        public DropDown DropDown { get; set; }

        public Func<object, HelperResult> DropDownTemplate { get; set; }

        public IEnumerable<MenuItem> DropDownItems { get; set; }

        public MenuSettings MenuSettings { get; set; }

        #endregion

        public bool? Available { get; set; }

        public bool Ignore { get; set; }

        public bool Hidden { get; set; }

        public bool Clickable { get; set; }

        public string Location { get; set; }

        public Func<ToolBarItem, HelperResult> Template { get; set; }

        public HtmlContent Content { get; set; }

        public Func<ToolBarItem, HelperResult> OverlayTemplate { get; set; }

        public string Group { get; set; }

        public virtual bool HasContent { get { return Icon != null || string.IsNullOrEmpty(ImageUrl) == false || Template != null || Content != null; } }

        public virtual bool HasLabel { get { return string.IsNullOrEmpty(Label) == false; } }

        protected virtual DropDown CreateDropDown()
        {
            return new DropDown
            {
                AutoClose = true,
                Top = 0,
                Right = 0,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                Direction = DropDownDirection.Down
            };
        }

        public ToolBarItem Copy()
        {
            return (ToolBarItem)MemberwiseClone();
        }

        protected virtual void AlignDropDown(DropDown dropDown)
        {
            if (Alignment == ToolBarAlignment.Left)
            {
                if (dropDown.Left == null && dropDown.Right == null)
                {
                    dropDown.Left = 0;
                }

                if (dropDown.HorizontalAlignment == HorizontalAlignment.None)
                {
                    dropDown.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
            else
            {
                if (dropDown.Left == null && dropDown.Right == null)
                {
                    dropDown.Right = 0;
                }

                if (dropDown.HorizontalAlignment == HorizontalAlignment.None)
                {
                    dropDown.HorizontalAlignment = HorizontalAlignment.Right;
                }
            }

            if (dropDown.Top == null && dropDown.Bottom == null)
            {
                dropDown.Top = 0;
            }

            if (dropDown.VerticalAlignment == VerticalAlignment.None)
            {
                dropDown.VerticalAlignment = VerticalAlignment.Bottom;
            }
        }

        protected virtual PopupActivation PopupActivation
        {
            get
            {
                return new PopupActivation
                {
                    ActivationMode = PopupActivationModes.Click,
                    ActivatorMode = PopupSelectorMode.Closest,
                    Activator = "li"
                };
            }
        }

        public virtual void RenderHtml(ToolBarRenderContext context)
        {
            if (context.Settings.NewLayout == false)
            {
                // TODO WRITE TO WRITER DIRECT!
                string templateHtml = Template != null ? Template(this).ToString().Trim() : Content != null ? Content.GetContentHtml(context.Helper, this) : null;

                if (Helpers.BeginsWithTag(templateHtml, "li") == true)
                {
                    context.HtmlWriter.Write(templateHtml);
                }
                else
                {
                    var itemTag = GetItemTag(context);

                    itemTag.Write(context.HtmlWriter, TagRenderMode.StartTag);

                    // Content Container

                    var containerTag = GetContainerTag(context);

                    if (containerTag != null)
                    {
                        containerTag.Write(context.HtmlWriter, TagRenderMode.StartTag);
                    }

                    if (TextAlignment == ToolBarAlignment.Right)
                    {
                        RenderContent(templateHtml, context);

                        RenderLabel(context);
                    }
                    else
                    {
                        RenderLabel(context);

                        RenderContent(templateHtml, context);
                    }

                    if (containerTag != null)
                    {
                        containerTag.Write(context.HtmlWriter, TagRenderMode.EndTag);
                    }

                    if (OverlayTemplate != null)
                    {
                        context.HtmlWriter.Write("<div class=\"overlay\">");
                        OverlayTemplate(this).WriteTo(context.HtmlWriter);
                        context.HtmlWriter.Write("</div>");
                    }

                    // Drop Down

                    if (DropDown != null)
                    {
                        string content = DropDownTemplate != null ? DropDownTemplate(null).ToString() : DropDownItems != null && DropDownItems.Any(i => i.Ignore == false) ? MenuHelper.Menu(context.Helper, DropDownItems, settings: MenuSettings).ToString() : null;

                        AlignDropDown(DropDown);

                        DropDownHelper.DropDown(context.Helper, context.HtmlWriter, content, DropDown, PopupActivation);
                    }
                    else if (DropDownTemplate != null)
                    {
                        string content = DropDownTemplate(null).ToString();

                        var dropDown = CreateDropDown();
                        AlignDropDown(dropDown);

                        DropDownHelper.DropDown(context.Helper, context.HtmlWriter, content, dropDown, PopupActivation);
                    }
                    else if (DropDownItems != null && DropDownItems.Any(i => i.Ignore == false))
                    {
                        string menuHtml = MenuHelper.Menu(context.Helper, DropDownItems, settings: MenuSettings).ToString();
                        var dropDown = CreateDropDown();
                        dropDown.CloseOnClick = true;
                        AlignDropDown(dropDown);

                        DropDownHelper.DropDown(context.Helper, context.HtmlWriter, menuHtml, dropDown, PopupActivation);
                    }

                    itemTag.Write(context.HtmlWriter, TagRenderMode.EndTag);
                }
            }
            else
            {
                var itemTag = GetItemTag(context);

                TagBuilder containerTag = null;

                if (Url != null)
                {
                    containerTag = new TagBuilder("a");
                    containerTag.MergeAttribute("href", Url);

                    if (AnchorTagHtmlAttributes != null)
                    {
                        containerTag.MergeAttributes(Helpers.GetHtmlAttributes(AnchorTagHtmlAttributes));
                    }

                    if (UrlTarget != null)
                    {
                        containerTag.MergeAttribute("target", UrlTarget);
                    }
                    containerTag.AddCssClass("container");
                }
                else
                {
                    itemTag.AddCssClass("container");
                }

                itemTag.Write(context.HtmlWriter, TagRenderMode.StartTag);

                if (containerTag != null) containerTag.Write(context.HtmlWriter, TagRenderMode.StartTag);

                RenderContent(context);

                if (context.Settings.DisplayLabels != false && HasLabel == true)
                {
                    context.HtmlWriter.Write("<label>");
                    //context.HtmlWriter.Write(context.RequestContext.GetApplicationTextTranslation(Label));
                    context.HtmlWriter.Write(Label);
                    context.HtmlWriter.Write("</label>");
                }

                if (containerTag != null) containerTag.Write(context.HtmlWriter, TagRenderMode.EndTag);

                if (OverlayTemplate != null)
                {
                    context.HtmlWriter.Write("<div class=\"overlay\">");
                    OverlayTemplate(this).WriteTo(context.HtmlWriter);
                    context.HtmlWriter.Write("</div>");
                }

                // Drop Down
                if (DropDown != null)
                {
                    string content = DropDownTemplate != null ? DropDownTemplate(null).ToString() : DropDownItems != null && DropDownItems.Any(i => i.Ignore == false) ? MenuHelper.Menu(context.Helper, DropDownItems, settings: MenuSettings).ToString() : null;

                    AlignDropDown(DropDown);

                    DropDownHelper.DropDown(context.Helper, context.HtmlWriter, content, DropDown, PopupActivation);
                }
                else if (DropDownTemplate != null)
                {
                    string content = DropDownTemplate(null).ToString();

                    var dropDown = CreateDropDown();
                    AlignDropDown(dropDown);

                    DropDownHelper.DropDown(context.Helper, context.HtmlWriter, content, dropDown, PopupActivation);
                }
                else if (DropDownItems != null && DropDownItems.Any(i => i.Ignore == false))
                {
                    string menuHtml = MenuHelper.Menu(context.Helper, DropDownItems, settings: MenuSettings).ToString();
                    var dropDown = CreateDropDown();
                    dropDown.CloseOnClick = true;
                    AlignDropDown(dropDown);

                    DropDownHelper.DropDown(context.Helper, context.HtmlWriter, menuHtml, dropDown, PopupActivation);
                }

                itemTag.Write(context.HtmlWriter, TagRenderMode.EndTag);
            }
        }

        protected virtual TagBuilder GetItemTag(ToolBarRenderContext context)
        {
            var itemTag = new TagBuilder("li");

            if (string.IsNullOrEmpty(Id) == false)
            {
                itemTag.GenerateId(Id);
            }

            if (string.IsNullOrEmpty(Name) == false)
            {
                itemTag.MergeAttribute("data-name", Name);
            }

            if (SpecialTypeCss != null)
            {
                itemTag.MergeAttribute("data-type", SpecialTypeCss);
            }

            if (HtmlAttributes != null)
            {
                var htmlAttributesAttributeBuilder = HtmlAttributes as AttributeBuilder;
                if (htmlAttributesAttributeBuilder != null)
                {
                    itemTag.MergeAttributes(htmlAttributesAttributeBuilder);
                }
                else
                {
                    itemTag.MergeAttributes(Helpers.GetHtmlAttributes(HtmlAttributes));
                }
            }

            if (Clickable == false)
            {
                itemTag.AddCssClass("not-clickable");
            }

            if (Checked == true)
            {
                itemTag.AddCssClass("checked");
                //itemTag.MergeAttribute("data-checked", Checked.Value ? "true" : "false");
            }

            if (CheckType != CheckType.None)
            {
                itemTag.AddCssClass("checkable");
                if (CheckType != CheckType.Normal)
                {
                    itemTag.MergeAttribute("data-check-type", CheckType == CheckType.Toggle ? "toggle" : "radio");
                }

                if (CheckGroup != null)
                {
                    itemTag.MergeAttribute("data-check-group", CheckGroup);
                }
                //itemTag.MergeAttribute("data-check-on-click", "true");
            }

            if ((ShowTooltip ?? context.Settings.ShowToolTip) == true && string.IsNullOrEmpty(ToolTip) == false)
            {
                itemTag.MergeAttribute("title", ToolTip);
            }

            if (Available.HasValue == true)
            {
                itemTag.AddCssClass(Available.Value == true ? "available" : "unavailable");
            }

            if (Hidden == true)
            {
                itemTag.AddCssClass("hidden");
            }

            if (context.DisplayContentAndLabel == true && HasContent != HasLabel)
            {
                itemTag.AddCssClass("expand");
            }

            itemTag.AddCssClass((ContentDirection ?? context.Settings.ContentDirection) == LayoutDirection.Horizontal ? "horizontal" : "vertical");

            if (Alignment == ToolBarAlignment.Right)
            {
                itemTag.AddCssClass("right-aligned");
            }

            if (Alignment == ToolBarAlignment.Center)
            {
                itemTag.AddCssClass("center-aligned");
            }

            if (TextAlignment == ToolBarAlignment.Left)
            {
                itemTag.AddCssClass("text-left-aligned");
            }

            if (Icon != null)
            {
                itemTag.AddCssClass("i");
            }

            if (HasLabel == true && context.Settings.DisplayLabels != false)
            {
                itemTag.AddCssClass("l");
            }

            if (ClassNames != null)
            {
                itemTag.AddCssClass(ClassNames);
            }

            return itemTag;
        }

        protected virtual TagBuilder GetContainerTag(ToolBarRenderContext context)
        {
            TagBuilder containerTag = null;

            if (Url != null)
            {
                containerTag = new TagBuilder("a");
                containerTag.MergeAttribute("href", Url);
                if (UrlTarget != null)
                {
                    containerTag.MergeAttribute("target", UrlTarget);
                }
            }
            else if ((ContentDirection ?? context.Settings.ContentDirection) == LayoutDirection.Vertical)
            {
                containerTag = new TagBuilder("div");
            }

            if (Template != null && Icon != null)
            {
                containerTag = new TagBuilder("div");
                containerTag.AddCssClass("i-and-t");
            }

            return containerTag;
        }

        protected virtual void RenderContent(string templateHtml, ToolBarRenderContext context)
        {
            if ((ContentDirection ?? context.Settings.ContentDirection) == LayoutDirection.Vertical)
            {
                if (Icon != null)
                {
                    context.HtmlWriter.Write("<div class=\"icon\">");
                    //IconHelper.IconInternal(Icon, context.HtmlWriter, size: Size);
                    context.HtmlWriter.Write("</div>");
                }
                else if (string.IsNullOrEmpty(ImageUrl) == false)
                {
                    context.HtmlWriter.Write("<img class=\"content\" src=\"{0}\" />", ImageUrl);
                }

                if (templateHtml != null)
                {
                    context.HtmlWriter.WriteConcat("<div class=\"content\">", templateHtml, "</div>");
                }
            }
            else
            {
                //if (Icon != null)
                //{
                //    IconHelper.IconInternal(Icon, context.HtmlWriter, size: Size);
                //}
                //else
                if (string.IsNullOrEmpty(ImageUrl) == false)
                {
                    context.HtmlWriter.Write("<img src=\"{0}\" />", ImageUrl);
                }

                if (templateHtml != null)
                {
                    context.HtmlWriter.Write(templateHtml);
                }
            }
        }

        protected virtual void RenderContent(ToolBarRenderContext context)
        {
            //if (Icon != null)
            //{
            //    IconHelper.IconInternal(Icon, writer: context.HtmlWriter, @class: "content", size: Size);
            //}
            //else 
            if (string.IsNullOrEmpty(ImageUrl) == false)
            {
                context.HtmlWriter.Write("<img class=\"content\" src=\"{0}\" />", ImageUrl);
            }

            string templateHtml = Template != null ? Template(this).ToString().Trim() : Content != null ? Content.GetContentHtml(context.Helper, this) : null;
            if (templateHtml != null)
            {
                context.HtmlWriter.WriteConcat(templateHtml);
                //context.HtmlWriter.WriteConcat("<div class=\"content\">", templateHtml, "</div>");
            }
        }

        protected virtual void RenderLabel(ToolBarRenderContext context)
        {
            if (context.Settings.DisplayLabels != false && HasLabel == true)
            {
                if ((ContentDirection ?? context.Settings.ContentDirection) == LayoutDirection.Vertical)
                {
                    context.HtmlWriter.Write("<span class=\"label\">");
                    context.HtmlWriter.Write(Label);
                    context.HtmlWriter.Write("</span>");
                }
                else
                {
                    context.HtmlWriter.Write(Label);
                }
            }
        }

        protected virtual string SpecialTypeCss { get { return null; } }
    }

    public class ToolBarImageItem : ToolBarItem
    {
        public string ImageSmallUrl { get; set; }
        protected override string SpecialTypeCss { get { return "image"; } }

    }

    public class ToolBarCheckItem : ToolBarItem
    {
        protected override string SpecialTypeCss {  get { return "check"; } }
    }

    public class ToolBarDropDownListItem : ToolBarItem
    {
        //public override string Label { get { return base.Label ?? SelectedItems.Select(i => i.Label).FirstOrDefault(); } set { base.Label = value; } }

        protected override string SpecialTypeCss { get { return "drop-down-list"; } }
    }

    public class ToolBarButtonItem : ToolBarItem
    {
        protected override string SpecialTypeCss { get { return "button"; } }

    }

    public static class UrlTargets
    {
        public const string Blank = "_blank";

        public const string Self = "_self";

        public const string Parent = "_parent";

        public const string Top = "_top";
    }

    public class ToolBarRenderContext
    {
        public ToolBarRenderContext(HtmlHelper helper, TextWriter htmlWriter, ToolBarSettings settings, IEnumerable<ToolBarItem> items)
        {
            Helper = helper;
            HtmlWriter = htmlWriter;
            Settings = settings;
            Items = items.Where(item => item.Ignore == false).ToArray();
            HasContentAndLabel = Items.Any(item => item.HasContent && item.HasLabel);
            HasContents = HasContentAndLabel || Items.Any(item => item.HasContent);
            HasLabels = HasContentAndLabel || Items.Any(item => item.HasLabel);
            DisplayContentAndLabel = settings.DisplayContent != false && settings.DisplayLabels != false && HasContentAndLabel == true;
        }

        public HtmlHelper Helper { get; private set; }

        private ApplicationRequestContext requestContext;
        public ApplicationRequestContext RequestContext {  get { return requestContext ?? (requestContext = ApplicationRequestContext.GetContext(Helper.ViewContext.HttpContext)); } }

        public TextWriter HtmlWriter { get; private set; }

        public ToolBarSettings Settings { get; private set; }

        public IEnumerable<ToolBarItem> Items { get; private set; }

        public bool HasContents { get; private set; }

        public bool HasLabels { get; private set; }

        public bool HasContentAndLabel { get; private set; }

        public bool DisplayContentAndLabel { get; private set; }
    }

    public enum ToolBarAlignment { Left, Right, Center, NotSet }

    public enum TargetDevice { Desktop, Tablet, Phone, NotSet }

    public static class ToolBarHelper
    {
        public static MvcHtmlString ToolBar(this HtmlHelper htmlHelper, IEnumerable<ToolBarItem> items, ToolBarSettings settings = null, object htmlAttributes = null)
        {
            var htmlBuilder = new StringBuilder();
            ToolBarHelper.ToolBar(htmlHelper, htmlBuilder, items, settings, htmlAttributes);
            return MvcHtmlString.Create(htmlBuilder.ToString());
        }

        public static void ToolBar(this HtmlHelper helper, StringBuilder htmlBuilder, IEnumerable<ToolBarItem> items, ToolBarSettings settings = null, object htmlAttributes = null)
        {
            using (var writer = new StringWriter(htmlBuilder, CultureInfo.CurrentUICulture))
            {
                ToolBarHelper.ToolBar(helper, writer, items, settings, htmlAttributes);
            }
        }

        public static void ToolBar(this HtmlHelper helper, TextWriter writer, IEnumerable<ToolBarItem> items, ToolBarSettings settings = null, object htmlAttributes = null)
        {
            var context = new ToolBarRenderContext(helper, writer, settings ?? ToolBarSettings.Default, items ?? Enumerable.Empty<ToolBarItem>());

            var toolbarTag = new TagBuilder("ul");

            var attributeBuilder = htmlAttributes as AttributeBuilder;
            if (attributeBuilder != null)
            {
                toolbarTag.MergeAttributes(attributeBuilder);
            }
            else if (htmlAttributes != null)
            {
                toolbarTag.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            }

            //toolbarTag.AddCssClass(context.Settings.DisplayContent != false && context.HasContents == true ? "contents" : "no-contents");

            toolbarTag.AddCssClass(context.Settings.DisplayLabels != false && context.HasLabels == true ? "labels" : "no-labels");

            //if (context.HasContentAndLabel == true)
            //{
            //    toolbarTag.AddCssClass("full");
            //}

            if (context.Settings.Compact == true)
            {
                toolbarTag.AddCssClass("compact");
            }

            if (context.Settings.Highlight == true)
            {
                toolbarTag.AddCssClass("highlight");
            }

            //bool displayContents = context.Settings.DisplayContent != false && context.HasContents == true;
            //bool displayLabel = context.Settings.DisplayLabels != false && context.HasLabels == true;
            
            if (context.DisplayContentAndLabel == false)
            {
                toolbarTag.AddCssClass("collapse");
            }

            switch (context.Settings.Size)
            {
                case ToolBarSize.Small:
                    toolbarTag.AddCssClass("small");
                    break;
                case ToolBarSize.Medium:
                    toolbarTag.AddCssClass("medium");
                    break;
                case ToolBarSize.Large:
                    toolbarTag.AddCssClass("large");
                    break;
                default:
                    break;
            }


            if (settings!=null && settings.NewLayout == true)
            {
                toolbarTag.AddCssClass("new");
            }

            toolbarTag.AddCssClass("multivendor-toolbar");

            toolbarTag.Write(context.HtmlWriter, TagRenderMode.StartTag);

            if (context.Items != null)
            {
                RenderToolBarItems(context);
            }

            toolbarTag.Write(context.HtmlWriter, TagRenderMode.EndTag);
        }

        public static MvcHtmlString RenderToolBarItems(HtmlHelper helper, IEnumerable<ToolBarItem> items, ToolBarSettings settings = null)
        {
            var htmlBuilder = new StringBuilder();
            ToolBarHelper.RenderToolBarItems(helper, htmlBuilder, items, settings);
            return MvcHtmlString.Create(htmlBuilder.ToString());
        }
        public static void RenderToolBarItems(HtmlHelper helper, StringBuilder htmlBuilder, IEnumerable<ToolBarItem> items, ToolBarSettings settings = null)
        {
            using (var writer = new StringWriter(htmlBuilder, CultureInfo.CurrentUICulture))
            {
                ToolBarHelper.RenderToolBarItems(helper, writer, items, settings);
            }
        }

        public static void RenderToolBarItems(HtmlHelper helper, TextWriter htmlWriter, IEnumerable<ToolBarItem> items, ToolBarSettings settings = null)
        {
            var context = new ToolBarRenderContext(helper, htmlWriter, settings ?? ToolBarSettings.Default, items);
            RenderToolBarItems(context);
        }

        public static void RenderToolBarItems(ToolBarRenderContext context)
        {
            if (context.Items != null)
            {
                foreach (var item in context.Items)
                {
                    item.RenderHtml(context);
                }
            }
        }
    }
}