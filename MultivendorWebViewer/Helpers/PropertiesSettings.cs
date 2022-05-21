using MultivendorWebViewer.Common;
using MultivendorWebViewer.Collections;
using MultivendorWebViewer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using MultivendorWebViewer.Helpers;

namespace MultivendorWebViewer.Configuration
{
    public enum MissingValueMode { SkipValue, SkipProperty, SkipNothing }
    public class PropertiesSettings 
    {
        public PropertiesSettings()
        {
            //Id = string.Empty;
            Properties = new PropertyDescriptorSettingsDictionary();
        }

        //[XmlAttribute("id")]
        //public string Id { get; set; }

        [XmlElement("Property")]
        public PropertyDescriptorSettingsDictionary Properties { get; set; }

        /// <summary>
        /// Display specifications. Default is true.
        /// </summary>
        /// <value>Element name DisplaySpecifications</value>
        [XmlIgnore]
        public bool? DisplaySpecifications { get; set; }

        [XmlAttribute("display-specifications")]
        public string DisplaySpecificationsSerializeable { get { return DisplaySpecifications.ToString(); } set { DisplaySpecifications = value.ToNullableBool(); } }

        /// <summary>
        /// The number of specifications when the specifications gets collapsed.
        /// </summary>
        /// <value>Element name CollapseSpecificationsCount</value>
        [XmlIgnore]
        public int? CollapseCount { get; set; }

        [XmlAttribute("collapse-count")]
        public string CollapseCountSerializeable { get { return CollapseCount.ToString(); } set { CollapseCount = value.ToNullableInt(); } }


        /// <summary>
        ///  Based on the count , text 'Show all' would appear for as label the specifications.
        /// </summary>
        [XmlIgnore]
        public int? ShowAllCount { get; set; }

        [XmlAttribute("show-all-count")]
        public string ShowAllCountSerializeable { get { return ShowAllCount.ToString(); } set { ShowAllCount = value.ToNullableInt(); } }



        /// <summary>
        /// Display labels for each specification were applicable. Default is true.
        /// </summary>
        /// <value>Element name DisplaySpecificationsLabels</value>
        [XmlIgnore]
        public bool? DisplayPropertyLabels { get; set; }

        [XmlAttribute("display-labels")]
        public string DisplayPropertyLabelsSerializeable { get { return DisplayPropertyLabels.ToString(); } set { DisplayPropertyLabels = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? AutoSizeProperties { get; set; }

        [XmlAttribute("auto-size-properties")]
        public string AutoSizePropertiesSerializeable { get { return AutoSizeProperties.ToString(); } set { AutoSizeProperties = value.ToNullableBool(); } }

        [XmlIgnore]
        public MissingValueMode? MissingValueMode { get; set; }

        [XmlAttribute("missing-value-mode")]
        public string MissingValueModeSerializeable { get { return MissingValueMode.ToString(); } set { MissingValueMode = Utility.ParseNullableEnum<MissingValueMode>(value); } }

        [XmlIgnore]
        public bool? ExcludeCategories { get; set; }

        [XmlAttribute("exclude-categories")]
        public string ExcludeCategoriesSerializeable { get { return ExcludeCategories.ToString(); } set { ExcludeCategories = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? ExcludeSubCategories { get; set; }

        [XmlAttribute("exclude-subcategories")]
        public string ExcludeSubCategoriesSerializeable { get { return ExcludeSubCategories.ToString(); } set { ExcludeSubCategories = value.ToNullableBool(); } }

        [XmlIgnore]
        public LayoutDirection? LayoutDirection { get; set; }

        [XmlAttribute("layout-direction")]
        public string LayoutDirectionSerializeable { get { return LayoutDirection.ToString(); } set { LayoutDirection = Utility.ParseNullableEnum<LayoutDirection>(value); } }

        /// <summary>
        /// Separator to join the values, if there are more than one value.
        /// </summary>
        /// <value>Attribute name separator</value>
        [XmlAttribute("separator")]
        public string HtmlSeparator { get; set; }
        protected string id;
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
            }
        }
    }

    public class PropertiesSettingsDictionary : ObjectDictionary<PropertiesSettings>, IMerge<IEnumerable<PropertiesSettings>>
    {
        public PropertiesSettingsDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
            DefaultSettings = new PropertiesSettings { };
        }

        public PropertiesSettingsDictionary(IEnumerable<PropertiesSettings> settings)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            DefaultSettings = new PropertiesSettings {};
            AddRange(settings);
        }

        public PropertiesSettings DefaultSettings { get; set; }

        protected override string GetKeyForItem(PropertiesSettings item)
        {
            return item.Id ?? string.Empty;
        }

        public void MergeWith(IEnumerable<PropertiesSettings> item, IMerger<IEnumerable<PropertiesSettings>> merger = null)
        {
            AddRange(item);
        }

     
    }

    /// <summary>
    /// Settings for properties in the html. E.g. the properties of a presentation.
    /// </summary>
    public class PropertyDescriptorSettings 
    {
        public PropertyDescriptorSettings()
        {
            ValueProviders = new List<ValueProvider>();
           
        }

        [XmlAttribute("new")]
        public bool New { get; set; }

        [XmlAttribute("hide-empty")]
        public bool HideEmpty { get; set; } = true;
        protected string id;
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
            }
        }


        /// <summary>
        /// The id to match the property this settings should be applied to.
        /// </summary>
        //[XmlAttribute("id")]
        //public string Id { get; set; }


        //[XmlAttribute("format")]
        //public string FormatId { get { return Format != null ? Format.Id : null; } set { Format = Formatter.GetNamedInstance(value); } }

        /// <summary>
        /// Formatting of the properties value.
        /// </summary>
        /// <value>Element name Format</value>
        public Formatter Format { get; set; }

        /// <summary>
        /// Value providers for getting the values for a row cell in the column.
        /// </summary>
        /// <remarks>Multiple.</remarks>
        /// <value>Element name Value</value>
        /// <see cref="ValueProvider"/>
        [XmlElement("Value")]
        public List<ValueProvider> ValueProviders { get; private set; }

        [XmlAttribute("text")]
        public string TextShorthand
        {
            get { return ValueProviders.OfType<TextValueProvider>().Select(v => v.Text).FirstOrDefault(); }
            set
            {
                var vp = ValueProvider.CreateText(value);
                ValueProviders.RemoveRange(ValueProviders.OfType<TextValueProvider>().ToArray());
                if (vp != null) ValueProviders.Add(vp);
            }
        }

        //[XmlAttribute("specification")]
        //public string SpecificationValueShorthand
        //{
        //    get { return string.Join(",", ValueProviders.Where(v => v.Specification != null).Select(v => v.Specification.Code)); }
        //    set
        //    {
        //        ValueProviders.RemoveRange(ValueProviders.Where(v => v.Specification != null).ToArray());
        //        ValueProviders.AddRange(ValueProvider.CreateSpecifications(value));
        //    }
        //}

        /// <summary>
        /// The name of the template to use to render html content for a row cell in the column.
        /// </summary>
        /// <value>Attribute name template</value>
        [XmlAttribute("custom-view")]
        public string CustomView { get; set; }

        /// <summary>
        /// Separator to join the values, if there are more than one value.
        /// </summary>
        /// <value>Attribute name separator</value>
        [XmlAttribute("separator")]
        public string HtmlSeparator { get; set; }

        //[XmlAttribute("order")]
        //public float Order { get; set; }

        public PropertyDescriptorSettings Copy()
        {
            var copy = new PropertyDescriptorSettings();
            //copy.MergeWith(this);
            return copy;
        }

        //protected override void MergeWithCore(ComponentSettings other)
        //{
        //    base.MergeWithCore(other);

        //    var o = other as PropertyDescriptorSettings;
        //    if (o != null)
        //    {
        //        New = o.New;
        //        HideEmpty = o.HideEmpty;
        //        if (o.Format != null) Format = o.Format;
        //        if (o.ValueProviders != null && o.ValueProviders.Count > 0) ValueProviders = o.ValueProviders;
        //        if (o.CustomView != null) CustomView = o.CustomView;
        //        if (o.HtmlSeparator != null) HtmlSeparator = o.HtmlSeparator;
        //    }
        //}

    }

    public class PropertyDescriptorSettingsDictionary : DictionaryCollection<string, PropertyDescriptorSettings>, ICollection<PropertyDescriptorSettings>
    {
        public PropertyDescriptorSettingsDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        protected override string GetKeyForItem(PropertyDescriptorSettings item)
        {
            return item.Id;
        }

        public void Merge(IEnumerable<PropertyDescriptorSettings> settings)
        {
            foreach (var setting in settings)
            {
                AddOrReplace(setting);
            }
        }

        private bool? isAnyHidden;
        public bool IsAnyHidden { get { return isAnyHidden ?? (bool)(isAnyHidden =false /*Items.Any(i => i.IsHidden == true)*/); } }

        protected override void InsertItem(int index, PropertyDescriptorSettings item)
        {
            string key = GetKeyForItem(item);

            if (key != null && key.IndexOf(',') != -1) // special case, normal case need to be as fast as possible
            {
                string[] multiKeys = key.Split(',');
                if (multiKeys.Length == 1)
                {
                    var copy = item.Copy();
                    copy.Id = multiKeys[0];
                    base.InsertItem(index, copy);
                    for (int i = 1; i < multiKeys.Length; i++)
                    {
                        copy = item.Copy();
                        copy.Id = multiKeys[i];
                        Items.Add(copy);
                    }
                    return;
                }
            }

            base.InsertItem(index, item);

            isAnyHidden = null;
        }
    }
}