
using Newtonsoft.Json;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Controllers;
using MultivendorWebViewer.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using System.Web.Mvc.Html;
using MultivendorWebViewer.Components;
using MultivendorWebViewer.Configuration;
using System.Globalization;

namespace MultivendorWebViewer.Helpers
{
    public class DataViewResult : ActionResult
    {
        public DataViewResult(DataViewRequest request, ApplicationRequestContext requestContext)
        {
            Request = request;
            RequestContext = requestContext;
        }

        public ApplicationRequestContext RequestContext { get; private set; }

        public DataViewRequest Request { get; private set; }

        public DataViewOptions DataViewOptions { get; set; }

        public int? ItemCount { get; set; }

        public string ContentHtml { get; set; }

        public Func<MultivendorWebViewer.Components.DataViewState, DataViewContentResult> Content { get; set; }

        //public Func<PaginationResultContext, string> PaginationHtmlProvider { get; set; }

        public string PaginationHtml { get; set; }


        public string ToolsHtml { get; set; }

        public Func<DataViewContentResult, IEnumerable<TableReportColumn>> ReportColumnsProvider { get; set; }

        public Func<string> ReportFileNameProvider { get; set; }

        public Func<DataViewContentResult, IEnumerable<DataViewSortSelector>> SortSelectorProvider { get; set; }

#if NET5
        public override void ExecuteResult(ActionContext context)
#else
        public override void ExecuteResult(ControllerContext context)
#endif
        {
            var timer = Stopwatch.StartNew();
            long totalTime = 0, downloadTime = -1, contentTime = -1, contentRenderTime = -1, facetsRenderTime = -1;

            //var paginationResultContext = new PaginationResultContext(context, Request);

            var state = Request.State ?? new DataViewState();

            var options = DataViewOptions ?? state.CreateOptions();

            if (ItemCount.HasValue == true)
            {
                state.ItemCount = ItemCount;
            }

            // Set default pagination if needed
            if (options.PaginationDefined == true)
            {
                if (state.Pagination == null) state.Pagination = new PaginationSelection();
                if (state.Pagination.PageIndex.HasValue == false) state.Pagination.PageIndex = 0;
                if (state.Pagination.PageSize.HasValue == false) state.Pagination.PageSize = options.DefaultPageSize ?? options.PageSizes.Select(p => p.Count).FirstOrDefault();
            }

            bool updateSort = Request.UpdateSort == true;

            // Set sort
            if (options.SortDefined == true)
            {
                bool definedSortSelected = false;
                var matchContext = new MatchContext(RequestContext);
                if (state.SortId == null) state.SortId = options.DefaultSortSelector;
                if (state.SortId != null)
                {
                    var definedSort = options.SortSelectors.Where(s => s.IsAvailable(null, matchContext) == true && StringComparer.OrdinalIgnoreCase.Equals(s.Id, state.SortId)).Select(s => s.Sort).FirstOrDefault();
                    if (definedSort != null)
                    {
                        var definedSortCopy = definedSort.Copy();
                        if (state.Sort != null)
                        {
                            definedSortCopy.Direction = state.Sort.Direction;
                        }
                        definedSortSelected = true;
                        state.Sort = definedSortCopy;
                    }
                }

                // Selected sort selector is not valid, try set default
                if (definedSortSelected == false && options.DefaultSortSelector != null && options.SortSelectors.Any(s => s.IsAvailable(null, matchContext) == true && StringComparer.OrdinalIgnoreCase.Equals(s.Id, state.SortId) == true) == false)
                {
                    var fallbackSortSelector = options.SortSelectors.Where(s => s.IsAvailable(null, matchContext) == true && StringComparer.OrdinalIgnoreCase.Equals(s.Id, options.DefaultSortSelector)).FirstOrDefault();
                    if (fallbackSortSelector != null)
                    {
                        state.Sort = fallbackSortSelector.Sort;
                        state.SortId = fallbackSortSelector.Id;
                    }
                    else
                    {
                        state.Sort = null;
                        state.SortId = null;
                    }
                    updateSort = true;
                }
            }

            // Recalc page if out of bounds
            if (state.Pagination != null && state.ItemCount.HasValue == true && (state.Pagination.PageIndex ?? 0) * (state.Pagination.PageSize ?? 0) >= state.ItemCount.Value)
            {
                state.Pagination.PageIndex = null;
            }

            // If we have filter selector defined in the options and we have filters for the state, try to take the property path from the options instead of blindly trusting the state (that comes from the browser, a security list). TODO! We should cryp.
            if (state.Filter != null && state.Filter.Properties != null)
            {
                if (options.FilterSelectors == null)
                {
                    options.FilterSelectors = new DataViewSelectorCollection<DataViewFilterSelector>();
                }

                var map = options.FilterSelectors.ToDictionaryOptimized(f => f.Id, StringComparer.OrdinalIgnoreCase);

                foreach (var property in state.Filter.Properties)
                {
                    DataViewFilterSelector filter;
                    if (map.TryGetValue(property.Id, out filter) == true)
                    {
                        property.Property = filter.Property;
                        property.ValueProvider = filter.ValueProvider;
                        property.ExplicitValueProvider = filter.ExplicitValueProvider;
                    }
                    else
                    {
                        property.ValueProvider = ValueProvider.CreateProperty(property.Property);
                        //property.Skip = true;
                        //options.FilterSelectors.Add(new DataViewFilterSelector { Id = property.Id, ValueProvider = ValueProvider.CreateProperty(property.Property) });
                    }
                }
            }


            if (Request.DownloadReport == true)
            {
                if (ReportColumnsProvider != null && Content != null)
                {
                    var contentResult = Content(state);
                    if (contentResult.DataView != null)
                    {
                        var reportRenderer = new TableReportRenderer { ReportColumns = ReportColumnsProvider(contentResult) };
                        var table = reportRenderer.CreateReport(RequestContext, contentResult.DataView.DisplayedItems, formatProvider: new CultureInfo(RequestContext.SelectedCulture) );

                        string fileName = null;
                        if (ReportFileNameProvider != null)
                        {
                            fileName = ReportFileNameProvider();
                            fileName = String.Join("_", fileName.Split(System.IO.Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

                            if (Path.HasExtension(fileName) == false)
                            {
                                fileName = Path.Combine(fileName, ".csv");
                            }
                        }
                        var csvSeparator =  ",";
                        var fileResult = new CSVFileResult(separator: csvSeparator) { Content = table, FileDownloadName = fileName ?? "report.csv" };
                        fileResult.ExecuteResult(context);

                    }
                }
            }
            else
            {
                bool init = Request.Init == true;
                bool updateContent = (Request.UpdateContent ?? ContentHtml != null) || state.ItemCount.HasValue == false;
                bool updatePagination = (Request.UpdatePagination ?? PaginationHtml != null) || state.ItemCount.HasValue == false;
                bool updateTools = Request.UpdateTools == true;
                bool updateItemCountDesc = Request.UpdateItemCountDescription ?? (options.DisplayItemCountDescription == true && updateContent == true);

                var htmlHelper = context.CreateHtmlHelper();

                var result = new Dictionary<string, object> { { "requestId", Request.RequestId } };

                if (updateContent == true)
                {
                    if (ContentHtml != null)
                    {
                        result["content"] = ContentHtml;
                    }
                    else if (Content != null)
                    {
                        if (state.Pagination != null)
                        {
                            if (Request.ResetPaging == true)
                            {
                                state.Pagination.PageIndex = null;
                            }
                        }

                        var contentResult = Content(state);


                        state.ItemCount = contentResult.ItemCountProvider();

                        // Recalc page if out of bounds
                        if (state.Pagination != null)
                        {
                            int firstDisplayedItemIndex = (state.Pagination.PageIndex ?? 0) * (state.Pagination.PageSize ?? 0);
                            if (firstDisplayedItemIndex >= state.ItemCount.Value)
                            {
                                if (firstDisplayedItemIndex == 0)
                                {
                                    updatePagination = true;
                                }
                                else
                                {
                                    state.Pagination.PageIndex = null;

                                    // Rerun content
                                    contentResult = Content(state);
                                }
                            }
                        }

                        string contentHtml = contentResult.Html.GetContentHtml(htmlHelper);

                        result["content"] = contentHtml;


                        //result["content"] = Content.GetContentHtml(htmlHelper);


                        if (state.ItemCount == 0)
                        {
                            //if (string.IsNullOrEmpty(contentHtml) == true)
                            if (DataViewOptions.DisplayEmptyDataMessage == true)
                            {
                                if (contentResult.DataView != null && contentResult.DataView.ItemCount > 0)
                                {
                                    if (string.IsNullOrEmpty(DataViewOptions.EmptyFilterDataMessage) == false)
                                    {
                                        var noData = new TagBuilder("h3");
                                        noData.AddCssClass("data-view-no-item");
                                        noData.SetInnerText(DataViewOptions.EmptyFilterDataMessage);

                                        var tempContent = string.Format("{0} {1}", contentHtml, noData.ToString());
                                        result["content"] = tempContent;
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(DataViewOptions.EmptyDataMessage) == false)
                                    {
                                        var noData = new TagBuilder("h3");
                                        noData.AddCssClass("data-view-no-item");
                                        noData.SetInnerText(DataViewOptions.EmptyDataMessage);

                                        result["content"] = noData.ToString();
                                    }
                                }
                            }
                        }


                       
                        if (contentResult.Attributes != null)
                        {
                            var attrDict = MultivendorWebViewer.Helpers.Helpers.GetHtmlAttributes(contentResult.Attributes);
                            foreach (var kvp in attrDict)
                            {
                                result[kvp.Key] = kvp.Value;
                            }
                        }
                    }

                    //string tableHtml = result["content"] as string;
                    //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    //var tableDoc = XDocument.Parse(tableHtml);
                    //var table = tableDoc.Descendants("table");
                    //var header = table.Descendants("thead");
                    //var headerColumns = header.Descendants("th");
                    //var headerStr = string.Join(";", headerColumns.Select(c => c.Value));
                    //sb.AppendLine(headerStr);
                    //var rows = table.Descendants("tbody").Descendants("tr");
                    //foreach (var row in rows)
                    //{
                    //    var cellsStrings = string.Join(";", row.Nodes().OfType<XElement>().Select(c => c.Value));
                    //    sb.AppendLine(cellsStrings);
                    //}
                }

                if (updateTools == true)
                {
                    if (ToolsHtml != null)
                    {
                        result["tools"] = ToolsHtml;
                    }
                    else
                    {
                        result["tools"] = MultivendorWebViewer.Helpers.DataViewHelper.RenderTools(htmlHelper, options, state, RequestContext/*, renderOnlyItems: Request.HasTools == true*/).ToHtmlString();
                    }
                }
                else if (updateSort == true)
                {
                    result["sort"] = MultivendorWebViewer.Helpers.DataViewHelper.RenderSortToolBarItem(htmlHelper, options, state, RequestContext).ToHtmlString();
                }

                if (state.ItemCount.HasValue == true)
                {
                    result["itemCount"] = state.ItemCount.Value;

                    if (updatePagination == true)
                    {
                        if (PaginationHtml != null)
                        {
                            result["pagination"] = PaginationHtml;
                        }
                        else if (state.Pagination != null)
                        {
                            if (Request.ResetPaging == true)
                            {
                                state.Pagination.PageIndex = null;
                            }

                            result["pagination"] = MultivendorWebViewer.Helpers.DataViewHelper.RenderPaginationTools(htmlHelper, options, state, RequestContext, renderOnlyItems: Request.HasPaginationTools == true).ToHtmlString();
                        }
                    }

                    if (updateItemCountDesc == true)
                    {
                        result["itemCountDescription"] = MultivendorWebViewer.Helpers.DataViewHelper.RenderItemCountDescription(htmlHelper, options, state).ToHtmlString();
                    }
                }

                var response = context.HttpContext.Response;

                response.ContentType = "application/json";
                var serializedObject = JsonConvert.SerializeObject(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                response.Write(serializedObject);
            }

        }

    }

    //public class PaginationResultContext
    //{
    //    public PaginationResultContext(ControllerContext context, DataViewRequest paginationRequest)
    //    {
    //        ControllerContext = context;
    //        PaginationRequest = paginationRequest;
    //    }

    //    public ControllerContext ControllerContext { get; private set; }

    //    public DataViewRequest PaginationRequest { get; set; }

    //    private ApplicationRequestContext applicationRequestContext;
    //    public ApplicationRequestContext ApplicationRequestContext { get { return applicationRequestContext ?? (applicationRequestContext = ControllerContext.GetApplicationRequestContext()); } }

    //    public string PartialView(string view, object model = null)
    //    {
    //        return ViewHelpers.RenderPartial(ControllerContext, view, model: model);
    //    }
    //}

    public class DataViewContentResult
    {
        public DataViewContentResult(HtmlContent html, int itemCount)
        {
            Html = html;
            ItemCountProvider = () => itemCount;
        }
        public DataViewContentResult(string partialViewName, object model, IDataView dataView = null, int? itemCount = null)
        {
            Html = HtmlContent.Partial(partialViewName, model);
            ItemCountProvider = () => itemCount ?? (dataView != null ? dataView.DisplayedItemCount : 0);
            DataView = dataView;
            Model = model;
        }

        public DataViewContentResult(string partialViewName, IDataView dataView)
        {
            Html = HtmlContent.Partial(partialViewName, dataView);
            ItemCountProvider = () => dataView.DisplayedItemCount;
            DataView = dataView;
        }

        public HtmlContent Html { get; set; }

        public Func<int> ItemCountProvider { get; set; }

        public IDataView DataView { get; set; }

        public object Model { get; set; }

        public IEnumerable<object> ReportTableItems { get; set; }

        public object Attributes { get; set; }
    }

    [Flags]
    public enum DataViewRequestElemets { Tools = 0x1, Content = 0x2, Pagination = 0x4 };

    [ModelBinder(typeof(JsonNetModelBinder))]
    public class DataViewRequest
    {
        [JsonProperty("requestId")]
        public long RequestId { get; set; }

        [JsonProperty("init")]
        public bool? Init { get; set; }

        [JsonProperty("hasTools")]
        public bool? HasTools { get; set; }

        [JsonProperty("hasPagination")]
        public bool? HasPaginationTools { get; set; }


        [JsonProperty("updateContent")]
        public bool? UpdateContent { get; set; }

        [JsonProperty("updatePagination")]
        public bool? UpdatePagination { get; set; }

        [JsonProperty("updateTools")]
        public bool? UpdateTools { get; set; }


        [JsonProperty("updateSort")]
        public bool? UpdateSort { get; set; }

        [JsonProperty("updateItemCount")]
        public bool? UpdateItemCountDescription { get; set; }

        [JsonProperty("updateFacets")]
        public bool? UpdateFacets { get; set; }


        [JsonProperty("sortChanged")]
        public bool? SortChanged { get; set; }

        [JsonProperty("viewChanged")]
        public bool? ViewChanged { get; set; }

        [JsonProperty("paginationChanged")]
        public bool? PaginationChanged { get; set; }

        [JsonProperty("filterChanged")]
        public bool? FilterChanged { get; set; }

        //[JsonProperty("elements")]
        //public DataViewRequestElemets Elements { get; set; }


        [JsonProperty("state")]
        public DataViewState State { get; set; }

        [JsonProperty("report")]
        public bool DownloadReport { get; set; }

        [JsonProperty("resetPaging")]
        public bool ResetPaging { get; set; }
    }

    //[ModelBinder(typeof(JsonNetModelBinder))]
    //public class DataViewState
    //{
    //    public DataViewState()
    //    {

    //    }

    //    [JsonProperty("itemCount")]
    //    public int? ItemCount { get; set; }

    //    [JsonProperty("pagination")]
    //    public PaginationSelection Pagination { get; set; }

    //    //public bool PaginationDefined {  get {  return Pagination != null && (Pagination.PageIndex.HasValue == true || Pagination.)} }

    //    [JsonProperty("sort")]
    //    public Sort Sort { get; set; }

    //    [JsonProperty("sortId")]
    //    public string SortId { get; set; }

    //    [JsonProperty("viewId")]
    //    public string ViewId { get; set; }

    //    [JsonProperty("filter")]
    //    public DataViewFilterSelection Filter { get; set; }

    //    [JsonProperty("tools")]
    //    public DataViewToolState[] Tools { get; set; }

    //    public string FilterQuery { get { return Filter != null ? Filter.Query : null; } }

    //    public virtual DataViewOptions CreateOptions()
    //    {
    //        var options = new DataViewOptions();
    //        if (Pagination != null)
    //        {
    //            if (Pagination.PageSizes != null)
    //            {
    //                options.PageSizes = PaginationPageSize.Create(Pagination.PageSizes);
    //            }
    //            if (Pagination.PageSize.HasValue == true)
    //            {
    //                options.DefaultPageSize = Pagination.PageSize;
    //            }
    //        }

    //        options.DefaultViewSelector = ViewId;
    //        options.DefaultSortSelector = SortId;
    //        //options.DefaultFilterSelector = Filter != null ? Filter.Query : null;
    //        return options;
    //    }

    //    public DataViewToolState GetTool(string name)
    //    {
    //        if (Tools == null || name == null) return null;
    //        var comparer = StringComparer.OrdinalIgnoreCase;
    //        return Tools.FirstOrDefault(t => comparer.Equals(t.Name, name) == true);
    //    }

    //    public bool? IsToolChecked(string name)
    //    {
    //        var tool = GetTool(name);
    //        return tool != null ? (bool?)tool.Checked : null;
    //    }

    //    //public SearchFacetSelection GetFacetSelection(string id)
    //    //{
    //    //    return Filter != null ? Filter.GetFacetSelection(id) : null;
    //    //}
    //}

    public class DataViewToolState
    {
        public DataViewToolState()
        {
            Values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("checked")]
        public bool Checked { get; set; }

        [JsonProperty("selectedValue")]
        public string SelectedValue { get; set; }

        [JsonProperty("selectedValues")]
        public string[] SelectedValues { get; set; }

        [JsonProperty("values")]
        public Dictionary<string, string> Values { get; set; }
    }

    //[ModelBinder(typeof(JsonNetModelBinder))]
    //public class DataViewState<TFilter> : DataViewState
    //{
    //    [JsonProperty("advancedFilter")]
    //    public TFilter AdvancedFilter { get; set; }
    //}
}