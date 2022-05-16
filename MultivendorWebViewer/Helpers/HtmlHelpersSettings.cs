using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.ViewModels;
using System.Xml.Serialization;
using MultivendorWebViewer.Common;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Ajax.Utilities;
using MultivendorWebViewer.Configuration;

namespace MultivendorWebViewer.Helpers
{
    public class MenuSettings
    {
        static MenuSettings()
        {
            Default = new MenuSettings();
        }

        public MenuSettings()
        {
            Hottrack = true;

            SelectionMode = ListSelectMode.None;
        }

        public bool CloseButton { get; set; }

        public bool DropdownSelectButton { get; set; }
        public bool Hottrack { get; set; }

        public ListSelectMode SelectionMode { get; set; }

        public bool? DisplaySelect { get; set; }

        //public bool Check { get; set; }

        //public bool CheckOnClick { get; set; }

        //public IconDescriptor CheckIcon { get; set; }

        public static MenuSettings Default { get; private set; }
    }

    //public class ToolBarSettings
    //{
    //    public ToolBarSettings()
    //    {
    //        Direction = LayoutDirection.Horizontal;
    //        ContentDirection = LayoutDirection.Vertical;
    //        ShowToolTip = true;
    //        Size = ToolBarSize.Medium;
    //    }

    //    public bool? DisplayLabels { get; set; }

    //    public bool? DisplayContent { get; set; }

    //    public LayoutDirection Direction { get; set; }

    //    public LayoutDirection ContentDirection { get; set; }

    //    public bool Compact { get; set; }

    //    public ToolBarSize Size { get; set; }

    //    public bool Highlight { get; set; }

    //    public bool ShowToolTip { get; set; }

    //    public bool NewLayout { get; set; }

    //    static ToolBarSettings()
    //    {
    //        ToolBarSettings.Default = new ToolBarSettings();
    //    }

    //    public static ToolBarSettings Default { get; set; }
    //}

    //public enum ToolBarSize { NotSet, Small, Medium, Large }

    //public enum ToolBarAlignment { Left, Right, Center, NotSet }

    //[Flags]
    //public enum CheckType { None = 0x0, Normal = 0x1, Toggle = 0x2, Radio = 0x4 }

    public enum ListSelectMode { None, Single, Multi }

    //public class DataViewFacetSetting : DataViewSelector
    //{
    //    public DataViewFacetSetting() { }

    //    public DataViewFacetSetting(string id) { Id = id; }

    //    public DataViewFacetSetting(DataViewFacetSetting prototype) : base(prototype) { }

    //    public DataViewFacetSetting(IEnumerable<DataViewFacetSetting> prototypes) : base(prototypes) { }

    //    [XmlAttribute("visibility")]
    //    public PaneDisplayMode Visibility { get; set; }

    //    [XmlAttribute("unit")]
    //    public string UnitKey { get; set; }

    //    [XmlAttribute("input-type")]
    //    public string InputType { get; set; }

    //    public Formatter InputMinMaxFormatter { get; set; }

    //    [XmlAttribute("input-min-max-format")]
    //    public string FormatShorthand { get { return InputMinMaxFormatter != null ? InputMinMaxFormatter.Format : null; } set { InputMinMaxFormatter = Formatter.Create(value); } }

    //    [XmlAttribute("input-min")]
    //    public string InputMin { get; set; }

    //    [XmlAttribute("input-max")]
    //    public string InputMax { get; set; }

    //    [XmlAttribute("input-pattern")]
    //    public string InputPattern { get; set; }

    //    [XmlAttribute("date-parse-format")]
    //    public string DateParseFormat { get; set; }

    //    public override DataViewSelector Copy(bool clone = false)
    //    {
    //        var copy = (DataViewFacetSetting)MemberwiseClone();
    //        copy.MergeWith(this);
    //        if (clone == true) copy.Id = Id;

    //        return copy;
    //    }

    //    public override void MergeWith(DataViewSelector other, IMerger<DataViewSelector> merger = null)
    //    {
    //        base.MergeWith(other, merger);

    //        var otherSettings = other as DataViewFacetSetting;
    //        if (otherSettings != null)
    //        {
    //            if (otherSettings.Visibility != PaneDisplayMode.NotSet) Visibility = otherSettings.Visibility;
    //            if (otherSettings.UnitKey != null) UnitKey = otherSettings.UnitKey;
    //            if (otherSettings.InputType != null) InputType = otherSettings.InputType;
    //            if (otherSettings.InputMinMaxFormatter != null) InputMinMaxFormatter = otherSettings.InputMinMaxFormatter;
    //            if (otherSettings.InputMin != null) InputMin = otherSettings.InputMin;
    //            if (otherSettings.InputMax != null) InputMax = otherSettings.InputMax;
    //            if (otherSettings.InputPattern != null) InputPattern = otherSettings.InputPattern;
    //        }
    //    }
    //}

    //public class DataViewRangeFacetSetting : DataViewFacetSetting
    //{
    //    /// <summary>
    //    /// Gets the value from the item that is the lower bounds of the range that the selection value or range intersect (in overlapped by)
    //    /// </summary>
    //    [XmlElement("From")]
    //    public ValueProvider ItemFrom { get; set; }

    //    /// <summary>
    //    /// Gets the value from the item that is the upper bounds of the range that the selection value or range intersect (in overlapped by)
    //    /// </summary>
    //    [XmlElement("To")]
    //    public ValueProvider ItemTo { get; set; }

    //    /// <summary>
    //    /// Gets or sets if the selection should be a range, otherwise a single value
    //    /// </summary>
    //    [XmlIgnore]
    //    public bool? RangeSelection { get; set; }

    //    [XmlAttribute("range-selection")]
    //    public string RangeSelectionSerializable { get { return RangeSelection.ToString(); } set { RangeSelection = value.ToNullableBool(); } }

    //    /// <summary>
    //    /// Gets the value(s) from the item to compare agains the selecttion range
    //    /// </summary>
    //    [XmlElement("Value")]
    //    public ValueProvider[] ItemValues { get; set; }

    //    /// <summary>
    //    /// Gets or sets the Value(Provider) PropertyName shorthand
    //    /// </summary>
    //    [XmlAttribute("property")]
    //    public string PropertyName
    //    {
    //        get { return ItemValues != null && ItemValues.Length > 0 ? ItemValues[0].PropertyName : null; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                ItemValues = null;
    //            }
    //            else
    //            {
    //                ItemValues = new ValueProvider[] { ValueProvider.CreateProperty(PropertyName) };
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Gets or sets the Value(Provider) Specification.Code shorthand
    //    /// </summary>
    //    [XmlAttribute("specification")]
    //    public string Specification
    //    {
    //        get { return ItemValues != null && ItemValues.Length > 0 ? ItemValues[0].PropertyName : null; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                ItemValues = null;
    //            }
    //            else
    //            {
    //                ItemValues = new ValueProvider[] { new ValueProvider { Specification = new SpecificationIdentifier { Code = value } } };
    //            }
    //        }
    //    }




    //    /// <summary>
    //    /// Gets or sets if all values needs to be in the selected range. This is only effective if there are multiple Value.
    //    /// </summary>
    //    public bool? AllValuesInRange { get; set; }

    //    [XmlIgnore]
    //    public bool? AutoGeneratedRange { get; set; }

    //    [XmlAttribute("auto-range")]
    //    public string AutoGeneratedRangeSerializable { get { return AutoGeneratedRange.ToString(); } set { AutoGeneratedRange = value.ToNullableBool(); } }

    //    /// <summary>
    //    /// Gets or sets the valid range values. That is the selection can ony consist of those values
    //    /// </summary>
    //    [XmlIgnore]
    //    public bool? ExplicitRangeValues { get; set; }


    //    [XmlAttribute("explicit-range-values")]
    //    public string ExplicitRangeValuesSerializable { get { return ExplicitRangeValues.ToString(); } set { ExplicitRangeValues = value.ToNullableBool(); } }


    //    /// <summary>
    //    /// Gets or sets the step of the range. This is not used yet.
    //    /// </summary>
    //    [XmlAttribute("range-step")]
    //    public string RangeValueStep { get; set; }

    //    /// <summary>
    //    /// Gets or sets the selectable values. The item value need to be equal one of the values in this collection.
    //    /// </summary>
    //    [XmlIgnore]
    //    public string[] SelectableValues { get; set; }

    //    [XmlAttribute("selectable-values")]
    //    public string SelectableValuesSerializable { get { return string.Join(",", SelectableValues); } set { SelectableValues = value != null ? value.Split(',', '|', ' ') : null; } }

    //    /// <summary>
    //    /// Gets or sets the formatted value of the selectable values. This is displayed. If the collection is empty or if different length as the selectable values, the selected values is used instead.
    //    /// </summary>
    //    [XmlIgnore]
    //    public string[] FormattedSelectableValues { get; set; }

    //    [XmlAttribute("selectable-values-labels")]
    //    public string FormattedSelectableValuesSerializable { get { return string.Join(",", FormattedSelectableValues); } set { FormattedSelectableValues = value != null ? value.Split(',', '|') : null; } }


    //    public override void MergeWith(DataViewSelector other, IMerger<DataViewSelector> merger = null)
    //    {
    //        base.MergeWith(other, merger);

    //        var otherSettings = other as DataViewRangeFacetSetting;
    //        if (otherSettings != null)
    //        {
    //            if (otherSettings.ItemFrom != null) ItemFrom = otherSettings.ItemFrom;
    //            if (otherSettings.ItemTo != null) ItemTo = otherSettings.ItemTo;
    //            if (otherSettings.RangeSelection != null) RangeSelection = otherSettings.RangeSelection;

    //            if (otherSettings.ItemValues != null) ItemValues = otherSettings.ItemValues;
    //            if (otherSettings.AllValuesInRange != null) AllValuesInRange = otherSettings.AllValuesInRange;
    //            if (otherSettings.AutoGeneratedRange != null) AutoGeneratedRange = otherSettings.AutoGeneratedRange;
    //            if (otherSettings.ExplicitRangeValues != null) ExplicitRangeValues = otherSettings.ExplicitRangeValues;
    //            if (otherSettings.RangeValueStep != null) RangeValueStep = otherSettings.RangeValueStep;
    //        }
    //    }
    //}

    //public class DataViewSerialNumberFacetSetting : DataViewFacetSetting
    //{
    //    public DataViewSerialNumberFacetSetting()
    //    {

    //    }

    //    public string SerialNumberLabel { get; set; }

    //    public string Group1Label { get; set; }

    //    public string Group2Label { get; set; }

    //    public string Group3Label { get; set; }

    //    public string Group4Label { get; set; }


    //    public bool? DisplaySerialNumber { get; set; }

    //    public bool? DisplayGroup1 { get; set; }

    //    public bool? DisplayGroup2 { get; set; }

    //    public bool? DisplayGroup3 { get; set; }

    //    public bool? DisplayGroup4 { get; set; }

    //    public LabelDisplayMode? LabelDisplayMode { get; set; }

    //    public override void MergeWith(DataViewSelector other, IMerger<DataViewSelector> merger = null)
    //    {
    //        base.MergeWith(other, merger);

    //        var otherSettings = other as DataViewSerialNumberFacetSetting;
    //        if (otherSettings != null)
    //        {
    //            if (otherSettings.SerialNumberLabel != null) SerialNumberLabel = otherSettings.SerialNumberLabel;
    //            if (otherSettings.Group1Label != null) Group1Label = otherSettings.Group1Label;
    //            if (otherSettings.Group2Label != null) Group2Label = otherSettings.Group2Label;
    //            if (otherSettings.Group3Label != null) Group3Label = otherSettings.Group3Label;
    //            if (otherSettings.Group4Label != null) Group4Label = otherSettings.Group4Label;

    //            if (otherSettings.DisplaySerialNumber.HasValue == true) DisplaySerialNumber = otherSettings.DisplaySerialNumber.Value;
    //            if (otherSettings.DisplayGroup1.HasValue == true) DisplayGroup1 = otherSettings.DisplayGroup1.Value;
    //            if (otherSettings.DisplayGroup2.HasValue == true) DisplayGroup2 = otherSettings.DisplayGroup2.Value;
    //            if (otherSettings.DisplayGroup3.HasValue == true) DisplayGroup3 = otherSettings.DisplayGroup3.Value;
    //            if (otherSettings.DisplayGroup4.HasValue == true) DisplayGroup4 = otherSettings.DisplayGroup4.Value;
    //            if (otherSettings.LabelDisplayMode.HasValue == true) LabelDisplayMode = otherSettings.LabelDisplayMode.Value;
    //        }
    //    }
    //}

    [Flags]
    public enum LabelDisplayMode { None = 0x0, Label = 0x1, Placeholder = 0x2 };

    [Flags]
    public enum PaginationPageSelectorTypes { None = 0x0, Previous = 0x1, Next = 0x2, First = 0x4, Last = 0x8, Index = 0x10, All = Previous | Next | First | Last | Index, NotSet = ~All }

    public class ElementSelector
    {
        [JsonProperty("parent")]
        public string ParentSelector { get; set; }

        [JsonProperty("closest")]
        public string ClosestSelector { get; set; }

        [JsonProperty("find")]
        public string Selector { get; set; }

        public bool IsEmpty { get { return Selector == null && ParentSelector == null && ClosestSelector == null; } }

        public static ElementSelector Find(string selector, string parentSelector = null, string closestSelector = null)
        {
            return new ElementSelector { Selector = selector, ParentSelector = null, ClosestSelector = closestSelector };
        }

        public static ElementSelector FindById(string id)
        {
            return new ElementSelector { Selector = "#" + id };
        }

        public static ElementSelector FindByClass(string className)
        {
            return new ElementSelector { Selector = "." + className };
        }

        public ElementSelector Parent(string selector)
        {
            ParentSelector = selector;
            return this;
        }

        public ElementSelector ParentById(string id)
        {
            return Parent("#" + id);
        }

        public ElementSelector ParentByClass(string className)
        {
            return Parent("." + className);
        }

        public ElementSelector Closest(string selector)
        {
            ClosestSelector = selector;
            return this;
        }

        public ElementSelector ClosestById(string id)
        {
            return Closest("#" + id);
        }

        public ElementSelector ClosestByClass(string className)
        {
            return Closest("." + className);
        }
    }

    public class PaginationPageSelector
    {
        public string Id { get; set; }

        public PaginationPageSelectorTypes Type { get; set; }

        public string Label { get; set; }

        public string Icon { get; set; }

        public int Index { get; set; }

        public string Url { get; set; }
    }

    public class PaginationPageSize
    {
        public string Id { get; set; }

        public string Label { get; set; }

        public string Icon { get; set; }

        public int Count { get; set; }

        public string Url { get; set; }

        public static implicit operator PaginationPageSize(int size)
        {
            return new PaginationPageSize { Count = size, Label = size.ToString() };
        }

        public static PaginationPageSize[] Create(IEnumerable<int> sizes)
        {
            if (sizes == null) return null;
            return sizes.Select(s => new PaginationPageSize { Count = s, Label = s.ToString() }).ToArray();
        }

        public static PaginationPageSize[] Create(string sizes)
        {
            return PaginationPageSize.Create(sizes, null);
        }

        public static PaginationPageSize[] Create(string sizes, char[] separators)
        {
            if (sizes == null) return null;
            if (separators == null) separators = new[] { ',', ';', ' ' };
            string[] stringSizes = sizes.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var intSizes = stringSizes.Select(s => s.ToNullableInt()).Where(s => s != null).Select(i => i.Value);
            return PaginationPageSize.Create(intSizes);
        }
    }

    public class PaginationSelection
    {
        [JsonProperty("page")]
        public int? PageIndex { get; set; }

        [JsonProperty("pageId")]
        public string PageId { get; set; }

        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }

        [JsonProperty("pageSizeId")]
        public string PageSizeId { get; set; }

        [JsonProperty("pageSizes")]
        public List<int> PageSizes { get; set; }
    }

    public interface IDataViewQueryFilterSelection
    {
        string Query { get; }

        List<DataViewFilterSelectionProperty> Properties { get; }

        //List<DataViewFilterSelector> FilterSelectors { get; }
    }

    public class DataViewFilterSelection : IDataViewQueryFilterSelection
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("properties")]
        public List<DataViewFilterSelectionProperty> Properties { get; set; }

        //[JsonProperty("facets")]
        //public List<DataViewFilterFacetSelection> Facets { get; set; }
        //public List<SearchFacetSelection> Facets { get; set; }

        //public SearchFacetSelection GetFacetSelection(string id)
        //{
        //    return Facets != null ? Facets.Where(f => StringComparer.OrdinalIgnoreCase.Equals(f.Id, id) == true).FirstOrDefault() : null;
        //}
    }

    public class DataViewFilterSelectionProperty
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("property")]
        public string Property { get; set; }

        [JsonIgnore]
        public bool Skip { get; set; }

        [JsonIgnore]
        public bool ExplicitValueProvider { get; set; }

        [JsonIgnore]
        public ValueProviderBase ValueProvider { get; set; }
    }

    public class TableReportColumn 
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("header")]
        public string Header { get; set; }

        [XmlAttribute("property")]
        public string PropertyName { get; set; }

        [XmlElement("Format")]
        public FormatterContext Formatter { get; set; }

        [XmlElement("Value", Type = typeof(ValueProvider))]
        public ValueProviderBase ValueProvider { get; set; }

        [XmlIgnore]
        public virtual Func<object, string> FormattedValueProviderWrapper { get; set; }
    }

    public interface IToolBarItem
    {

    }

    //public enum IconSize { Default, S50, S110, S125, S133, S150, S200, S300, S400, S500 };

    //public class MultiIconDescriptor : IconDescriptor
    //{
    //    public MultiIconDescriptor(IEnumerable<IconDescriptor> icons)
    //    {
    //        Icons = icons.ToArray();
    //    }

    //    public ICollection<IconDescriptor> Icons { get; set; }

    //}

    //[XmlInclude(typeof(IcomoonIconDescriptor))]
    //[XmlInclude(typeof(GlyphIconDescriptor))]
    //[XmlInclude(typeof(FontAwesomeIconDescriptor))]
    //[XmlInclude(typeof(MaterialIconDescriptor))]
    //public class IconDescriptor
    //{
    //    public IconDescriptor() { }

    //    protected IconDescriptor(string id, string name)
    //    {
    //        Id = id;
    //        Name = name;
    //    }

    //    protected IconDescriptor(params IconDescriptor[] prototype)
    //    {
    //        Merge(prototype);
    //    }


    //    [XmlAttribute("id")]
    //    public string Id { get; set; }

    //    //public string Context { get; private set; }

    //    [XmlAttribute("name")]
    //    public string Name { get; set; }

    //    [XmlAttribute("InnerText")]
    //    public string InnerText { get; set; }

    //    [XmlAttribute("css")]
    //    public string CustomCss { get; set; }

    //    public void Merge(params IconDescriptor[] others)
    //    {
    //        others.ForEach(o => Merge(o));
    //    }

    //    public virtual void Merge(IconDescriptor other)
    //    {
    //        if (Name == null) Name = other.Name;
    //        if (CustomCss == null) CustomCss = other.CustomCss;
    //    }
    //}

    //public class GlyphIconDescriptor : IconDescriptor
    //{
    //    public GlyphIconDescriptor() { }

    //    public GlyphIconDescriptor(string id, string name) : base(id, name) { }
    //}

    //public class MaterialIconDescriptor : IconDescriptor
    //{
    //    public MaterialIconDescriptor() { }

    //    public MaterialIconDescriptor(string id, string name) : base(id, name)
    //    {
    //        //InnerText = Name;
    //    }
    //}

    //public class IcomoonIconDescriptor : IconDescriptor
    //{
    //    public IcomoonIconDescriptor() { }

    //    public IcomoonIconDescriptor(string id, string name) : base(id, name) { }
    //}


    //public class FontAwesomeIconDescriptor : IconDescriptor
    //{
    //    public FontAwesomeIconDescriptor() { }

    //    public FontAwesomeIconDescriptor(string id, string name, bool staticWidth = true, bool fontAwsome5 = false)
    //        : base(id, name)
    //    {
    //        //StaticWidth = staticWidth;
    //        FontAwsome5 = fontAwsome5;
    //    }

    //    [XmlElement("StaticWidth")]
    //    public bool? StaticWidth { get; set; }

    //    [XmlElement("FontAwsome5")]
    //    public bool? FontAwsome5 { get; set; }
    //}

}