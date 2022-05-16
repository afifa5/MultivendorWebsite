using Newtonsoft.Json;

using MultivendorWebViewer.Common;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Helpers;
using MultivendorWebViewer.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultivendorWebViewer.Components;

namespace MultivendorWebViewer.Components
{
    public class DataViewState
    {
        [JsonProperty("itemCount")]
        public int? ItemCount { get; set; }

        [JsonProperty("pagination")]
        public PaginationSelection Pagination { get; set; }

        //public bool PaginationDefined {  get {  return Pagination != null && (Pagination.PageIndex.HasValue == true || Pagination.)} }

        [JsonProperty("sort")]
        public Sort Sort { get; set; }

        [JsonProperty("sortId")]
        public string SortId { get; set; }

        [JsonProperty("viewId")]
        public string ViewId { get; set; }

        [JsonProperty("filter")]
        public DataViewFilterSelection Filter { get; set; }

        [JsonProperty("tools")]
        public DataViewToolState[] Tools { get; set; }

        public string FilterQuery { get { return Filter != null ? Filter.Query : null; } }

        public virtual DataViewOptions CreateOptions()
        {
            var options = new DataViewOptions();
            if (Pagination != null)
            {
                if (Pagination.PageSizes != null)
                {
                    options.PageSizes = PaginationPageSize.Create(Pagination.PageSizes);
                }
                if (Pagination.PageSize.HasValue == true)
                {
                    options.DefaultPageSize = Pagination.PageSize;
                }
            }

            options.DefaultViewSelector = ViewId;
            options.DefaultSortSelector = SortId;
            //options.DefaultFilterSelector = Filter != null ? Filter.Query : null;
            return options;
        }

        public DataViewToolState GetTool(string name)
        {
            if (Tools == null || name == null) return null;
            var comparer = StringComparer.OrdinalIgnoreCase;
            return Tools.FirstOrDefault(t => comparer.Equals(t.Name, name) == true);
        }

        public bool? IsToolChecked(string name)
        {
            var tool = GetTool(name);
            return tool != null ? (bool?)tool.Checked : null;
        }


    }

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


    public interface IDataView
    {
        int ItemCount { get; }

        int DisplayedItemCount { get; }

        int VisibleItemCount { get; }

        Sort Sort { get; set; }

        int? PageSize { get; set; }

        int PageIndex { get; set; }

        IEnumerable<object> Items { get; }

        IEnumerable<object> DisplayedItems { get; }

        IEnumerable<object> VisibleItems { get; }

        IEnumerable<object> GetItems(bool applyFilter, bool applySort, bool applyPaging);
    }



    public abstract class DataView : IDataView
    {
        public DataView(ApplicationRequestContext applicationRequestContext)
        {
            ApplicationRequestContext = applicationRequestContext;
        }

        public string ViewId { get; set; }

        public ApplicationRequestContext ApplicationRequestContext { get; private set; }

        public abstract int ItemCount { get; }

        public abstract int DisplayedItemCount { get; }

        public abstract int VisibleItemCount { get; }

        IEnumerable<object> IDataView.Items { get { throw new NotImplementedException(); } }

        IEnumerable<object> IDataView.DisplayedItems { get { throw new NotImplementedException(); } }

        IEnumerable<object> IDataView.VisibleItems { get { throw new NotImplementedException(); } }

        IEnumerable<object> IDataView.GetItems(bool applyFilter, bool applySort, bool applyPaging)
        {
            throw new NotImplementedException();
        }

        public Sort Sort { get; set; }

        public int? PageSize { get; set; }

        public int PageIndex { get; set; }

        //public static DataViewStringModel<TViewModel, TItem> Create<TViewModel, TItem>(TViewModel model, IEnumerable<TItem> items, CultureInfo cultureInfo, DataViewState state, StringCompareMode stringCompareMode = StringCompareMode.Contains, bool ignoreCase = true, int? itemCount = null)
        //{
        //    return new DataViewStringModel<TViewModel, TItem>(model, items, state, cultureInfo, stringCompareMode, ignoreCase);
        //}

        //public static DataViewStringModel<TViewModel, TItem> Create<TViewModel, TItem>(TViewModel model, Func<TViewModel, IEnumerable<TItem>> itemsProvider, CultureInfo cultureInfo, DataViewState state, StringCompareMode stringCompareMode = StringCompareMode.Contains, bool ignoreCase = true, int? itemCount = null)
        //{
        //    return DataViewModel.Create<TViewModel, TItem>(model, itemsProvider != null ? itemsProvider(model) : null, cultureInfo, state, stringCompareMode, ignoreCase, itemCount);
        //}
    }

    public abstract class ServerDataViewBase<TIn, TResult> : DataView, IDataView
    {
        public ServerDataViewBase(ApplicationRequestContext applicationRequestContext, TIn[] items, DataViewState state, int totalItemCount, int? displayedItemCount = null)
            : base(applicationRequestContext)
        {
            if (state != null)
            {
                if (state.Sort != null)
                {
                    Sort = state.Sort;
                }

                if (state.Pagination != null)
                {
                    PageSize = state.Pagination.PageSize;
                    PageIndex = state.Pagination.PageIndex ?? 0;
                }

                ViewId = state.ViewId;
            }

            SourceItems = items;
            this.totalItemCount = totalItemCount;
            this.displayedItemCount = displayedItemCount;
        }


        private int totalItemCount;

        private int? displayedItemCount;

        public override int ItemCount { get { return totalItemCount; } }

        public override int DisplayedItemCount { get { return displayedItemCount ?? VisibleItemCount; } }

        public override int VisibleItemCount { get { return Items.Length; } }

        protected TIn[] SourceItems { get; private set; }

        private TResult[] items;
        public TResult[] Items { get { return items ?? (items = GetItemsCore(SourceItems).ToArray()); } }

        //public virtual IEnumerable<TResult> DisplayedItems { get { throw new NotImplementedException(); } }

        //public virtual IEnumerable<TResult> VisibleItems { get { throw new NotImplementedException(); } }

        protected abstract IEnumerable<TResult> GetItemsCore(TIn[] items);

        IEnumerable<object> IDataView.Items { get { return Items.Cast<object>(); } }

        IEnumerable<object> IDataView.DisplayedItems { get { return Items.Cast<object>(); } }

        IEnumerable<object> IDataView.VisibleItems { get { return Items.Cast<object>(); } }
    }

    public abstract class DataViewBase<TIn, TResult> : DataView, IDataView
    {
        public DataViewBase(ApplicationRequestContext applicationRequestContext, IEnumerable<TIn> items, int? itemCount = null)
            : base(applicationRequestContext)
        {
            Items = items;
            this.itemCount = itemCount;
        }

        public DataViewBase(ApplicationRequestContext applicationRequestContext, IEnumerable<TIn> items, DataViewState state, int? overrideItemCount = null)
            : base(applicationRequestContext)
        {
            Items = items;
            if (state != null)
            {
                if (state.Sort != null)
                {
                    Sort = state.Sort;
                }

                if (state.Pagination != null)
                {
                    PageSize = state.Pagination.PageSize;
                    PageIndex = state.Pagination.PageIndex ?? 0;
                }

                ViewId = state.ViewId;
            }
            this.itemCount = overrideItemCount;
        }

        protected IEnumerable<TIn> Items { get; set; }

        private int? itemCount;
        public override int ItemCount { get { return (int)(itemCount ?? (itemCount = Items.Count())); } }

        public override int DisplayedItemCount { get { return DisplayedItems.Count; } }

        public override int VisibleItemCount { get { return VisibleItems.Count; } }

        private IList<TResult> displayedItems;
        public IList<TResult> DisplayedItems { get { return displayedItems ?? (displayedItems = GetItemsView(GetItemsCore(Items, true, true, false), true, true, false).ToList()); } }

        private IList<TResult> visibleItems;
        public IList<TResult> VisibleItems
        {
            get
            {
                if (visibleItems == null)
                {
                    if (displayedItems != null)
                    {
                        visibleItems = GetItemsView(displayedItems, false, false, true).ToList();
                    }
                    else
                    {
                        visibleItems = GetItemsView(GetItemsCore(Items, true, true, true), true, true, true).ToList();
                    }
                }
                return visibleItems;
            }
        }

        public ItemSorter<TResult> Sorterer { get; set; }

        //public IEnumerable<ItemFilterer<TResult, DataViewFilterSelection>> Filterers { get; set; }

        public IEnumerable<TResult> GetItems(bool applyFilter, bool applySort, bool applyPaging)
        {
            return GetItemsView(GetItemsCore(Items, applyFilter, applySort, applyPaging), applyFilter, applySort, applyPaging);
        }

        protected abstract IEnumerable<TResult> GetItemsCore(IEnumerable<TIn> items, bool applyFilter, bool applySort, bool applyPaging);

        protected virtual IEnumerable<TResult> GetItemsView(IEnumerable<TResult> items, bool applyFilter, bool applySort, bool applyPaging)
        {
            if (applySort == true && Sorterer != null)
            {
                items = ApplySort(items, Sort);
            }

            if (applyPaging == true && PageSize.HasValue == true)
            {
                items = ApplyPagination(items, PageIndex, PageSize.Value);
            }

            return items;
        }

        protected virtual IEnumerable<TResult> ApplySort(IEnumerable<TResult> items, Sort sort)
        {
            return Sorterer != null ? Sorterer.Sort(items, sort) : items;
        }

        protected virtual IEnumerable<TResult> ApplyPagination(IEnumerable<TResult> items, int pageIndex, int pageSize)
        {
            var list = items as IList<TResult>;
            if (list != null)
            {
                return new RangeEnumerable<TResult>(list, pageIndex * pageSize, Math.Min(pageSize, list.Count));
            }
            return items.Skip(pageIndex * pageSize).Take(pageSize);
        }

        IEnumerable<object> IDataView.Items { get { return Items.Cast<object>(); } }

        IEnumerable<object> IDataView.DisplayedItems { get { return DisplayedItems.Cast<object>(); } }

        IEnumerable<object> IDataView.VisibleItems { get { return VisibleItems.Cast<object>(); } }

        IEnumerable<object> IDataView.GetItems(bool applyFilter, bool applySort, bool applyPaging)
        {
            return GetItems(applyFilter, applySort, applyPaging).Cast<object>();
        }
    }

    public abstract class DataViewBase<TIn, TResult, TFilter> : DataViewBase<TIn, TResult>, IDataView
    {
        public DataViewBase(ApplicationRequestContext applicationRequestContext, IEnumerable<TIn> items, int? itemCount = null) : base(applicationRequestContext, items, itemCount) { }

        public DataViewBase(ApplicationRequestContext applicationRequestContext, IEnumerable<TIn> items, DataViewState state, DataViewOptions options = null, int? overrideItemCount = null)
            : base(applicationRequestContext, items, state, overrideItemCount)
        {
            //if (options != null)
            //{
            //    if (options.FacetSettings != null)
            //    {
            //        Filterers = CreateDefaultFilters(options.FacetSettings).ToList();
            //    }
            //}
        }

        //protected virtual IEnumerable<ItemFilterer<TResult, TFilter>> CreateDefaultFilters(IEnumerable<DataViewFacetSetting> facetSettings)
        //{
        //    return Enumerable.Empty<ItemFilterer<TResult, TFilter>>();
        //}

        public TFilter Filter { get; set; }

        public ItemQueryFilterer<TResult> QueryFilterer { get; set; }

        public IList<ItemFilterer<TResult, TFilter>> Filterers { get; set; }

        //public virtual DataViewFacetsViewModel GetFacetsViewModel(DataViewState state)
        //{
        //    var facets = GetFacetViewModels(state).ToList();
        //    if (facets.Count > 0)
        //    {
        //        return new DataViewFacetsViewModel(ApplicationRequestContext) { Facets = facets };
        //    }
        //    return null;
        //}

        //public virtual IEnumerable<DataViewFacetViewModel> GetFacetViewModels(DataViewState state)
        //{
        //    return Enumerable.Empty<DataViewFacetViewModel>();
        //}

        //private IList<TResult> filterItems;
        //public IList<TResult> FilterItems
        //{
        //    get
        //    {
        //        if (this.filterItems == null)
        //        {
        //            var filteredItems = Items;
        //            if (Filter != null)
        //            {
        //                filteredItems = ApplyFilter(filteredItems);
        //            }

        //            this.filterItems = filteredItems.ToArray();
        //        }
        //        return this.filterItems;
        //    }
        //}

        protected override IEnumerable<TResult> GetItemsView(IEnumerable<TResult> items, bool applyFilter, bool applySort, bool applyPaging)
        {
            return base.GetItemsView(applyFilter == true ? ApplyFilter(items) : items, applyFilter, applySort, applyPaging);
        }

        protected virtual IEnumerable<TResult> ApplyFilter(IEnumerable<TResult> items)
        {
            if (QueryFilterer != null)
            {
                var queryFilter = Filter as IDataViewQueryFilterSelection;
                if (queryFilter != null)
                {
                    items = QueryFilterer.Filter(items, queryFilter).ToArray();
                }
            }

            if (Filterers != null)
            {
                foreach (var filterer in Filterers)
                {
                    if (filterer.IsFilterActive(Filter) == true)
                    {
                        items = filterer.Filter(items, Filter).ToArray();
                    }
                }
            }

            return items;
        }
    }

    public abstract class DataViewStringBase<TIn, TResult> : DataViewBase<TIn, TResult, DataViewFilterSelection>
    {
        public DataViewStringBase(ApplicationRequestContext applicationRequestContext, IEnumerable<TIn> items, DataViewFilterSelection filter, DataViewOptions options = null, CultureInfo cultureInfo = null, StringCompareMode stringCompareMode = StringCompareMode.Contains, bool ignoreCase = true, int? itemCount = null)
            : base(applicationRequestContext, items, itemCount)
        {
            Filter = filter;

            if (cultureInfo == null)
            {
                if (ApplicationRequestContext != null)
                {
                    cultureInfo =new CultureInfo( ApplicationRequestContext.SelectedCulture);
                }
                if (cultureInfo == null)
                {
                    cultureInfo = CultureInfo.CurrentUICulture;
                }
            }

            QueryFilterer = new ItemQueryFilterer<TResult>(ApplicationRequestContext, cultureInfo, stringCompareMode, ignoreCase);
            Sorterer = new ItemStringSorter<TResult>(cultureInfo, ignoreCase);
        }

        public DataViewStringBase(ApplicationRequestContext applicationRequestContext, IEnumerable<TIn> items, DataViewState state, DataViewOptions options = null, CultureInfo cultureInfo = null, StringCompareMode stringCompareMode = StringCompareMode.Contains, bool ignoreCase = true, int? overrideItemCount = null)
            : base(applicationRequestContext, items, state, options, overrideItemCount)
        {
            if (state != null)
            {
                Filter = state.Filter;
            }

            if (cultureInfo == null)
            {
                if (ApplicationRequestContext != null)
                {
                    cultureInfo = new CultureInfo( ApplicationRequestContext.SelectedCulture);
                }
                if (cultureInfo == null)
                {
                    cultureInfo = CultureInfo.CurrentUICulture;
                }
            }

            QueryFilterer = new ItemQueryFilterer<TResult>(ApplicationRequestContext, cultureInfo, stringCompareMode, ignoreCase);
            Sorterer = new ItemStringSorter<TResult>(cultureInfo, ignoreCase);
        }

        //protected override IEnumerable<ItemFilterer<TResult, DataViewFilterSelection>> CreateDefaultFilters(IEnumerable<DataViewFacetSetting> facetSettings)
        //{
        //    foreach (var faceSetting in facetSettings)
        //    {
        //        // TODO , use a more structured creation mechanism.
        //        //var rangeFacetSetting = faceSetting as DataViewRangeFacetSetting;
        //        //if (rangeFacetSetting != null)
        //        //{
        //        //    yield return new DataViewRangeFacet<TResult>() { Settings = rangeFacetSetting, RequestContext = ApplicationRequestContext };
        //        //}
        //        //else
        //        //{
        //        //    var serialNumberFacetSettings = faceSetting as DataViewSerialNumberFacetSetting;
        //        //    if (serialNumberFacetSettings != null)
        //        //    {
        //        //        yield return new DataViewSerialNumberFilterFacet<TResult>() { Settings = serialNumberFacetSettings, RequestContext = ApplicationRequestContext };
        //        //    }
        //        //}
        //        yield return null;
        //    }
        //}

        //public override IEnumerable<DataViewFacetViewModel> GetFacetViewModels(DataViewState state)
        //{
        //    if (Filterers != null)
        //    {
        //        var facets = new List<DataViewFacetViewModel>();
        //        foreach (var filterer in Filterers)
        //        {
        //            DataViewFacetViewModel facetViewModel = null;
        //            var rangeFilterer = filterer as DataViewRangeFacet<TResult>;
        //            if (rangeFilterer != null)
        //            {
        //                facetViewModel = new DataViewRangeFacetViewModel(ApplicationRequestContext) { DataView = this, Selection = state.GetFacetSelection(rangeFilterer.Settings.Id) ?? new SearchFacetSelection(), Settings = rangeFilterer.Settings };
        //            }
        //            else
        //            {
        //                var serialNumberFilterer = filterer as DataViewSerialNumberFilterFacet<TResult>;
        //                if (serialNumberFilterer != null)
        //                {
        //                    facetViewModel = new DataViewSerialNumberFilterFacetViewModel(ApplicationRequestContext) { DataView = this, Selection = state.GetFacetSelection(serialNumberFilterer.Settings.Id) ?? new SearchFacetSelection(), Settings = serialNumberFilterer.Settings };
        //                }
        //            }

        //            if (facetViewModel != null)
        //            {
        //                facetViewModel.Init();
        //                facets.Add(facetViewModel);
        //            }
        //        }
        //        return facets;
        //    }
        //    return base.GetFacetViewModels(state);
        //}
    }



  
    public class DataViewString<TItem> : DataViewStringBase<TItem, TItem>
    {
        public DataViewString(ApplicationRequestContext applicationRequestContext, IEnumerable<TItem> items, DataViewFilterSelection filter, DataViewOptions options = null, CultureInfo cultureInfo = null, StringCompareMode stringCompareMode = StringCompareMode.Contains, bool ignoreCase = true, int? itemCount = null)
            : base(applicationRequestContext, items, filter, options, cultureInfo, stringCompareMode, ignoreCase, itemCount)
        {
        }

        public DataViewString(ApplicationRequestContext applicationRequestContext, IEnumerable<TItem> items, DataViewState state = null, DataViewOptions options = null, CultureInfo cultureInfo = null, StringCompareMode stringCompareMode = StringCompareMode.Contains, bool ignoreCase = true, int? overrideItemCount = null)
            : base(applicationRequestContext, items, state, options, cultureInfo, stringCompareMode, ignoreCase, overrideItemCount)
        {
        }

        protected override IEnumerable<TItem> GetItemsCore(IEnumerable<TItem> items, bool applyFilter, bool applySort, bool applyPaging)
        {
            return items;
            //return GetItemsView(items, applyFilter, applySort, applyPaging);
        }
    }

    public class DataViewModelString<TItem, TModel> : DataViewString<TItem>
    {
        public DataViewModelString(ApplicationRequestContext applicationRequestContext, TModel model, IEnumerable<TItem> items, DataViewState state, DataViewOptions options = null, CultureInfo cultureInfo = null, StringCompareMode stringCompareMode = StringCompareMode.Contains, bool ignoreCase = true, int? overrideItemCount = null)
            : base(applicationRequestContext, items, state, options, cultureInfo, stringCompareMode, ignoreCase, overrideItemCount)
        {
            Model = model;
        }

        public TModel Model { get; set; }
    }
}
