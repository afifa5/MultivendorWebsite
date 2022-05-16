using MultivendorWebViewer.Common;
using MultivendorWebViewer.Components;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Common
{

    [Flags]
    public enum PaginationPageSelectorTypes { None = 0x0, Previous = 0x1, Next = 0x2, First = 0x4, Last = 0x8, Index = 0x10, All = Previous | Next | First | Last | Index, NotSet = ~All }
    public interface IHtmlContent
    {
        object Content { get; }
    }
    public class DataViewOptions 
    {
        public DataViewOptions()
        {
            PageSelectorTypes = PaginationPageSelectorTypes.All;
            SortAlignment = ToolBarAlignment.Right;
        }
        [XmlIgnore]
        public string Url { get; set; }

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlIgnore]
        public IHtmlContent CaptionHtml { get; set; }

        [XmlAttribute("item-count-format")]
        public string ItemCountDescriptionFormat { get; set; }

        [XmlIgnore]
        public bool? DisplayItemCountDescription { get; set; }

        [XmlAttribute("dispay-item-count")]
        public string DisplayItemCountDescriptionSerializable { get { return DisplayItemCountDescription.ToString(); } set { DisplayItemCountDescription = value.ToNullableBool(); } }

        [XmlAttribute("item-count-template")]
        public string ItemCountDescriptionTemplate { get; set; }

        [XmlIgnore]
        public IHtmlContent ItemCountDescriptionHtml { get; set; }

        [XmlIgnore]
        public bool? LayoutFill { get; set; }

        [XmlIgnore]
        public ElementSelector ContentSelector { get; set; }

        [XmlIgnore]
        public bool? DownloadToolAvailable { get; set; }

        [XmlAttribute("display-download-tool")]
        public string DownloadToolAvailableSerializable { get { return DownloadToolAvailable.ToString(); } set { DownloadToolAvailable = value.ToNullableBool(); } }

        public int? DelayedLoad { get; set; }

        [XmlAttribute("delayed-load")]
        public string DelayedLoadSerializable { get { return DelayedLoad.ToString(); } set { DelayedLoad = value.ToNullableInt(); } }

        #region Pagination

        [XmlIgnore]
        public bool? AutoHidePageSizes { get; set; }

        [XmlAttribute("auto-hide-page-sizes")]
        public string AutoHidePageSizesSerializable { get { return AutoHidePageSizes.ToString(); } set { AutoHidePageSizes = value.ToNullableBool(); } }

        [XmlIgnore]
        public int? DefaultPageSize { get; set; }

        [XmlAttribute("default-page-size")]
        public string DefaultPageSizeSerializable { get { return DefaultPageSize.ToString(); } set { DefaultPageSize = value.ToNullableInt(); } }

        [XmlIgnore]
        public PaginationPageSize[] PageSizes { get; set; }

        [XmlAttribute("page-sizes")]
        public string PageSizesSerializable { get { return PageSizes != null ? string.Join(", ", PageSizes.Select(p => p.ToString())) : null; } set { PageSizes = PaginationPageSize.Create(value); } }

        public PaginationPageSelectorTypes PageSelectorTypes { get; set; }

        [XmlIgnore]
        public int? MaxIndexPageSelectors { get; set; }

        [XmlAttribute("max-index-page-selectors")]
        public string MaxIndexPageSelectorsSerializable { get { return MaxIndexPageSelectors.ToString(); } set { MaxIndexPageSelectors = value.ToNullableInt(); } }

        //public List<PaginationPageSelector> PageSelectors { get; set; }

        //public PaginationIcons Icons { get; set; }

        [XmlIgnore]
        public bool PaginationDefined { get { return PageSizes != null && PageSizes.Length > 0; } }

        [XmlIgnore]
        public bool? DisplayEmptyDataMessage { get; set; }

        [XmlIgnore]
        public string EmptyDataMessage { get; set; }

        [XmlIgnore]
        public string EmptyFilterDataMessage { get; set; }

        #endregion

        #region View

        [XmlElement("ViewSelector")]
        public DataViewSelectorCollection<DataViewViewSelector> ViewSelectors { get; set; }

        [XmlAttribute("default-view-selector")]
        public string DefaultViewSelector { get; set; }

        [XmlElement("DisplayViewSelector")] // For backward compability
        public bool? DisplayViewSelector { get; set; }

        [XmlAttribute("display-view-selector")]
        public string DisplayViewSelectorSerializable { get { return DisplayViewSelector.ToString(); } set { DisplayViewSelector = value.ToNullableBool(); } }

        #endregion

        #region Sort 

        [XmlAttribute("sort-alignment")]
        public ToolBarAlignment SortAlignment { get; set; }

        [XmlElement("SortSelector")]
        public DataViewSelectorCollection<DataViewSortSelector> SortSelectors { get; set; } // OrderSelectors

        [XmlAttribute("default-sort-selector")]
        public string DefaultSortSelector { get; set; }

        [XmlElement("DisplaySortSelector")] // For backward compability
        public bool? DisplaySortSelector { get; set; }

        [XmlAttribute("display-sort-selector")]
        public string DisplaySortSelectorSerializable { get { return DisplaySortSelector.ToString(); } set { DisplaySortSelector = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool SortDefined { get { return SortSelectors != null && SortSelectors.Count > 0; } }

        #endregion

        #region Filter

        [XmlElement("FilterSelector")]
        public DataViewSelectorCollection<DataViewFilterSelector> FilterSelectors { get; set; }

        public DataViewFilterSelection DefaultFilterSelection { get; set; }

        [XmlAttribute("filter-placeholder")]
        public string FilterPlaceholder { get; set; }

        [XmlElement("DisplayFilter")] // For backward compability
        public bool? DisplayFilter { get; set; }

        [XmlAttribute("display-filter")]
        public string DisplayFilterSerializable { get { return DisplayFilter.ToString(); } set { DisplayFilter = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool FilterDefined { get { return FilterSelectors != null && FilterSelectors.Count > 0; } }

        [XmlElement("FilterQueryFormat")]
        public List<Formatter> QueryFormatters { get; set; }

        //[XmlArray("Facets")]
        //[XmlArrayItem(typeof(DataViewRangeFacetSetting), ElementName = "Range")]
        //[XmlArrayItem(typeof(DataViewSerialNumberFacetSetting), ElementName = "SerialNumber")]
        //public DataViewSelectorCollection<DataViewFacetSetting> FacetSettings { get; set; }

        //[XmlAttribute("facet-container")]
        //public string FacetContainerSelector { get; set; }

        #endregion

        #region Report

        //[XmlElement("TableReportColumn")]
        //public MergeableSettingsCollection<TableReportColumn> TableReportColumns { get; set; }

        #endregion

        [XmlIgnore]
        public List<ToolBarItem> Tools { get; set; }

        //public Func<PaginationPageSize, string> PageSizeUrlProvider { get; set; }

        //public Func<PaginationPageSelector, string> PageUrlProvider { get; set; }

        //[XmlIgnore]
        //public ComponentLayoutMode? Layout { get; set; }

        //[XmlAttribute("display")]
        //public virtual string LayoutSerializable { get { return ComponentLayoutModeHelper.ToBackwardCompabilityString(Layout); } set { Layout = ComponentLayoutModeHelper.FromBackwardCompabilityString(value); } }

        //[XmlAttribute("label")]
        //public string Label { get; set; }

        //[XmlAttribute("label-short")]
        //public string LabelShort { get; set; }

        //[XmlAttribute("label-tag")]
        //public string LabelTag { get; set; }

        //[XmlAttribute("label-for")]
        //public string LabelFor { get; set; }

        //[XmlAttribute("label-class")]
        //public string LabelClass { get; set; }

        public static class ToolLocations
        {
            public const string AfterFilterSelector = "AfterFilterSelector";

            public const string BeforeViewSelectors = "BeforeViewSelectors";

            public const string AfterViewSelectors = "AfterViewSelectors";

            public const string BeforeSortSelectors = "BeforeSortSelectors";

            public const string AfterSortSelectors = "AfterSortSelectors";

            public const string BeforePageSelectors = "BeforePageSelectors";

            public const string AfterPageSelectors = "AfterPageSelectors";
        }
    }

    public class DataViewSelectorIdentifier : Identifier
    {
        [XmlIgnore]
        public bool? FilteringEnabled { get; set; }

        [XmlAttribute("filtering-enabled")]
        public string FilteringEnabledSerializable { get { return FilteringEnabled.ToString(); } set { FilteringEnabled = value.ToNullableBool(); } }

        //public override bool Match(object obj, MatchContext context = null)
        //{
        //    if (FilteringEnabled.HasValue == true && context != null && FilteringEnabled.Value != (context.SiteContext.FilterManager != null && context.SiteContext.FilterManager.Enabled == true)) return false;

        //    return base.Match(obj, context);
        //}
    }

    public abstract class DataViewSelector : IMerge<DataViewSelector>, ICopyable<DataViewSelector>
    {
        protected DataViewSelector()
        {
            Id = Guid.NewGuid().ToString();
        }

        protected DataViewSelector(DataViewSelector prototype)
        {
            Id = Guid.NewGuid().ToString();
            MergeWith(prototype);
        }

        protected DataViewSelector(IEnumerable<DataViewSelector> prototypes)
        {
            Id = Guid.NewGuid().ToString();
            MergeWith(prototypes);
        }

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("class")]
        public string ClassName { get; set; }

        [XmlIgnore]
        public bool? CreateNew { get; set; }

        [XmlAttribute("create-new")]
        public string CreateNewSerializable { get { return CreateNew.ToString(); } set { CreateNew = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? Ignore { get; set; }

        [XmlAttribute("ignore")]
        public string IgnoreSerializable { get { return Ignore.ToString(); } set { Ignore = value.ToNullableBool(); } }

        [XmlAttribute("order")]
        public float Order { get; set; }

        [XmlAttribute("label")]
        public string Label { get; set; }

        [XmlAttribute("icon")]
        public string Icon { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("tool-tip")]
        public string ToolTip { get; set; }

        [XmlElement("Trigger")]
        public Identifier[] Triggers { get; set; }

        public bool IsAvailable(object triggerMatchObject = null, MatchContext triggerMatchContext = null)
        {
            return Ignore != true && (Triggers == null || Triggers.Length == 0 || Triggers.Any(t => t.Match(triggerMatchObject, triggerMatchContext)) == true);
        }

        public abstract DataViewSelector Copy(bool clone = false);

        public void MergeWith(IEnumerable<DataViewSelector> others)
        {
            foreach (var other in others)
            {
                MergeWith(other);
            }
        }

        public virtual void MergeWith(DataViewSelector other, IMerger<DataViewSelector> merger = null)
        {
            if (other.Ignore.HasValue == true) Ignore = other.Ignore;
            if (other.Order != 0) Order = other.Order;
            if (other.Label != null) Label = other.Label;
            if (other.Icon != null) Icon = other.Icon;
            if (other.Url != null) Url = other.Url;
            if (other.ToolTip != null) ToolTip = other.ToolTip;
            if (other.CreateNew.HasValue == true) CreateNew = other.CreateNew;
            if (other.ClassName != null) ClassName = other.ClassName;
        }
    }

    public class DataViewSortSelector : DataViewSelector
    {
        public DataViewSortSelector() { AlternateDirection = true; }

        public DataViewSortSelector(string id) { Id = id; AlternateDirection = true; }

        public DataViewSortSelector(DataViewSortSelector prototype) : base(prototype) { AlternateDirection = true; }

        public DataViewSortSelector(IEnumerable<DataViewSortSelector> prototypes) : base(prototypes) { AlternateDirection = true; }

        //public string SortProperty { get; set; }

        public bool? AlternateDirection { get; set; }

        public Sort Sort { get; set; }

        [XmlAttribute("sort")]
        public string SortShorthand { get { return Sort != null ? Sort.ToString() : null; } set { Sort = Sort.Parse(value); } }

        [XmlAttribute("property")]
        public string Type
        {
            get { return Sort != null ? Sort.Type : null; }
            set
            {
                if (value != null)
                {
                    if (Sort == null) Sort = new Sort();
                    Sort.Type = value;
                }
            }
        }

        [XmlAttribute("direction")]
        public SortDirection Direction
        {
            get { return Sort != null ? Sort.Direction : SortDirection.Ascending; }
            set
            {
                if (Sort == null) Sort = new Sort();
                Sort.Direction = value;
            }
        }

        public override DataViewSelector Copy(bool clone = false)
        {
            var copy = new DataViewSortSelector(this);
            if (clone == true) copy.Id = Id;
            return copy;
        }

        public override void MergeWith(DataViewSelector other, IMerger<DataViewSelector> merger = null)
        {
            base.MergeWith(other, merger);

            var otherSort = other as DataViewSortSelector;
            if (otherSort != null)
            {
                if (otherSort.AlternateDirection != null) AlternateDirection = otherSort.AlternateDirection;
                if (otherSort.Sort != null) Sort = otherSort.Sort;
            }
        }
    }

    public class DataViewViewSelector : DataViewSelector
    {
        public DataViewViewSelector() { }

        public DataViewViewSelector(string id) { Id = id; }

        public DataViewViewSelector(DataViewSelector prototype) : base(prototype) { }

        public DataViewViewSelector(IEnumerable<DataViewSelector> prototypes) : base(prototypes) { }

        public override DataViewSelector Copy(bool clone = false)
        {
            var copy = new DataViewViewSelector(this);
            if (clone == true) copy.Id = Id;
            return copy;
        }
    }

    public class DataViewFilterSelector : DataViewSelector
    {
        public DataViewFilterSelector() { }

        public DataViewFilterSelector(string id) { Id = id; }

        public DataViewFilterSelector(DataViewFilterSelector prototype) : base(prototype) { }

        public DataViewFilterSelector(IEnumerable<DataViewFilterSelector> prototypes) : base(prototypes) { }

        [XmlAttribute("property")]
        public string Property { get { return valueProvider is ValueProvider ? ((ValueProvider)valueProvider).PropertyName : null; } set { valueProvider =MultivendorWebViewer.Common.ValueProvider.CreateProperty(value); } }

        [XmlElement("Format")]
        public Formatter Format { get { return valueProvider is ValueProvider ? ((ValueProvider)valueProvider).Format : null; } set { if (valueProvider is ValueProvider) { ((ValueProvider)ValueProvider).Format = value; } else if (valueProvider == null) { valueProvider = new ValueProvider { Format = value }; } } }
        //public List<Formatter> Formatters { get; set; }

        private ValueProviderBase valueProvider;
        [XmlElement("Value", Type = typeof(ValueProvider))]
        public ValueProviderBase ValueProvider
        {
            get { return valueProvider; }
            set
            {
                valueProvider = value;
                ExplicitValueProvider = true;
            }
        }

        [XmlIgnore]
        public bool ExplicitValueProvider { get; private set; }

        public override DataViewSelector Copy(bool clone = false)
        {
            var copy = new DataViewFilterSelector(this);
            if (clone == true) copy.Id = Id;
            return copy;
        }

        public override void MergeWith(DataViewSelector other, IMerger<DataViewSelector> merger = null)
        {
            base.MergeWith(other, merger);

            var otherFilter = other as DataViewFilterSelector;
            if (otherFilter != null)
            {
                if (otherFilter.ValueProvider != null) ValueProvider = otherFilter.ValueProvider;
                //if (otherFilter.Property != null) Property = otherFilter.Property;
                //if (otherFilter.Formatters != null) Formatters = new List<Formatter>(otherFilter.Formatters);
            }
        }
    }
}