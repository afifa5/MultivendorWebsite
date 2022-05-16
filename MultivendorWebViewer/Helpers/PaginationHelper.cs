using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
#if NET452
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using System.Web.Helpers;
#endif
using System.Xml.Serialization;
using Newtonsoft.Json;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Controllers;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.ViewModels;
using System.Web.Mvc;
using MultivendorWebViewer.Components;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.ComponentModel;

namespace MultivendorWebViewer.Helpers
{
    public static class DataViewHelper
    {
        public static IDictionary<string, object> GetUpdateViewOptions(bool? content = null, bool? pagination = null, bool? resetPaging = null, bool? tools = null)
        {
            var options = new Dictionary<string, object>();
            if (content.HasValue) options["content"] = content.Value;
            if (pagination.HasValue) options["pagination"] = pagination.Value;
            if (resetPaging.HasValue) options["content"] = resetPaging.Value;
            if (tools.HasValue) options["tools"] = tools.Value;
            return options;
        }

        public static AttributeBuilder UpdateViewOptionsAttribute(bool? content = null, bool? pagination = null, bool? resetPaging = null, bool? tools = null)
        {
            return new AttributeBuilder().Attr("data-view-update", JsonConvert.SerializeObject(GetUpdateViewOptions(content, pagination, resetPaging, tools), new JsonSerializerSettings { Formatting = Formatting.None }));
        }

        public static MvcHtmlString DataView(this HtmlHelper helper, DataViewOptions options, DataViewState state = null, object htmlAttributes = null, HtmlContent content = null)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder, CultureInfo.CurrentUICulture))
            {
                DataViewHelper.DataView(helper, writer, options, state, htmlAttributes, content);
            }
            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        public static void DataView(this HtmlHelper helper, TextWriter writer, DataViewOptions options, DataViewState state = null, object htmlAttributes = null, HtmlContent content = null)
        {
            var requestContext = helper.ViewContext.GetApplicationRequestContext();

            //if (selection == null) selection = new DataViewRequest();

            if (options == null) options = new DataViewOptions();

            var htmlAttributeBuilder = new AttributeBuilder(htmlAttributes);

            int page = state != null && state.Pagination != null ? (state.Pagination.PageIndex ?? 0) : 0;
            htmlAttributeBuilder.Attr("data-page", page);

            if (options.Url != null)
            {
                htmlAttributeBuilder.Attr("data-url", options.Url);
            }



            if (state != null && state.ItemCount.HasValue == true)
            {
                //int pageSize = selection.PageSize ?? options.DefaultPageSize ?? options.PageSizes.First().Count;

                //int numberOfPages = itemCount / pageSize;
                //if (numberOfPages < itemCount * pageSize) numberOfPages++;

                htmlAttributeBuilder.Attr("data-item-count", state.ItemCount.Value);
            }

            if (options.DelayedLoad > 0)
            {
                htmlAttributeBuilder.Attr("data-delayed-load", options.DelayedLoad.Value);
            }

            var dataViewTagBuilder = new TagBuilder("section");
            dataViewTagBuilder.MergeAttributes(htmlAttributeBuilder);
            dataViewTagBuilder.AddCssClass("multivendor-dataview");
            dataViewTagBuilder.AddCssClass("init");
            if (options.LayoutFill == true)
            {
                dataViewTagBuilder.AddCssClass("fill");
            }
            //if (options.Class != null)
            //{
            //    dataViewTagBuilder.AddCssClass(options.Class);
            //}

            dataViewTagBuilder.Write(writer, TagRenderMode.StartTag);

            //if (options.Label != null)
            //{
            //    helper.RenderComponentLabel(writer, options, requestContext: requestContext);
            //}

            //var layoutDefs = new List<LayoutDefinition>(3);

            if ((options.DisplaySortSelector != false && options.SortSelectors != null) || (options.DisplayViewSelector != false && options.ViewSelectors != null) || (options.DisplayFilter != false && options.FilterSelectors != null) 
                /*|| options.DownloadToolAvailable != false*/ || options.Caption != null || options.CaptionHtml != null)
            {

                RenderTools(helper, writer, options, /*state*/ null, requestContext);

            }

            if (options.ContentSelector != null)
            {
                htmlAttributeBuilder.Attr("data-content-selector", JsonConvert.SerializeObject(options.ContentSelector, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
            else
            {
                var contentContainerTagBuilder = new TagBuilder("div");
                contentContainerTagBuilder.AddCssClass("content-container");

                if (content == null)
                {
                    contentContainerTagBuilder.Write(writer, TagRenderMode.Normal);
                }
                else
                {
                    contentContainerTagBuilder.Write(writer, TagRenderMode.StartTag);
                    content.WriteContentHtml(helper, writer);
                    contentContainerTagBuilder.Write(writer, TagRenderMode.EndTag);
                }
              
            }

            if (options.PageSizes != null)
            {
                RenderPaginationTools(helper, writer, options, null, requestContext, renderOnlyItems: false);
            }

            dataViewTagBuilder.Write(writer, TagRenderMode.EndTag);
        }

        public static MvcHtmlString RenderTools(HtmlHelper helper, DataViewOptions options, DataViewState state = null, ApplicationRequestContext requestContext = null)
        {
            if (options == null) options = new DataViewOptions();
            var htmlBuilder = new StringBuilder(); // SHould be a text writer with iformatter set
            RenderTools(helper, htmlBuilder, options, state, requestContext);
            return MvcHtmlString.Create(htmlBuilder.ToString());
        }

        private static void RenderTools(HtmlHelper helper, StringBuilder htmlBuilder, DataViewOptions options, DataViewState state = null, ApplicationRequestContext requestContext = null)
        {
            using (var writer = new StringWriter(htmlBuilder, CultureInfo.CurrentUICulture))
            {
                DataViewHelper.RenderTools(helper, writer, options, state, requestContext);
            }
        }

        private static void RenderTools(HtmlHelper helper, TextWriter writer, DataViewOptions options, DataViewState state = null, ApplicationRequestContext requestContext = null)
        {
            if (requestContext == null)
            {
                requestContext = helper.GetApplicationRequestContext();
            }

            var matchContext = new MatchContext(requestContext);

            var items = new List<ToolBarItem>();

            var tools = options.Tools.ToArray();

            Action<string, ToolBarAlignment> addTools = (location, alignment) =>
            {
                foreach (var tool in tools.OfType<ToolBarItem>().Where(t => location.Equals(t.Location) == true))
                {
                    if (tool.Alignment == ToolBarAlignment.NotSet) tool.Alignment = alignment;
                    tool.ClassNames = tool.ClassNames == null ? "custom-tool" : string.Concat(tool.ClassNames, " custom-tool");
                    items.Add(tool);
                }
            };

            // Add tool next to filter
            addTools(DataViewOptions.ToolLocations.AfterFilterSelector, ToolBarAlignment.Left);

            // Add sort selectors
            addTools(DataViewOptions.ToolLocations.BeforeSortSelectors, ToolBarAlignment.Left);
            var sortAligment = options.SortAlignment;
            var sortsToolBarItem = GetSortToolBarItem(options, requestContext, state);
            if (sortsToolBarItem != null)
            {
                items.Add(sortsToolBarItem);
            }

            addTools(DataViewOptions.ToolLocations.AfterSortSelectors, ToolBarAlignment.Left);
            // Add view selectors
            addTools(DataViewOptions.ToolLocations.BeforeViewSelectors, ToolBarAlignment.Left);

            if (options.DisplayViewSelector != false && options.ViewSelectors != null)
            {
                var availableViewSelectors = options.ViewSelectors.Where(s => s.IsAvailable(triggerMatchContext: matchContext) == true).OrderBy(s => s.Order).ToArray();
                if (availableViewSelectors.Length > 0)
                {
                    string selectedViewId = state != null ? state.ViewId ?? options.DefaultViewSelector : options.DefaultViewSelector;
                    foreach (var viewSelector in availableViewSelectors)
                    {
                        var customClass = string.Empty;
                        if (availableViewSelectors.Length > 1)
                        {
                            if (availableViewSelectors[0].Id == viewSelector.Id)
                                customClass = "first";
                            if (availableViewSelectors[availableViewSelectors.Length - 1].Id == viewSelector.Id)
                                customClass = "last";
                        }
                        else {
                            customClass = "all";
                        }
                        var item = new ToolBarItem
                        {
                            Name = viewSelector.Id,
                            Icon = requestContext.GetIcon(viewSelector.Icon),
                            Label = requestContext.GetApplicationTextTranslation(viewSelector.Label),
                            ToolTip = viewSelector.ToolTip,
                            //Url = viewSelector.Url, // TODO Not like this?
                            Checked = selectedViewId != null ? StringComparer.OrdinalIgnoreCase.Equals(selectedViewId, viewSelector.Id) : false,
                            CheckType = CheckType.Radio,
                            CheckGroup = "view",
                            Alignment = ToolBarAlignment.Left,
                            ClassNames = "view-tool "+ customClass
                        };

                        items.Add(item);
                    }
                }
            }

            addTools(DataViewOptions.ToolLocations.AfterViewSelectors, ToolBarAlignment.Left);


            if (options.DownloadToolAvailable != false)
            {
                // Add download tool
                var downloadItem = new ToolBarItem
                {
                    Name = "download",
                    ToolTip = requestContext.GetApplicationTextTranslation("Download"),
                    ClassNames = "download-tool",
                    Alignment = ToolBarAlignment.Left,
                    Icon = requestContext.GetIcon(Icons.Save),
                };

                items.Add(downloadItem);
            }

            Action<TextWriter> writeTools = w =>
            {
                var settings = new ToolBarSettings { Direction = LayoutDirection.Horizontal, Highlight = true, Size = ToolBarSize.Medium, ContentDirection = LayoutDirection.Horizontal, ShowToolTip = false };
                //if (renderOnlyItems == true)
                //{
                //    ToolBarHelper.RenderToolBarItems(helper, h, items, settings);
                //}
                //else
                //{
                    ToolBarHelper.ToolBar(helper, w, items, settings, htmlAttributes: new { @class = "tools" });
                //}
            };

            var availableFilterSelectors = options.FilterSelectors != null && options.DisplayFilter != false ? options.FilterSelectors.Where(s => s.IsAvailable(null, matchContext) == true).OrderBy(s => s.Order).ToArray() : new DataViewFilterSelector[0];

            Action<TextWriter> writeFilter = w =>
            {

                var filterContainerTag = new TagBuilder("div");
                filterContainerTag.AddCssClass("filter");
                filterContainerTag.Write(w, TagRenderMode.StartTag);

                var filterInputTag = new TagBuilder("input");
                filterInputTag.AddCssClass("query-input");
                filterInputTag.Attributes.Add("autocomplete","off");
                if (state != null && state.FilterQuery != null)
                {
                    filterInputTag.MergeAttribute("value", state.FilterQuery);
                }
                //if (options.FilterPlaceholder != null)
                //{
                //    filterInputTag.MergeAttribute("placeholder", options.FilterPlaceholder);
                //}
                //else
                //{
                var checkedFilterProperties = (state != null && state.Filter != null && state.Filter.Properties != null ? state.Filter.Properties.Select(p => p.Id).ToSetOptimized(StringComparer.OrdinalIgnoreCase) : null /*availableFilterSelectors.Select(f => f.Id)*/);
                if (availableFilterSelectors != null && availableFilterSelectors.Length > 0)
                    {
                        string placeHolder = string.Empty;
                        if (state == null || state.Filter == null || state.Filter.Properties == null) {
                            checkedFilterProperties = availableFilterSelectors.Select(f => f.Id).ToSetOptimized(StringComparer.OrdinalIgnoreCase);
                        }
                        if (checkedFilterProperties != null && checkedFilterProperties.Count() == availableFilterSelectors.Count())
                        {
                            placeHolder = string.Format("Filter on selector all");
                        }
                        else if (checkedFilterProperties != null && checkedFilterProperties.Count() < availableFilterSelectors.Count())
                        {
                            var selectedSelectors = string.Join(", ", availableFilterSelectors.Where(f => checkedFilterProperties.Contains(f.Id) == true).Select(f => requestContext.GetApplicationTextTranslation(f.Label)));
                            placeHolder = string.Format("Filter on selector", selectedSelectors);
                        }
                        else {

                            placeHolder = string.Format("Filter on selector", "nothing");
                        }

                        filterInputTag.MergeAttribute("placeholder", placeHolder);
                    }
                //}

                filterInputTag.Write(w, TagRenderMode.SelfClosing);

                if (availableFilterSelectors != null && availableFilterSelectors.Length > 0)
                {
                    var filterSettingsButton = new TagBuilder("div");
                    filterSettingsButton.AddCssClass("filter-settings-button");
                    filterSettingsButton.Write(w, TagRenderMode.StartTag);

                    var icon = requestContext.GetIcon(Icons.CaretDown);
                    IconRendererProvider.GetRenderer(icon).WriteHtml(icon, w, classNames: "filter-tool");
                    var menuItems = availableFilterSelectors.Select(f => new MenuItem
                    {
                        Name = f.Id,
                        Label = requestContext.GetApplicationTextTranslation(f.Label),
                        Icon = requestContext.GetIcon(f.Icon),
                        Selected = checkedFilterProperties != null && checkedFilterProperties.Contains(f.Id),
                        CheckType = CheckType.Toggle,
                        HtmlAttributes = new Dictionary<string, object> { { "data-property", f.Property } } // TODO SECURITY RISK!!
                    }).ToArray();

                    string menuHtml = MenuHelper.Menu(helper, menuItems).ToString();

                    DropDownHelper.DropDown(helper, w, menuHtml, new DropDown
                    {
                        AutoClose = true,
                        Top = 0,
                        Right = 0,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Direction = DropDownDirection.Down,
                        CloseOnClick = true,
                        ClassNames = "filter-properties"
                    }, new PopupActivation
                    {
                        ActivationMode = PopupActivationModes.Click,
                        ActivatorMode = PopupSelectorMode.Closest,
                        Activator = ".filter-settings-button"
                    });

                    filterSettingsButton.Write(w, TagRenderMode.EndTag);
                }

                filterContainerTag.Write(w, TagRenderMode.EndTag);
            };         

            // Add filter
            bool filterAvailable = options.DisplayFilter == true || availableFilterSelectors.Length > 1; // Do not display filters selector is only one avaiable. make no sense to be able to disable it.
            bool toolsAvailable = items.Count > 0;

            var toolsContainerTagBuilder = new TagBuilder("section");
            toolsContainerTagBuilder.AddCssClass("tools-container");
            toolsContainerTagBuilder.Write(writer, TagRenderMode.StartTag);

            if (options.CaptionHtml != null && options.Caption != null)
            {
                var captionTagBuilder = new TagBuilder("div");
                captionTagBuilder.AddCssClass("caption-container");
                captionTagBuilder.Write(writer, TagRenderMode.StartTag);
                var captionHtml = options.CaptionHtml as HtmlContent;
                if (captionHtml != null)
                {
                    captionHtml.WriteContentHtml(helper, writer);
                }
                else
                {
                    writer.WriteConcat("<h3>", requestContext.GetApplicationTextTranslation(options.Caption), "</h3>");
                }
                captionTagBuilder.Write(writer, TagRenderMode.EndTag);
            }

            if (options.DisplayItemCountDescription == true && state != null)
            {
                RenderItemCountDescription(helper, writer, options, state);
            }

            if (filterAvailable == true)
            {
                var filterTagBuilder = new TagBuilder("div");
                filterTagBuilder.AddCssClass("filter-container");

                filterTagBuilder.Write(writer, TagRenderMode.StartTag);
                writeFilter(writer);
                filterTagBuilder.Write(writer, TagRenderMode.EndTag);
            }

            if (toolsAvailable == true)
            {
                writeTools(writer);
            }

            toolsContainerTagBuilder.Write(writer, TagRenderMode.EndTag);

            //if (filterAvailable == true && toolsAvailable == true)
            //{
            //    LayoutHelper.Layout(helper, htmlBuilder, new LayoutDefinition[]
            //    {
            //        new LayoutDefinition
            //        {
            //            Ignore = captionHtml == null,
            //            Content = captionHtml,
            //            ClassNames = "caption-container"
            //        },
            //        new LayoutDefinition
            //        {
            //            Ignore = filterAvailable == false,
            //            ClassNames = "filter-container",
            //            FillWeight = 1,
            //            Content = new HtmlContent((HtmlHelper htmlHelper, StringBuilder h) => writeFilter(h))
            //        },
            //        new LayoutContent
            //        {
            //            Ignore = toolsAvailable == false,
            //            Content = new HtmlContent((HtmlHelper htmlHelper, StringBuilder h) => writeTools(h))

            //        }
            //    }, new LayoutSettings { Direction = LayoutDirection.Horizontal }, new { @class = "tools-container" });
            //}
            //else if (toolsAvailable == true)
            //{
            //    writeTools(htmlBuilder);
            //}
            //else if (filterAvailable == true)
            //{
            //    writeFilter(htmlBuilder);
            //}
        }

        public static MvcHtmlString RenderItemCountDescription(HtmlHelper helper, DataViewOptions options, DataViewState state)
        {
            var htmlBuilder = new StringBuilder(); // SHould be a text writer with iformatter set
            using (var htmlWriter = new StringWriter(htmlBuilder, CultureInfo.CurrentUICulture))
            {
                RenderItemCountDescription(helper, htmlWriter, options, state);
            }
            return MvcHtmlString.Create(htmlBuilder.ToString());
        }

        public static void RenderItemCountDescription(HtmlHelper helper, TextWriter writer, DataViewOptions options, DataViewState state)
        {
            var itemCountTagBuilder = new TagBuilder("div");
            itemCountTagBuilder.AddCssClass("item-count-container");
            itemCountTagBuilder.Write(writer, TagRenderMode.StartTag);
            var itemCountDescriptionHtml = options.ItemCountDescriptionHtml as HtmlContent;
            if (itemCountDescriptionHtml != null)
            {
                itemCountDescriptionHtml.WriteContentHtml(helper, writer);
            }
            else if (options.ItemCountDescriptionTemplate != null)
            {
                helper.RenderPartial(options.ItemCountDescriptionTemplate, writer, model: state);
            }
            else if (state.ItemCount.HasValue == true)
            {
                string format = options.ItemCountDescriptionFormat ?? "{0} hits";
                string displayText = string.Format(format, state.ItemCount.Value);
                writer.WriteConcat(displayText);
            }
            itemCountTagBuilder.Write(writer, TagRenderMode.EndTag);
        }

        private static ToolBarItem GetSortToolBarItem(DataViewOptions options, ApplicationRequestContext requestContext, DataViewState state = null)
        {
            if (options.DisplaySortSelector != false && options.SortSelectors != null)
            {
                var matchContext = new MatchContext(requestContext);
                var availableSortSelector = options.SortSelectors.Where(s => s.IsAvailable(null, matchContext) == true).OrderBy(s => s.Order).ToArray();

                if (availableSortSelector.Length > 0)
                {
                    //var siteContext = requestContext.SiteContext;
                    string selectedSortId = state != null ? state.SortId ?? options.DefaultSortSelector : options.DefaultSortSelector;
                    var sortAligment =ToolBarAlignment.Left /*?? options.SortAlignment*/;
                    var sortSelectorItem = !string.IsNullOrEmpty(selectedSortId) ? availableSortSelector.Where(p => StringComparer.OrdinalIgnoreCase.Equals(selectedSortId, p.Id) == true).FirstOrDefault() : null;
                    if (sortSelectorItem != null)
                    {
                        sortSelectorItem = (DataViewSortSelector)sortSelectorItem.Copy(true);
                        if (state != null && state.Sort != null)
                        {
                            sortSelectorItem.Direction = state.Sort.Direction;
                        }
                    }

                    var ascDecText = string.Empty /*sortSelectorItem != null && (int)sortSelectorItem.Direction == 0 ? ", ASC" : ", DEC"*/;
                    var selectedText = sortSelectorItem != null ? string.Format("Sort by" +" {0}{1}", requestContext.GetApplicationTextTranslation(sortSelectorItem.Label), ascDecText) : requestContext.GetApplicationTextTranslation("Sort");
                    //icon.GetHtml(htmlAttributes, classNames, size)

                    //var sortsToolBarItemIcon = siteContext.GetIcon(Icons.Sorting);
                    var sortsToolBarItem = new ToolBarItem
                    {
                        Label = requestContext.GetApplicationTextTranslation("Sort"),
                        Icon = requestContext.GetIcon(Icons.Sorting),
                        Content = new HtmlContent(MvcHtmlString.Create(string.Format("<div class=\"sort-selector-container\"><input size=\"{0}\" autocomplete=\"off\" disabled placeholder=\"{1}\" class=\"sort-text\"/><div class=\"sort-icon-container\"></div></div>", selectedText.Length, selectedText /*IconRendererProvider.GetRenderer(sortsToolBarItemIcon).GetHtml(sortsToolBarItemIcon, classNames: "sort-icon")*/))),
                        DropDownItems = availableSortSelector.Select(sortSelector =>
                        {
                            var sort = sortSelector.Sort;
                            var htmlAttributes = new Dictionary<string, object> { { "class", "sort-selector" } };
                            if (sortSelectorItem != null && sortSelector.Id == sortSelectorItem.Id)
                            {
                                htmlAttributes.Add("data-sort-dir", (int)sortSelectorItem.Direction);
                                if (sortSelectorItem.Type != null) htmlAttributes.Add("data-sort-type", sortSelectorItem.Type);
                            }
                            else if (sort != null)
                            {
                                htmlAttributes.Add("data-sort-dir", (int)sort.Direction);
                                if (sort.Type != null) htmlAttributes.Add("data-sort-type", sort.Type);
                                //htmlAttributes.Add("data-sort", JsonConvert.SerializeObject(sortSelector.Sort));
                            }
                            else
                            {

                                htmlAttributes.Add("data-sort-dir", 0);
                            }

                            var menuItem = new MenuItem
                            {
                                Name = sortSelector.Id,
                                //Icon = siteContext.GetIcon(sortSelector.Icon),// ?? new MultiIconDescriptor(siteContext.GetIcons(Icons.SortUp, Icons.SortDown)),
                                Label = requestContext.GetApplicationTextTranslation(sortSelector.Label),
                                Selected = selectedSortId != null ? StringComparer.OrdinalIgnoreCase.Equals(selectedSortId, sortSelector.Id) == true : state != null && state.Sort != null && state.Sort.Equals(sortSelector.Sort),
                                CheckType = CheckType.Radio,
                                CheckGroup = "sort",
                                HtmlAttributes = htmlAttributes
                            };
                            return menuItem;
                        }).ToArray(),

                        DropDown = new DropDown
                        {
                            ClassNames = "sort-selectors",
                            CloseOnClick = true,
                            AutoClose = false,
                            Top = 0,
                            Right = 0,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Direction = DropDownDirection.Down,

                        },
                        Alignment = sortAligment,
                        ClassNames = "sort-tool"
                    };

                    //if (options.DefaultSortSelector != null && sortsToolBarItem.DropDownItems.Any(i => i.Selected == true) == false)
                    //{
                    //    var selected = sortsToolBarItem.DropDownItems.Where(i => StringComparer.OrdinalIgnoreCase.Equals(options.DefaultSortSelector, i.Name) == true).FirstOrDefault();
                    //    if (selected != null)
                    //    {
                    //        selected.Selected = true;
                    //    }
                    //}

                    return sortsToolBarItem;
                }
            }
            return null;
        }

        public static MvcHtmlString RenderSortToolBarItem(HtmlHelper helper, DataViewOptions options, DataViewState state = null, ApplicationRequestContext requestContext = null)
        {
            var htmlBuilder = new StringBuilder();
            using (var writer = new StringWriter(htmlBuilder, CultureInfo.CurrentUICulture))
            {
                DataViewHelper.RenderSortToolBarItem(helper, writer, options, state, requestContext);
            }
            return MvcHtmlString.Create(htmlBuilder.ToString());
        }

        public static void RenderSortToolBarItem(HtmlHelper helper, TextWriter writer, DataViewOptions options, DataViewState state = null, ApplicationRequestContext requestContext = null)
        {
            if (requestContext == null)
            {
                requestContext = helper.ViewContext.GetApplicationRequestContext();
            }

            var sortToolBarItem = MultivendorWebViewer.Helpers.DataViewHelper.GetSortToolBarItem(options, requestContext, state);
            if (sortToolBarItem != null)
            {
                var settings = new ToolBarSettings { Direction = LayoutDirection.Horizontal, Highlight = true, Size = ToolBarSize.Medium, ContentDirection = LayoutDirection.Horizontal, ShowToolTip = false };
                var toolbarContext = new ToolBarRenderContext(helper, writer, settings, Enumerable.Empty<ToolBarItem>());
                sortToolBarItem.RenderHtml(toolbarContext);
            }
        }

        public static MvcHtmlString RenderPaginationTools(HtmlHelper helper, DataViewOptions options, DataViewState state = null, ApplicationRequestContext requestContext = null, bool renderOnlyItems = false)
        {
            if (options == null) options = new DataViewOptions();
            var htmlBuilder = new StringBuilder(); // SHould be a text writer with iformatter set
            using (var htmlWriter = new StringWriter(htmlBuilder, CultureInfo.CurrentUICulture))
            {
                RenderPaginationTools(helper, htmlWriter, options, state, requestContext, renderOnlyItems);
            }
            return MvcHtmlString.Create(htmlBuilder.ToString());
        }

        private static void RenderPaginationTools(HtmlHelper helper, TextWriter htmlWriter, DataViewOptions options, DataViewState state = null, ApplicationRequestContext requestContext = null, bool renderOnlyItems = false)
        {
            int itemCount = state != null ? state.ItemCount ?? -1 : -1;
            var pagination = state != null ? state.Pagination : null;

            int pageSize = (pagination != null ? pagination.PageSize : null) ?? options.DefaultPageSize ?? (options.PageSizes != null ? options.PageSizes.Min(p => p.Count) : 1);

            var pagingItems = new List<ToolBarItem>();

            if (itemCount >= 0)
            {
                //var siteContext = requestContext.SiteContext;

                //if (siteContext == null)
                //{
                //    siteContext = helper.ViewContext.GetApplicationRequestContext().SiteContext;
                //}

                int page = pagination != null ? pagination.PageIndex ?? 0 : 0;

                //var pages = options.PageSelectors;
                //if (pages == null)
                //{
                var pages = new List<PaginationPageSelector>();
                //}

                //PaginationPageSelector firstPageSelector = null, backPageSelector = null, nextPageSelector = null, lastPageSelector = null;

                int numberOfPages = itemCount / pageSize;
                if (numberOfPages * pageSize < itemCount) numberOfPages++;

                //if (numberOfPages >= 6)
                //{
                //    if (options.PageSelectorTypes.HasFlag(PaginationPageSelectorTypes.First) == true)
                //    {
                //        firstPageSelector = pages.FirstOrDefault(p => p.Type == PaginationPageSelectorTypes.First) ?? new PaginationPageSelector { Type = PaginationPageSelectorTypes.First, Label = "1", Index = 0 };
                //        pages.Add(firstPageSelector);
                //    }

                //    if (options.PageSelectorTypes.HasFlag(PaginationPageSelectorTypes.Previous) == true)
                //    {
                //        backPageSelector = pages.FirstOrDefault(p => p.Type == PaginationPageSelectorTypes.Previous) ?? new PaginationPageSelector { Type = PaginationPageSelectorTypes.Previous, Icon = siteContext.GetIcon(Icons.ChevronLeft), Label = ApplicationRequestContext.CustomStrings.Back, Index = page - 1 };
                //        pages.Add(backPageSelector);
                //    }

                //    if (options.PageSelectorTypes.HasFlag(PaginationPageSelectorTypes.Next) == true && pages.Any(p => p.Type == PaginationPageSelectorTypes.Next) == false)
                //    {
                //        pages.Add(new PaginationPageSelector { Type = PaginationPageSelectorTypes.Next, Icon = siteContext.GetIcon(Icons.ChevronRight), Label = "Next", Index = page + 1 });
                //    }

                //    if (options.PageSelectorTypes.HasFlag(PaginationPageSelectorTypes.Last) == true && pages.Any(p => p.Type == PaginationPageSelectorTypes.Last) == false)
                //    {
                //        pages.Add(new PaginationPageSelector { Type = PaginationPageSelectorTypes.Last, Label = "Last", Index = numberOfPages - 1 });
                //    }
                //}

                int maxIndexPageSelectors = options.MaxIndexPageSelectors ?? 6;

                if (maxIndexPageSelectors > 0 && numberOfPages > 1)
                {
                    if (numberOfPages < maxIndexPageSelectors)
                    {
                        AddIndexPageSelectors(requestContext, pagingItems, pages, 0, numberOfPages, options, page);
                        //for (int i = 0; i < numberOfPages; i++)
                        //{
                        //    var pageSelector = pages.FirstOrDefault(p => p.Index == i);
                        //    if (pageSelector == null && options.PageSelectorTypes.HasFlag(PaginationPageSelectorTypes.Index) == true)
                        //    {
                        //        pageSelector = new PaginationPageSelector { Type = PaginationPageSelectorTypes.Index, Index = i, Label = (i + 1).ToString() };
                        //    }
                        //    if (pageSelector != null)
                        //    {
                        //        pagingItems.Add(CreatePageSelectonToolbarItem(pageSelector, options, selected: i == page));
                        //    }
                        //}
                    }
                    else
                    {                  
                        // Backward, if not on first page
                        if (page > 0)
                        {
                            var backPageSelector = pages.FirstOrDefault(p => p.Type == PaginationPageSelectorTypes.Previous) ?? (options.PageSelectorTypes.HasFlag(PaginationPageSelectorTypes.Previous) == true ? new PaginationPageSelector { Type = PaginationPageSelectorTypes.Previous, Icon = Icons.ChevronLeft, /*Label = ApplicationRequestContext.CustomStrings.Back,*/ Index = page - 1 } : null);
                            if (backPageSelector != null) pagingItems.Add(CreatePageSelectonToolbarItem(requestContext, backPageSelector, options));
                            //var backPageSelector = pages.FirstOrDefault(p => p.Index == page - 1);
                            //if (backPageSelector != null) pagingItems.Add(CreatePageSelectonToolbarItem(backPageSelector, options));
                            //pagingItems.Add(CreatePageSelection(page - 1, GetIcon(Icons.ChevronLeft), className: "page-back"));
                        }

                        int lastPageIndex = numberOfPages - 1;

                        // If on page 5 or more, display paged +/- 2 from current page
                        if (page > (maxIndexPageSelectors - 2))
                        {
                            //pagingItems.Add(CreatePageSelection(0, selected: false));
                            // First page and ellipsis to first page
                            if (page > 0)
                            {
                                var firstPageSelector = pages.FirstOrDefault(p => p.Type == PaginationPageSelectorTypes.First) ?? (options.PageSelectorTypes.HasFlag(PaginationPageSelectorTypes.First) == true ? new PaginationPageSelector { Type = PaginationPageSelectorTypes.First, Label = "1", Index = 0 } : null);

                                if (firstPageSelector != null)
                                {
                                    pagingItems.Add(CreatePageSelectonToolbarItem(requestContext, firstPageSelector, options));

                                    var ellipsis = CreatePageSelection(-1 /*siteContext.GetIcon(Icons.Ellipsis)*/);
                                    ellipsis.Clickable = false;
                                    pagingItems.Add(ellipsis);
                                }
                            }

                            if (page < lastPageIndex - (maxIndexPageSelectors - 3))
                            {
                                int firstSelectablePage = Math.Max(0, page - 2);
                                int lastSelectablePage = Math.Min(numberOfPages - 1, firstSelectablePage + (maxIndexPageSelectors - 2));

                                AddIndexPageSelectors(requestContext, pagingItems, pages, firstSelectablePage, lastSelectablePage + 1, options, page);

                                //for (int i = firstSelectablePage; i <= lastSelectablePage; i++)
                                //{
                                //    pagingItems.Add(CreatePageSelection(i, selected: i == page));
                                //}
                            }
                            else
                            {
                                AddIndexPageSelectors(requestContext, pagingItems, pages, lastPageIndex - (maxIndexPageSelectors - 2), lastPageIndex + 1, options, page);
                                //for (int i = lastPageIndex - 4; i <= lastPageIndex; i++)
                                //{
                                //    pagingItems.Add(CreatePageSelection(i, selected: i == page));
                                //}
                            }
                        }
                        else
                        {
                            // If on page 4 or less, display page 1 - 5
                            AddIndexPageSelectors(requestContext, pagingItems, pages, 0, maxIndexPageSelectors, options, page);
                            //for (int i = 0; i <= 5; i++)
                            //{
                            //    pagingItems.Add(CreatePageSelection(i, selected: i == page));
                            //}
                        }


                        // last page and ellipsis to last page
                        if (page < lastPageIndex - (maxIndexPageSelectors - 3))
                        {
                            // Display ellipsis to last page
                            var lastPageSelector = pages.FirstOrDefault(p => p.Type == PaginationPageSelectorTypes.Last) ?? (options.PageSelectorTypes.HasFlag(PaginationPageSelectorTypes.Last) == true ? new PaginationPageSelector { Type = PaginationPageSelectorTypes.Last, Label = numberOfPages.ToString(), Index = numberOfPages - 1 } : null);

                            if (lastPageSelector != null)
                            {
                                var ellipsis = CreatePageSelection(-1, requestContext.GetIcon(Icons.Ellipsis));
                                ellipsis.Clickable = false;
                                pagingItems.Add(ellipsis);

                                pagingItems.Add(CreatePageSelectonToolbarItem(requestContext, lastPageSelector, options));
                            }
                            //pagingItems.Add(CreatePageSelection(lastPageIndex, selected: false));
                        }

                        // Forward if on not last page
                        if (page < lastPageIndex)
                        {
                            var nextPageSelector = pages.FirstOrDefault(p => p.Type == PaginationPageSelectorTypes.Next) ?? (options.PageSelectorTypes.HasFlag(PaginationPageSelectorTypes.Next) == true ? new PaginationPageSelector { Type = PaginationPageSelectorTypes.Next, Icon = Icons.ChevronRight/*, Label = "Next"*/, Index = page + 1 } : null);
                            if (nextPageSelector != null) pagingItems.Add(CreatePageSelectonToolbarItem(requestContext, nextPageSelector, options));
                            //pagingItems.Add(CreatePageSelection(page + 1, GetIcon(Icons.ChevronRight), className: "page-forward"));
                        }
                    }
                }
            }

            IList<PaginationPageSize> pageSizes = options.PageSizes;
            if (pageSizes != null)
            {
                if (itemCount != -1 && options.AutoHidePageSizes == true)
                {
                    pageSizes = new List<PaginationPageSize>();
                    foreach (var s in options.PageSizes)
                    {
                        pageSizes.Add(s);
                        if (s.Count >= itemCount)
                            break;
                    }

                    if (pageSizes.Count == 1) pageSizes.Clear();
                }

                if (pageSizes.Count > 0)
                {
                    //CustomStrings.All
                    for (int index = 0; index < pageSizes.Count; index++)
                    {
                        var pageSizeOption = pageSizes[index];
                        var pageSizeAttributes = new Dictionary<string, object> { { "data-page-size", pageSizeOption.Count } };
                        if (pageSizeOption.Id != null) pageSizeAttributes.Add("data-page-size-id", pageSizeOption.Id);
                        if (pageSizes.Count == 1) pageSizeAttributes.Add("class", "single");

                        pagingItems.Add(new ToolBarItem
                        {
                            //ClassNames = "page-size",
                            Label = pageSizeOption.Label ?? pageSizeOption.Count.ToString(),
                            Icon = requestContext.GetIcon(pageSizeOption.Icon),
                            Checked = pagination != null && pagination.PageSizeId != null ? StringComparer.OrdinalIgnoreCase.Equals(pagination.PageSizeId, pageSizeOption.Id) == true : pageSize == pageSizeOption.Count,
                            CheckType = CheckType.Radio,
                            CheckGroup = "size",
                            //Alignment = ToolBarAlignment.Right,
                            ShowTooltip = false,
                            HtmlAttributes = pageSizeAttributes
                        });
                    }
                }
            }

            if (pagingItems.Count > 0)
            {
                var settings = new ToolBarSettings { Direction = LayoutDirection.Horizontal, Highlight = true, Size = ToolBarSize.Medium, ContentDirection = LayoutDirection.Horizontal, ShowToolTip = false };

                if (renderOnlyItems == true)
                {
                    ToolBarHelper.RenderToolBarItems(helper, htmlWriter, pagingItems, settings);
                }
                else
                {
                    var attributes = new Dictionary<string, object>();
                    attributes.Add("class", "pagination-tools");
                    ToolBarHelper.ToolBar(helper, htmlWriter, pagingItems, settings, htmlAttributes: attributes);
                }
            }
        }

        private static void AddIndexPageSelectors(ApplicationRequestContext requestContext, List<ToolBarItem> pagingItems, IList<PaginationPageSelector> pages, int from, int to, DataViewOptions options, int page)
        {
            for (int i = from; i < to; i++)
            {
                var pageSelector = pages.FirstOrDefault(p => p.Index == i);
                if (pageSelector == null && options.PageSelectorTypes.HasFlag(PaginationPageSelectorTypes.Index) == true)
                {
                    pageSelector = new PaginationPageSelector { Type = PaginationPageSelectorTypes.Index, Index = i, Label = (i + 1).ToString() };
                }
                if (pageSelector != null)
                {
                    pagingItems.Add(CreatePageSelectonToolbarItem(requestContext, pageSelector, options, selected: i == page));
                }
            }
        }

        private static ToolBarItem CreatePageSelectonToolbarItem(ApplicationRequestContext requestContext, PaginationPageSelector pageSelector, DataViewOptions options, bool? selected = null, string className = null)
        {
            var htmlAttributes = pageSelector.Index >= 0 ? new Dictionary<string, object> { { "data-page", pageSelector.Index } } : null;
            if (pageSelector.Id != null) htmlAttributes.Add("data-page-id", pageSelector.Id);
            if (pageSelector.Url != null) htmlAttributes.Add("data-page-url", pageSelector.Url);
            if (className != null)
            {
                htmlAttributes["class"] = className;
            }

            return new ToolBarItem
            {
                //ClassNames = "page-selection",
                Label = pageSelector.Label,
                Icon = requestContext.GetIcon(pageSelector.Icon),
                Checked = selected,
                CheckType = CheckType.Radio,
                CheckGroup = "page",          
                Alignment = ToolBarAlignment.Left,
                HtmlAttributes = htmlAttributes
            };
        }

        private static ToolBarItem CreatePageSelection(int page, IconDescriptor icon = null, bool? selected = null, string className = null)
        {
            var htmlAttributes = page >= 0 ? new Dictionary<string, object> { { "data-page", page } } : null;
            if (className != null)
            {
                htmlAttributes["class"] = className;
            }

            return new ToolBarItem
            {
                Label = icon == null ? (page + 1).ToString() : null,
                Icon = icon,
                Checked = selected,
                CheckType = selected.HasValue == true ? CheckType.Radio : CheckType.None,
                CheckGroup = selected.HasValue == true ? "page" : null,
                Alignment = ToolBarAlignment.Left,
                HtmlAttributes = htmlAttributes
            };
        }

        public static MvcHtmlString RenderFacets(HtmlHelper helper, DataViewOptions options, DataViewState state = null, ApplicationRequestContext requestContext = null)
        {
            if (options == null) options = new DataViewOptions();
            var htmlBuilder = new StringBuilder(); // SHould be a text writer with iformatter set
            using (var htmlWriter = new StringWriter(htmlBuilder, CultureInfo.CurrentUICulture))
            {
                RenderFacets(helper, htmlWriter, options, state, requestContext);
            }
            return MvcHtmlString.Create(htmlBuilder.ToString());
        }

        private static void RenderFacets(HtmlHelper htmlHelper, TextWriter writer, DataViewOptions options, DataViewState state = null, ApplicationRequestContext requestContext = null)
        {
            if (requestContext == null) htmlHelper.GetApplicationRequestContext();

            //foreach (var facet in options.FacetSettings) // TODO THIS IS TEMPORARY. VIEW MODELS SHOULD BE CREATED OUT OF FACETS (WITH SETTINGS). NOT THE SETTINGS
            //{
            //    var facetTagBuilder = new TagBuilder("section");
            //    facetTagBuilder.AddCssClass("facet");
            //    facetTagBuilder.MergeAttribute("data-id", facet.Id);

            //    facetTagBuilder.Write(writer, TagRenderMode.StartTag);


            //    var inputTagFrom = new TagBuilder("input");
            //    inputTagFrom.MergeAttribute("data-value-name", "from");
            //    inputTagFrom.Write(writer);

            //    var inputTagTo = new TagBuilder("input");
            //    inputTagTo.MergeAttribute("data-value-name", "to");
            //    inputTagTo.Write(writer);

            //    facetTagBuilder.Write(writer, TagRenderMode.EndTag);
            //}
        }

    }

    //public class DataViewFilterFacetSelection
    //{
    //    public DataViewFilterFacetSelection()
    //    {
    //        Enabled = true;
    //        Display = true;
    //        Properties = new List<DataViewFilterSelectionProperty>();
    //    }

    //    private string id;
    //    [JsonProperty("id")]
    //    public string Id { get { return id ?? (id = Guid.NewGuid().ToString()); } set { id = value; } }

    //    [JsonProperty("enabled")]
    //    public bool Enabled { get; set; }

    //    [JsonProperty("display")]
    //    public bool Display { get; set; }

    //    [JsonProperty("properties")]
    //    public List<DataViewFilterSelectionProperty> Properties { get; set; }

    //    public DataViewFilterSelectionProperty GetProperty(string id)
    //    {
    //        return Properties.Where(p => p.Id == id).FirstOrDefault();
    //    }
    //}

  
   
    public class PaginationIcons
    {
        public IconDescriptor Previous { get; set; }

        public IconDescriptor Next { get; set; }

        public IconDescriptor First { get; set; }

        public IconDescriptor Last { get; set; }

        public IconDescriptor Ellipsis { get; set; }
    }

    //public class DataViewModel<TViewModel, TItem, TFilter>
    //{
    //    public DataViewModel(TViewModel model, IEnumerable<TItem> items)
    //    {
    //        Model = model;
    //        this.items = items;
    //    }

    //    public TViewModel Model { get; set; }

    //    private IEnumerable<TItem> items;

    //    public Sort Sort { get; set; }

    //    public int PageIndex { get; set; }

    //    public int? PageSize { get; set; }

    //    public Func<TItem, bool> Filter { get; set; }

    //    public IList<string> FilterProperties { get; set; }

    //    public IEnumerable<TItem> DisplayItems
    //    {
    //        get
    //        {
    //            var displayItems = items;
    //            if (Sort != null)
    //            {
    //                displayItems = ApplySort(displayItems, Sort);
    //            }

    //            if (PageSize.HasValue == true)
    //            {
    //                displayItems = ApplyPagination(displayItems, PageIndex, PageSize.Value);
    //            }

    //            return displayItems;
    //        }
    //    }

    //    protected virtual IEnumerable<TItem> ApplyFilter(IEnumerable<TItem> items)
    //    {
    //        var getters = PropertyGetter.GetPropertyGetters(typeof(TItem));
    //        return items;
    //    }

    //    protected virtual IEnumerable<TItem> ApplySort(IEnumerable<TItem> items, Sort sort)
    //    {
    //        var getter = PropertyGetter.Create(sort.Type, typeof(TItem));
    //        if (getter != null)
    //        {
    //            return items.OrderByDirection(i => getter.GetValue(i), sort.Direction);
    //        }
    //        return items;
    //    }

    //    protected virtual IEnumerable<TItem> ApplyPagination(IEnumerable<TItem> items, int pageIndex, int pageSize)
    //    {
    //        return items.Skip(pageIndex * pageSize).Take(pageSize);
    //    }
    //}

    public abstract class ItemFilterer<TItem, TFilter>
    {
        public ApplicationRequestContext RequestContext { get; set; }

        public abstract IEnumerable<TItem> Filter(IEnumerable<TItem> items, TFilter filter);

        public abstract bool IsFilterActive(TFilter filter);
    }

    public class ItemQueryFilterer<TItem> : ItemFilterer<TItem, IDataViewQueryFilterSelection>
    {
        public ItemQueryFilterer(ApplicationRequestContext requestContext, CultureInfo stringCompareCulture = null, StringCompareMode stringCompareMode = StringCompareMode.Contains, bool ignoreCase = true)
        {
            RequestContext = requestContext;
            //StringSplitTokens = DefaultStringSplitTokens;
            StringCompareCulture = stringCompareCulture ??new CultureInfo( requestContext.SelectedCulture);
            StringCompareInfo = stringCompareCulture.CompareInfo;
            StringCompareOptions = ignoreCase == true ? CompareOptions.IgnoreCase : CompareOptions.None;
            StringFormatProvider = StringCompareCulture;
            StringCompareMode = stringCompareMode;
            StringMatch = GetStringMatch();
        }

        public char[] StringSplitTokens { get; set; }

        //protected virtual char[] DefaultStringSplitTokens { get { return new[] { ' ', ',', '.', ';', '-', '/', '\t' }; } }

        public IFormatProvider StringFormatProvider { get; set; }

        public CultureInfo StringCompareCulture { get; private set; }

        protected CompareInfo StringCompareInfo { get; private set; }

        protected CompareOptions StringCompareOptions { get; set; }

        public StringCompareMode StringCompareMode { get; private set; }

        protected Func<string, string, bool> StringMatch { get; private set; }

        public IEqualityComparer<TItem> DistinctComparer { get; set; }

        public class ItemFilterContext
        {
            public FormatterContext FormatterContext { get; set; }

            public IFormatProvider FormatProvider { get; set; }

            public SearchQuery Query { get; set; }

            public IEnumerable<TItem> Items { get; set; }

            //private IEnumerable<string> filterProperties;
            //public IEnumerable<string> FilterProperties
            //{
            //    get { return filterProperties; }
            //    set
            //    {
            //        filterProperties = value;
            //        filterPropertyGetters = null;
            //    }
            //}

            private IEnumerable<DataViewFilterSelectionProperty> filters;
            public IEnumerable<DataViewFilterSelectionProperty> Filters
            {
                get { return filters; }
                set
                {
                    filters = value;
                    //filterPropertyGetters = null;
                    valueFunctions = null;
                }
            }

            //private IEnumerable<DataViewFilterSelector> filters;
            //public IEnumerable<DataViewFilterSelector> Filters
            //{
            //    get { return filters; }
            //    set
            //    {
            //        filters = value;
            //        filterPropertyGetters = null;
            //        valueFunctions = null;
            //    }
            //}

            private IList<Func<TItem, string>> valueFunctions;
            public IList<Func<TItem, string>> ValueFunctions
            {
                get
                {
                    if (valueFunctions == null)
                    {
                        valueFunctions = new List<Func<TItem, string>>();
                        if (Filters != null)
                        {

                            var itemType = typeof(TItem);
                            foreach (var filter in Filters)
                            {
                                if (filter.Property != null && filter.ExplicitValueProvider == false)
                                {
                                    var propertyGetter = PropertyGetter.Create(filter.Property, itemType);
                                    var formatter = filter.ValueProvider is ValueProvider ? ((ValueProvider)filter.ValueProvider).Format : null;
                                    if (propertyGetter != null)
                                    {
                                        valueFunctions.Add(i =>
                                        {
                                            object value = propertyGetter.GetValue(i);
                                            return value != null ? ValueProvider.GetFormattedValue(value, formatter, formatterContext: FormatterContext, formatProvider: FormatProvider, multipleValuesSeparator: " ") : null;
                                        });
                                    }
                                }
                                else if (filter.ValueProvider != null)
                                {
                                    valueFunctions.Add(i => filter.ValueProvider.GetFormattedValue(i, " ", FormatterContext, FormatProvider));
                                }
                            }
                            //valueFunctions = Filters.Where(f => f.Property != null).Select(f => PropertyGetter.Create(f.Property, itemType)).Where(p => p != null).ToArray();
                        }
                    }
                    return valueFunctions;
                }
            }

            //private IList<IPropertyGetter> filterPropertyGetters;
            //public IList<IPropertyGetter> FilterPropertyGetters
            //{
            //    get
            //    {
            //        if (filterPropertyGetters == null)
            //        {
            //            if (Filters != null)
            //            {
            //                var itemType = typeof(TItem);
            //                filterPropertyGetters = Filters.Where(f => f.Property != null).Select(f => PropertyGetter.Create(f.Property, itemType)).Where(p => p != null).ToArray();
            //            }
            //            else
            //            {
            //                filterPropertyGetters = new IPropertyGetter[0];
            //            }
            //        }
            //        return filterPropertyGetters;
            //    }
            //}
        }

        //public ILookup<string, Formatter> Formatters { get; set; }

        //public ILookup<string, Formatter> QueryFormatters { get; set; }

        public ICollection<Formatter> QueryFormatters { get; set; }

        public override IEnumerable<TItem> Filter(IEnumerable<TItem> items, IDataViewQueryFilterSelection filter)
        {
            if (filter == null || filter.Query == null || filter.Properties == null || filter.Properties.Count == 0) return items;

            var distinctComparer = DistinctComparer ?? EqualityComparer<TItem>.Default;
            var searchIndexLookup = new Dictionary<string, IEnumerable<TItem>>();

            string queryString = filter.Query;

            if (QueryFormatters != null && QueryFormatters.Count > 0)
            {
                foreach (var queryFormatter in QueryFormatters)
                {
                    queryString = queryFormatter.GetFormattedValue(queryString, formatProvider: StringFormatProvider);
                }
            }
            
            var query = SearchQuery.Create(queryString);

            var context = new ItemFilterContext { FormatProvider = StringFormatProvider, Items = items.ToArray(), Query = query, Filters = filter.Properties /*FilterProperties = filter.Properties.Select(p => p.Property ?? p.Id)*/ };
            if (RequestContext != null)
            {
                context.FormatterContext = new FormatterContext(RequestContext);
                if (context.FormatProvider == null) context.FormatProvider =new CultureInfo( RequestContext.SelectedCulture);
            }

            var filteredItems = query.GetHits(q =>
            {
                var tq = q.Trim().Trim('"');
                if (string.IsNullOrWhiteSpace(tq) == false)
                {
                    string[] words = GetFilterWords(tq);
                    if (words.Length > 0)
                    {
                        string word = words[0];
                        IEnumerable<TItem> allHits = null;

                        // Search in index
                        IEnumerable<TItem> searchIndexHits;
                        if (searchIndexLookup.TryGetValue(word, out searchIndexHits) == false)
                        {
                            searchIndexHits = Filter(word, context);
                            searchIndexLookup[word] = searchIndexHits;
                        }

                        allHits = searchIndexHits;

                        if (words.Length == 1)
                        {
                            return allHits;
                        }
                        else
                        {
                            var combinedHits = new HashSet<TItem>(allHits, distinctComparer);

                            for (int i = 1; i < words.Length; i++)
                            {
                                word = words[i];

                                // Search in index
                                if (searchIndexLookup.TryGetValue(word, out searchIndexHits) == false)
                                {
                                    searchIndexHits = Filter(word, context);
                                    searchIndexLookup[word] = searchIndexHits;
                                }

                                combinedHits.IntersectWith(searchIndexHits);
                            }

                            return combinedHits;
                        }
                    }
                }
                return Enumerable.Empty<TItem>();
            }, distinctComparer);

            return filteredItems;
        }

        public override bool IsFilterActive(IDataViewQueryFilterSelection filter)
        {
            return filter != null && string.IsNullOrWhiteSpace(filter.Query) == false;
        }

        protected virtual Func<string, string, bool> GetStringMatch()
        {
            switch (StringCompareMode)
            {
                case StringCompareMode.Contains:
                    return (s, w) => StringCompareInfo.IndexOf(s, w, StringCompareOptions) >= 0;
                case StringCompareMode.Equals:
                    return (s, w) => StringCompareInfo.Compare(s, w, StringCompareOptions) == 0;
                //return (s, w) => StringCompareInfo.IndexOf(s, w, StringCompareOptions) == 0 && s.Length == w.Length;
                case StringCompareMode.StartsWith:
                    return (s, w) => StringCompareInfo.IsPrefix(s, w, StringCompareOptions);
                case StringCompareMode.EndsWith:
                    return (s, w) => StringCompareInfo.IsSuffix(s, w, StringCompareOptions) == true;
            }
            return (s, w) => false;
        }

        protected virtual IEnumerable<TItem> Filter(string word, ItemFilterContext context)
        {
            //var getters = context.FilterPropertyGetters;
            var getters = context.ValueFunctions;
            var items = context.Items;
            foreach (var item in items)
            {
                //FormatterContext formatterContext = null;
                bool hit = false;
                //var itemViewModel = item as IViewModel;
                //if (itemViewModel != null)
                //{
                //    formatterContext = new FormatterContext(itemViewModel.ApplicationRequestContext);
                //}

                foreach (var getter in getters)
                {
                    string formattedValue = getter(item);
                    if (formattedValue != null)
                    {
                        if (StringMatch(formattedValue, word) == true)
                        {
                            hit = true;
                            break;
                        }
                    }
                    //object value = getter.GetValue(item);
                    //if (value != null)
                    //{
                    //    bool formattedApplied = false;

                    //    if (Formatters != null)
                    //    {
                    //        var formatters = Formatters[getter.PropertyNamePath];
                    //        foreach (var formatter in formatters)
                    //        {
                    //            formattedApplied = true;
                    //            var formattedValueString = formatter.GetFormattedValue(value, context: formatterContext, formatProvider: StringFormatProvider);
                    //            if (formattedValueString != null)
                    //            {
                    //                if (StringMatch(formattedValueString, word) == true)
                    //                {
                    //                    hit = true;
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }

                    //    if (formattedApplied == false)
                    //    {
                    //        //string valueString = Formatter.ToFormattedString(value, StringFormatProvider);
                    //        string valueString = ValueProvider.GetFormattedValue(value, formatterContext: formatterContext, formatProvider: StringFormatProvider, multipleValuesSeparator: " ");
                    //        if (valueString != null)
                    //        {
                    //            if (StringMatch(valueString, word) == true)
                    //            {
                    //                hit = true;
                    //                break;
                    //            }
                    //        }
                    //    }

                    //    if (hit == true) break;
                    //}
                }

                if (hit == true)
                {
                    yield return item;
                }
            }
        }
        protected virtual string[] GetFilterWords(string word)
        {
            return StringSplitTokens != null ? word.Split(StringSplitTokens, StringSplitOptions.RemoveEmptyEntries).ToArray() : new[] { word };
        }

    }

    public class ItemSorter<TItem>
    {
        public virtual IEnumerable<TItem> Sort(IEnumerable<TItem> items, Sort sort)
        {
            if (sort == null) return items;

            if (sort.ValueProvider != null)
            {
                var valueProvider = sort.ValueProvider;
                var orderedItems = items as IOrderedEnumerable<TItem>;
                if (orderedItems != null)
                    return orderedItems.ThenBy(i => valueProvider.GetValue(i), sort.Direction, sort.Comparer ?? new CollectionSafeComparerWrapper());
                return items.OrderByDirection(i => valueProvider.GetValue(i), sort.Direction, sort.Comparer ?? new CollectionSafeComparerWrapper());
            }

            if (string.IsNullOrEmpty(sort.Type) == true)
            {
                return sort.Direction == SortDirection.Ascending ? items : items.Reverse();
            }

            string[] types = sort.Type.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (types.Length > 0)
            {
                for (int index = 0; index < types.Length; index++)
                {
                    string trimmedType = types[index].Trim();
                    if (string.IsNullOrEmpty(trimmedType) == false)
                    {
                        var getter = PropertyGetter.Create(trimmedType, typeof(TItem));
                        if (getter != null)
                        {
                            var orderedItems = items as IOrderedEnumerable<TItem>;
                            IComparer<object> comparer = null;
                            if (sort.Comparer != null)
                            {
                                comparer = sort.Comparer;
                            }
                            else
                            {
                                comparer = GetComparer(getter.PropertyType);
                            }
                            if (orderedItems != null)
                            {
                                items = orderedItems.ThenBy(i => getter.GetValue(i), sort.Direction, comparer);
                            }
                            else
                            {
                                items = items.OrderByDirection(i => getter.GetValue(i), sort.Direction, comparer);
                            }
                        }
                    }
                }
            }

            return items;
        }


        private static Dictionary<Type, IComparer> defaultComparers = new Dictionary<Type, IComparer>
        {
            { typeof(sbyte), Comparer<sbyte>.Default },
            { typeof(sbyte?), Comparer<sbyte?>.Default },
            { typeof(byte), Comparer<byte>.Default },
            { typeof(byte?), Comparer<byte?>.Default },
            { typeof(short), Comparer<short>.Default },
            { typeof(short?), Comparer<short?>.Default },
            { typeof(ushort), Comparer<ushort>.Default },
            { typeof(ushort?), Comparer<ushort?>.Default },
            { typeof(int), Comparer<int>.Default },
            { typeof(int?), Comparer<int?>.Default },
            { typeof(uint), Comparer<uint>.Default },
            { typeof(uint?), Comparer<uint?>.Default },
            { typeof(long), Comparer<long>.Default },
            { typeof(long?), Comparer<long?>.Default },
            { typeof(ulong), Comparer<ulong>.Default },
            { typeof(ulong?), Comparer<ulong?>.Default },
            { typeof(float), Comparer<float>.Default },
            { typeof(float?), Comparer<float?>.Default },
            { typeof(double), Comparer<double>.Default },
            { typeof(double?), Comparer<double?>.Default },
            { typeof(decimal), Comparer<decimal>.Default },
            { typeof(decimal?), Comparer<decimal?>.Default },
        };

        protected IComparer<object> GetComparer(Type type)
        {
            IComparer result;
            if (defaultComparers.TryGetValue(type, out result) == true)
            {
                return new ComparerWrapper() { Comparer = result };
            }

            //return (IComparer<object>)Instance.Create(typeof(CollectionSafeComparerWrapper), null);

            var comparerGenericType = typeof(Comparer<>);
            var comparerType = comparerGenericType.MakeGenericType(type);
            var defaultProperty = comparerType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var comparerInstance = defaultProperty.GetValue(null);
            var comparer = comparerInstance as IComparer;

            return new CollectionSafeComparerWrapper(comparer);
        }

        public class ComparerWrapper : IComparer<object>
        {
            public IComparer Comparer { get; set; }

            public int Compare(object x, object y)
            {
                return Comparer.Compare(x, y);
            }
        }
    }

    public class ItemStringSorter<TItem> : ItemSorter<TItem>
    {
        public ItemStringSorter(CultureInfo stringCompareCulture, bool ignoreCase = true)
        {
            StringCompareCulture = stringCompareCulture;
            StringCompareInfo = stringCompareCulture.CompareInfo;
            StringCompareOptions = ignoreCase == true ? CompareOptions.IgnoreCase : CompareOptions.None;
            StringFormatProvider = StringCompareCulture;
        }

        public ItemStringSorter(IComparer<string> stringComparer, IFormatProvider stringFormatProvider)
        {
            StringComparer = stringComparer;
            StringFormatProvider = stringFormatProvider;
        }

        public IFormatProvider StringFormatProvider { get; set; }

        public Formatter StringFormatter { get; set; }

        public CultureInfo StringCompareCulture { get; private set; }

        protected CompareInfo StringCompareInfo { get; set; }

        protected CompareOptions StringCompareOptions { get; set; }

        public IComparer<string> StringComparer { get; set; }

        protected IComparer<string> GetDefaultStringComparer()
        {
            //return new GenericObjectComparer
            return new StringComp() { StringCompareInfo = StringCompareInfo, StringCompareOptions = StringCompareOptions };
        }

        public class StringComp : IComparer<string>
        {
            public CompareInfo StringCompareInfo { get; set; }

            public CompareOptions StringCompareOptions { get; set; }

            public int Compare(string x, string y)
            {
                return StringCompareInfo.Compare(x, y, StringCompareOptions);
            }
        }

        public override IEnumerable<TItem> Sort(IEnumerable<TItem> items, Sort sort)
        {
            if (sort == null) return items;

            if (sort.ValueProvider != null)
            {
                var valueProvider = sort.ValueProvider;

                var orderedItems = items as IOrderedEnumerable<TItem>;
                if (orderedItems != null)
                    return orderedItems.ThenBy(i => valueProvider.GetValue(i), sort.Direction, sort.Comparer ?? new CollectionSafeComparerWrapper());
                return items.OrderByDirection(i => valueProvider.GetValue(i), sort.Direction, sort.Comparer ?? new CollectionSafeComparerWrapper());

                //var formatProvider = StringFormatProvider ?? StringCompareCulture;
                //var stringComparer = StringComparer ?? GetDefaultStringComparer();
                //if (orderedItems != null) orderedItems.ThenBy(i => valueProvider.GetFormattedValue(i, " ", formatProvider: formatProvider), sort.Direction, stringComparer);
                //return items.OrderByDirection(i => valueProvider.GetFormattedValue(i, " ", formatProvider: formatProvider), sort.Direction, stringComparer);
            }

            if (string.IsNullOrEmpty(sort.Type) == true)
            {
                return sort.Direction == SortDirection.Ascending ? items : items.Reverse();
            }

            string[] types = sort.Type.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string type in types)
            {
                string trimmedType = type.Trim();
                if (string.IsNullOrEmpty(trimmedType) == false)
                {
                    var getter = PropertyGetter.Create(trimmedType, typeof(TItem));
                    if (getter != null)
                    {
                        var orderedItems = items as IOrderedEnumerable<TItem>;
                        if (getter.PropertyType == typeof(string)) // Handle string specially
                        {
                            var formatProvider = StringFormatProvider ?? StringCompareCulture;
                            var stringComparer = StringComparer ?? GetDefaultStringComparer();
                            if (orderedItems != null)
                            {
                                items = orderedItems.ThenBy(i => ValueProvider.GetFormattedValue(getter.GetValue(i), StringFormatter, formatProvider: formatProvider, multipleValuesSeparator: " "), sort.Direction, sort.Comparer ?? stringComparer);
                            }
                            else
                            {
                                items = items.OrderByDirection(i => ValueProvider.GetFormattedValue(getter.GetValue(i), StringFormatter, formatProvider: formatProvider, multipleValuesSeparator: " "), sort.Direction, sort.Comparer ?? stringComparer);
                            }                         
                        }
                        else
                        {
                            var comparer = sort.Comparer ?? GetComparer(getter.PropertyType);
                            // What about falling back to stringcomparision when no comparer found?
                            //IComparer<object> objComparer = comparer != null ? new ComparerWrapper() { Comparer = comparer } : (IComparer<object>)Comparer<object>.Default;

                            if (orderedItems != null)
                            {
                                items = orderedItems.OrderByDirection(i => getter.GetValue(i), sort.Direction, comparer);
                            }
                            else
                            {
                                items = items.OrderByDirection(i => getter.GetValue(i), sort.Direction, comparer);
                            }
                        }

                    }
                }
            }

            return items;
        }
    }

    public class TableReportColumn<T> : TableReportColumn
    {
        public Func<T, string> FormattedValueProvider { get; set; }

        public override Func<object, string> FormattedValueProviderWrapper
        {
            get
            {
                if (FormattedValueProvider != null)
                {
                    return i => FormattedValueProvider((T)i);
                }
                return null;
            }
        }
    }

    public class TableReportRenderer
    {
        public IEnumerable<TableReportColumn> ReportColumns { get; set; }

        public virtual IEnumerable<string[]> CreateReport(ApplicationRequestContext requestContext, IEnumerable<object> items, bool includeHeader = true, IFormatProvider formatProvider = null, string nullValue = null, string multiValueSeparator = ", ")
        {
            if (ReportColumns == null) yield break;

            var reportColumnsList = ReportColumns.ToList();

            //var itemType = typeof(TItem);
            //var propertyGetters = reportColumnsList.Select(c => PropertyGetter.Create(c.PropertyName, itemType)).ToArray();
            var propertyGettersLookup = new Dictionary<Type, IPropertyGetter[]>();
            int columnCount = reportColumnsList.Count;

            string[] headerValues = reportColumnsList.Select(c => requestContext.GetApplicationTextTranslation(c.Header) ?? nullValue).ToArray();
            yield return headerValues;

            var formatterContext = new FormatterContext(requestContext);
            foreach (var item in items)
            {
                string[] itemValues = new string[columnCount];
                var itemType = item.GetType();
                IPropertyGetter[] propertyGetters;
                if (propertyGettersLookup.TryGetValue(itemType, out propertyGetters) == false)
                {
                    propertyGetters = reportColumnsList.Select(c => PropertyGetter.Create(c.PropertyName, itemType)).ToArray();
                    propertyGettersLookup.Add(itemType, propertyGetters);
                }

                for (int index = 0; index < columnCount; index++)
                {
                    var column = reportColumnsList[index];
                    if (column.ValueProvider != null)
                    {
                        string formattedValue = column.ValueProvider.GetFormattedValue(item, multiValueSeparator, formatterContext, formatProvider);
                        itemValues[index] = formattedValue ?? nullValue;
                    }
                    else if (column.FormattedValueProviderWrapper == null)
                    {
                        var propertyGetter = propertyGetters[index];
                        if (propertyGetter != null)
                        {
                            object value = propertyGetter.GetValue(item);
                            string formattedValue = column.ValueProvider.GetFormattedValue(value, multiValueSeparator, column.Formatter, formatProvider);
                            itemValues[index] = formattedValue ?? nullValue;
                        }
                        else
                        {
                            itemValues[index] = nullValue;
                        }
                    }
                    else
                    {
                        itemValues[index] = column.FormattedValueProviderWrapper(item) ?? nullValue;
                    }
                }
                yield return itemValues;
            }
        }
    }

}