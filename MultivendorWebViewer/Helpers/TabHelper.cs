using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using System.IO;
using System.Globalization;

namespace MultivendorWebViewer.Helpers
{
    public class TabSettings
    {
        public TabSettings()
        {
            Hottrack = true;

            HeaderTag = "h3";
        }

        public bool Hottrack { get; set; }
        public bool IsPresentationTab { get; set; }
        public string HeaderTag { get; set; }

        public HtmlContent Header { get; set; }

        public string SelectionGroup { get; set; }

        public Func<TabItem, HelperResult> HeaderTemplate
        {
            set { Header = value != null ? new HtmlContent(m => value((TabItem)m)) : null; }
        }
    }

    public class TabItem
    {
        public TabItem()
        {

        }

        public string Name { get; set; }

        public string HeaderLabel { get; set; }


        public string HeaderToolTip { get; set; }

        public string Url { get; set; }

        public object HtmlAttributes { get; set; }

        public bool Ignore { get; set; }

        public IEnumerable<ToolBarItem> ToolBarItems { get; set; }

        public ToolBarSettings ToolBarSettings { get; set; }

        public bool Selected { get; set; }

        public HtmlContent Content { get; set; }

        public Func<object, HelperResult> ContentTemplate
        {
            set { Content = value != null ? new HtmlContent(value) : null; }
        }

        public HtmlContent Header { get; set; }

        public Func<TabItem, HelperResult> HeaderTemplate
        {
            set { Header = value != null ? new HtmlContent(m => value((TabItem)m)) : null; }
        }
    }

    public static class TabHelper
    {
        public static void RenderTab(this HtmlHelper htmlHelper, IEnumerable<TabItem> tabs, TabSettings settings = null, object htmlAttributes = null)
        {
            TabHelper.Tab(htmlHelper, htmlHelper.ViewContext.Writer, tabs, settings, htmlAttributes);
        }

        public static MvcHtmlString Tab(this HtmlHelper htmlHelper, IEnumerable<TabItem> tabs, TabSettings settings = null, object htmlAttributes = null)
        {
            var htmlBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(htmlBuilder, CultureInfo.InvariantCulture))
            {
                TabHelper.Tab(htmlHelper, stringWriter, tabs, settings, htmlAttributes);
            }
            return MvcHtmlString.Create(htmlBuilder.ToString());
        }

        public static void Tab(this HtmlHelper htmlHelper, StringBuilder htmlBuilder, IEnumerable<TabItem> tabs, TabSettings settings = null, object htmlAttributes = null)
        {
            using (var stringWriter = new StringWriter(htmlBuilder, CultureInfo.InvariantCulture))
            {
                TabHelper.Tab(htmlHelper, stringWriter, tabs, settings, htmlAttributes);
            }
        }

        public static void Tab(this HtmlHelper htmlHelper, TextWriter htmlWriter, IEnumerable<TabItem> tabs, TabSettings settings = null, object htmlAttributes = null)
        {
            if (settings == null)
            {
                settings = new TabSettings();
            }

            var tabTag = new TagBuilder("section");

            if (htmlAttributes != null)
            {
                tabTag.MergeAttributes(Helpers.GetHtmlAttributes(htmlAttributes));
            }

            if (settings.SelectionGroup != null)
            {
                tabTag.MergeAttribute("data-selection-group", settings.SelectionGroup);
            }

            //listTag.MergeAttribute("data-selection-mode", settings.SelectionMode == ListSelectMode.Single ? "single" : settings.SelectionMode == ListSelectMode.Multi ? "multiple" : "none");

            if (settings.Hottrack == true)
            {
                tabTag.AddCssClass("hottrack");
            }

            tabTag.AddCssClass("multivendor-web-tab");


            tabTag.Write(htmlWriter, TagRenderMode.StartTag);

            var validTabs = tabs.Where(t => t != null && t.Ignore == false);



            if (settings != null && settings.IsPresentationTab)
            {

                foreach (var tab in validTabs)
                {
                    //tab header
                    var tabHeaderTag = new TagBuilder("section");

                    if (tab.Name != null)
                    {
                        tabHeaderTag.MergeAttribute("data-name", tab.Name);
                    }

                    if (tab.Selected == true)
                    {
                        tabHeaderTag.AddCssClass("selected");
                    }

                    tabHeaderTag.AddCssClass("tab-header");

                    if (tab.HeaderToolTip != null)
                    {
#if DEBUG
                        throw new NotImplementedException("Tabs tooltip");
#endif
                    }

                    tabHeaderTag.Write(htmlWriter, TagRenderMode.StartTag);

                    var header = tab.Header ?? settings.Header;

                    if (header != null)
                    {
                        header.WriteContentHtml(htmlHelper, htmlWriter, tab);
                    }
                    else
                    {
                        if (settings.HeaderTag != null)
                        {
                            htmlWriter.Write("<");
                            htmlWriter.Write(settings.HeaderTag);
                            htmlWriter.Write(" class=\"tab-header-label\">");
                        }

                        //if (tab.HeaderIcon != null)
                        //{
                        //    tab.HeaderIcon.WriteHtml(htmlWriter);
                        //}

                        if (tab.HeaderLabel != null)
                        {
                            //headersHtmlBuilder.Append("<span class=\"tab-label\">");
                            htmlWriter.Write(tab.HeaderLabel);
                            //headersHtmlBuilder.Append("</span>");
                        }

                        if (settings.HeaderTag != null)
                        {
                            htmlWriter.Write("</");
                            htmlWriter.Write(settings.HeaderTag);
                            htmlWriter.Write(">");
                        }

                        if (tab.ToolBarItems != null && tab.ToolBarItems.Any() == true)
                        {
                            htmlWriter.Write(ToolBarHelper.ToolBar(htmlHelper, tab.ToolBarItems, tab.ToolBarSettings, new { @class = "tab-tools" }));
                        }
                    }

                    tabHeaderTag.Write(htmlWriter, TagRenderMode.EndTag);

                    //tab pages
                    var tabPageTag = new TagBuilder("section");

                    if (tab.HtmlAttributes != null)
                    {
                        tabPageTag.MergeAttributes(Helpers.GetHtmlAttributes(tab.HtmlAttributes));
                    }

                    if (tab.Selected == true)
                    {
                        tabPageTag.AddCssClass("selected");
                    }

                    if (tab.Url != null)
                    {
                        tabPageTag.MergeAttribute("data-url", tab.Url);
                        tabPageTag.AddCssClass("not-loaded");
                    }

                    tabPageTag.AddCssClass("tab-page");

                    tabPageTag.Write(htmlWriter, TagRenderMode.StartTag);

                    if (tab.Content != null)
                    {
                        tab.Content.WriteContentHtml(htmlHelper, htmlWriter, tab);
                    }

                    tabPageTag.Write(htmlWriter, TagRenderMode.EndTag);
                }
            }
            else {
                // Headers

                Action<HtmlHelper, TextWriter> headerHtmlAction = (headerHtmlHelper, headerHtmlWriter) =>
                {
                    foreach (var tab in validTabs)
                    {
                        var tabHeaderTag = new TagBuilder("section");

                        if (tab.Name != null)
                        {
                            tabHeaderTag.MergeAttribute("data-name", tab.Name);
                        }

                        if (tab.Selected == true)
                        {
                            tabHeaderTag.AddCssClass("selected");
                        }

                        tabHeaderTag.AddCssClass("tab-header");

                        if (tab.HeaderToolTip != null)
                        {
#if DEBUG
                            throw new NotImplementedException("Tabs tooltip");
#endif
                        }

                        tabHeaderTag.Write(headerHtmlWriter, TagRenderMode.StartTag);

                        var header = tab.Header ?? settings.Header;

                        if (header != null)
                        {
                            header.WriteContentHtml(headerHtmlHelper, headerHtmlWriter, tab);
                        }
                        else
                        {
                            if (settings.HeaderTag != null)
                            {
                                headerHtmlWriter.Write("<");
                                headerHtmlWriter.Write(settings.HeaderTag);
                                headerHtmlWriter.Write(" class=\"tab-header-label\">");
                            }

                            

                            if (tab.HeaderLabel != null)
                            {
                                //headersHtmlBuilder.Append("<span class=\"tab-label\">");
                                headerHtmlWriter.Write(tab.HeaderLabel);
                                //headersHtmlBuilder.Append("</span>");
                            }

                            if (settings.HeaderTag != null)
                            {
                                headerHtmlWriter.Write("</");
                                headerHtmlWriter.Write(settings.HeaderTag);
                                headerHtmlWriter.Write(">");
                            }

                            if (tab.ToolBarItems != null && tab.ToolBarItems.Any() == true)
                            {
                                headerHtmlWriter.Write(ToolBarHelper.ToolBar(headerHtmlHelper, tab.ToolBarItems, tab.ToolBarSettings, new { @class = "tab-tools" }));
                            }
                        }

                        tabHeaderTag.Write(headerHtmlWriter, TagRenderMode.EndTag);
                    }
                };

                // Pages
                Action<HtmlHelper, TextWriter> pagesHtmlAction = (pagesHtmlHelper, pagesHtmlWriter) =>
                {
                    foreach (var tab in validTabs)
                    {
                        var tabPageTag = new TagBuilder("section");

                        if (tab.HtmlAttributes != null)
                        {
                            tabPageTag.MergeAttributes(Helpers.GetHtmlAttributes(tab.HtmlAttributes));
                        }

                        if (tab.Selected == true)
                        {
                            tabPageTag.AddCssClass("selected");
                        }

                        if (tab.Url != null)
                        {
                            tabPageTag.MergeAttribute("data-url", tab.Url);
                            tabPageTag.AddCssClass("not-loaded");
                        }

                        tabPageTag.AddCssClass("tab-page");

                        tabPageTag.Write(pagesHtmlWriter, TagRenderMode.StartTag);

                        if (tab.Content != null)
                        {
                            tab.Content.WriteContentHtml(pagesHtmlHelper, pagesHtmlWriter, tab);
                        }

                        tabPageTag.Write(pagesHtmlWriter, TagRenderMode.EndTag);
                    }
                };
               LayoutHelper.Layout(htmlHelper, htmlWriter, new LayoutDefinition[]
              {
                new LayoutNavigation
                {
                    ClassNames = "tab-headers",
                    ScrollMode = LayoutScrollMode.ScrollX,
                    Content = new HtmlContent(headerHtmlAction)
                },
                new LayoutDefinition
                {
                    ClassNames = "tab-pages",
                    ScrollMode = LayoutScrollMode.ScrollY,
                    FillWeight = 1,
                    Content = new HtmlContent(pagesHtmlAction)
                }
              });
            }
          

            //htmlWriter.Write(html);

            tabTag.Write(htmlWriter, TagRenderMode.EndTag);
        }
    }
}