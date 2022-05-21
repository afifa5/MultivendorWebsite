using MultivendorWebViewer.Common;
using MultivendorWebViewer.Helpers;
using MultivendorWebViewer.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Configuration
{
    /// <summary>
    /// Settings for a table column. The columns to apply the settings on are matched by id.
    /// </summary>
    [XmlInclude(typeof(TableCustomColumnSettings))]
    public class TableColumnSettings
    {
        public TableColumnSettings()
        {
            Enabled = true;
            Order = int.MinValue;
            CellSettings = new TriggerableObjectList<TableCellSettings>();
        }

        /// <summary>
        /// The id of the columns to add settings for.
        /// </summary>
        /// <value>Attribute name id</value>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// Caption text key.
        /// </summary>
        /// <value>Attribute name caption-key</value>
        [XmlAttribute("caption-key")]
        public string CaptionKey { get; set; }

        /// <summary>
        /// Set to false to exclude the column. Default true.
        /// </summary>
        /// <value>Attribute name enabled</value>
        [XmlAttribute("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// The index of the columns. Left to right.
        /// </summary>
        /// <value>Attribute name order</value>
        [XmlAttribute("order")]
        public int Order { get; set; }

        public bool? HideEmpty { get; set; }

        [XmlAttribute("hide-empty")]
        /// <summary>
        /// Set to true to hide the column automatically of the whole column is emtpy.
        /// </summary>
        /// <value>Element name HideEmpty</value>
        public string HideEmptySerializable { get { return HideEmpty.ToString(); } set { HideEmpty = value.ToNullableBool(); } }

        /// <summary>
        /// Comma separated list of class names added to the column.
        /// </summary>
        /// <value>Attribute name class</value>
        [XmlAttribute("class")]
        public string ClassNames { get; set; }

        //[XmlArrayItem("display-mode")]
        //public ValueDisplayMode DisplayMode { get; set; }

        /// <summary>
        /// Cell settings for the column. Triggerable. Multiple.
        /// </summary>
        /// <value>Element name CellSettings</value>
        /// <see cref="TableCellSettings"/>
        [XmlElement("CellSetting")]
        public TriggerableObjectList<TableCellSettings> CellSettings { get; private set; }
    }

    public class TableColumnSettingsDictionary<TIdentifier> : TriggerableObjectDictionary<string, TableCustomColumnSettings, TIdentifier>
        where TIdentifier : Identifier
    {
        public TableColumnSettingsDictionary() : base(StringComparer.OrdinalIgnoreCase) { }

        public IEnumerable<TableCustomColumnSettings> AvailableColumns { get { return Items.Where(c => c.Enabled == true); } }

        protected override string GetKeyForItem(Trigger<TableCustomColumnSettings, TIdentifier> item)
        {
            return item.Item.Id;
        }

        protected override Trigger<TableCustomColumnSettings, TIdentifier> ResolveConflictingItem(Trigger<TableCustomColumnSettings, TIdentifier> exisitingItem, Trigger<TableCustomColumnSettings, TIdentifier> newItem)
        {
            var item = base.ResolveConflictingItem(exisitingItem, newItem);
            var resolvedColumn = item.Item;
            var existingColumn = exisitingItem.Item;

            if (resolvedColumn.CaptionKey == null) resolvedColumn.CaptionKey = existingColumn.CaptionKey;
            if (resolvedColumn.CellSettings.Count == 0) resolvedColumn.CellSettings.AddRange(existingColumn.CellSettings);
            if (resolvedColumn.ClassNames == null) resolvedColumn.ClassNames = existingColumn.ClassNames;
            if (resolvedColumn.HideEmpty.HasValue == true) resolvedColumn.HideEmpty = existingColumn.HideEmpty;
            if (resolvedColumn.Order == int.MinValue) resolvedColumn.Order = existingColumn.Order;

            if (resolvedColumn.Format == null) resolvedColumn.Format = existingColumn.Format;
            if (resolvedColumn.HtmlSeparator == null) resolvedColumn.HtmlSeparator = existingColumn.HtmlSeparator;
            if (resolvedColumn.Template == null) resolvedColumn.Template = existingColumn.Template;
            if (resolvedColumn.ValueProviders.Count == 0 && resolvedColumn.RemoveValueProviders == false) resolvedColumn.ValueProviders.AddRange(existingColumn.ValueProviders);

            return item;
        }
    }

    /// <summary>
    /// Settings for a table cell.
    /// </summary>
    public class TableCellSettings
    {
        /// <summary>
        /// Comma separated list of class names added to the cell.
        /// </summary>
        /// <value>Attribute name class</value>
        [XmlAttribute("class")]
        public string ClassNames { get; set; }

        //[XmlAttribute("hide-number-mode")]
        //public HidePartNumberMode HidePartNumberMode { get; set; }

        //[XmlAttribute("hidden-number-")]
        //public bool HiddenNumberInAttribute { get; set; }

        [XmlAttribute("format")]
        public string FormatId { get { return Formatter != null ? Formatter.Id : null; } set { Formatter = Formatter.GetNamedInstance(value); } }
        /// <summary>
        /// Formatter for formatting the value of the cell.
        /// </summary>
        /// <value>Element name Formatter</value>
        /// <see cref="Formatter"/>
        public Formatter Formatter { get; set; }

        /// <summary>
        /// Html attributes added to the cell. Multiple.
        /// </summary>
        /// <value>Element name Attribute</value>
        /// <see cref="HtmlAttribute"/>
        [XmlElement("Attribute")]
        public List<HtmlAttribute> Attributes { get; set; }
    }

    /// <summary>
    /// Defines a custom column for table.
    /// </summary>
    public class TableCustomColumnSettings : TableColumnSettings
    {
        public TableCustomColumnSettings()
        {
            ValueProviders = new List<ValueProvider>();
            Id = "Custom_" + (instances++).ToString();
        }

        public static TableCustomColumnSettings DisabledColumn { get; private set; }

        static TableCustomColumnSettings()
        {
            DisabledColumn = new TableCustomColumnSettings { Enabled = false };
        }

        private static int instances;

        [XmlIgnore]
        public bool? CreateNew { get; set; }

        [XmlAttribute("create-new")]
        public string CreateNewSerializable { get { return CreateNew.ToString(); } set { CreateNew = value.ToNullableBool(); } }

        /// <summary>
        /// Value providers for getting the values for a row cell in the column.
        /// </summary>
        /// <remarks>Multiple.</remarks>
        /// <value>Element name Value</value>
        /// <see cref="ValueProvider"/>
        [XmlElement("Value")]
        public List<ValueProvider> ValueProviders { get; internal set; }

        [XmlAttribute("remove-value-provider")]
        public bool RemoveValueProviders { get; set; }

        /// <summary>
        /// The name of the template to use to render html content for a row cell in the column.
        /// </summary>
        /// <value>Attribute name template</value>
        [XmlAttribute("template")]
        public string Template { get; set; }

        /// <summary>
        /// Separator to join the values, if there are more than one value.
        /// </summary>
        /// <value>Attribute name separator</value>
        [XmlAttribute("separator")]
        public string HtmlSeparator { get; set; }

        [XmlAttribute("html-multi-value-tag")]
        public string HtmlMultiValueTag { get; set; }

        [XmlAttribute("distinct")]
        public bool Distinct { get; set; }

        ///// <summary>
        ///// The format for formatting the value for a row cell in the column
        ///// </summary>
        ///// <value>Attribute name value-format</value>
        //[XmlAttribute("value-format")]
        //public string ValueFormat { get; set; }

        [XmlAttribute("format")]
        public string FormatId { get { return Format != null ? Format.Id : null; } set { Format = Formatter.GetNamedInstance(value); } }

        /// <summary>
        /// Formatting of the properties value.
        /// </summary>
        /// <value>Element name Format</value>
        public Formatter Format { get; set; }

        //private ValueFormatter valueFormatter;
        //public ValueFormatter ValueFormatter
        //{
        //    get
        //    {
        //        if (valueFormatter == null && ValueFormat != null)
        //        {
        //            valueFormatter = new ValueFormatter(ValueFormat);
        //        }
        //        return valueFormatter;
        //    }
        //}    
    }

    /// <summary>
    /// Settings for a table row.
    /// </summary>
    public class TableRowSettings
    {
        public TableRowSettings()
        {
            Properties = new PropertyDescriptorSettingsDictionary();
        }

        /// <summary>
        /// Comma separated list of class names added to the column.
        /// </summary>
        /// <value>Attribute name class</value>
        [XmlAttribute("class")]
        public string ClassNames { get; set; }

        /// <summary>
        /// Html attributes added to the cell. Multiple.
        /// </summary>
        /// <value>Element name Attribute</value>
        /// <see cref="HtmlAttribute"/>
        [XmlElement("Attribute")]
        public List<HtmlAttribute> Attributes { get; set; }

        /// <summary>
        /// Property settings for the rows properties. E.g the part assembly row properties.
        /// </summary>
        /// <see cref="PropertyDescriptorSettings"/>
        /// <remarks>Multiple.</remarks>
        [XmlElement("Property")]
        public PropertyDescriptorSettingsDictionary Properties { get; private set; }


    }
}