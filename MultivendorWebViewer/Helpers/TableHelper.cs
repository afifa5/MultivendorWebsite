
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Collections;
using MultivendorWebViewer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;


namespace MultivendorWebViewer.Helpers
{
    public class TableColumn<T>
    {
        public TableColumn() { }

        public TableColumn(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

        public string Caption { get; set; }

        public string ClassName { get; set; }

        public string CellClassName { get; set; }

        public object HtmlAttributes { get; set; }

        public Func<IEnumerable<T>, HelperResult> HeaderTemplate { get; set; }

        public Func<T, HelperResult> CellTemplate { get; set; }

        public HtmlContent<T> CellContent { get; set; }

        public Func<IEnumerable<T>, bool> HideFunction { get; set; }

        public Action<T, TagBuilder> CellTagFormatter { get; set; }

        public Func<T, object, object> CellValueFormatter { get; set; }

        public bool? AutoHide { get; set; }

        public bool Ignore { get; set; }

        public int Order { get; set; }

        private Expression<Func<T, object>> property;
        public Expression<Func<T, object>> Property
        {
            get { return property; }
            set
            {
                property = value;
                valueFunction = property != null ? GetValueFunction(property) : null;
            }
        }

        private Func<T, string> valueFunction;
        public Func<T, string> ValueFunction
        {
            get { return valueFunction; }
            set
            {
                valueFunction = value;
                property = null;
            }
        }

        private static Func<T, string> GetValueFunction(Expression<Func<T, object>> expression)
        {
            // This only supports one depth selected!
            if (expression.Body.ToString().Count(c => c == '.') > 1)
            //if (expression.Body is MemberExpression && ((MemberExpression)expression.Body).Expression is MemberExpression)
            {
                var func = expression.Compile();
                return item =>
                {
                    var value = func(item);
                    return value != null ? value.ToString() : null;
                };
            }

            // If the property type is a string, create a function that simply return the property value
            if (expression.Body.Type == typeof(string))
            {
                return Expression.Lambda<Func<T, string>>(expression.Body, expression.Parameters).Compile();
            }
            else
            {
                // If the property type is a value type, it cannot be null (even if it is nullable). It safe to call ToString() without checking for null
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    if (unaryExpression.Operand.Type.IsValueType == true)
                    {
                        return Expression.Lambda<Func<T, string>>(Expression.Call(unaryExpression.Operand, typeof(object).GetMethod("ToString")), expression.Parameters).Compile();
                    }
                }

                // The property type is a reference type (but not string), check for null and return ToString() or null
                var returnTarget = Expression.Label(typeof(string));
                var block = Expression.Block(
                    Expression.IfThen(
                        Expression.ReferenceNotEqual(expression.Body, Expression.Constant(null)),
                        Expression.Return(returnTarget, Expression.Call(expression.Body, typeof(object).GetMethod("ToString")))
                        ),
                    Expression.Label(returnTarget, Expression.Constant(null, typeof(string)))
                 );


                return Expression.Lambda<Func<T, string>>(block, expression.Parameters).Compile();
            }
        }


        public static TableColumn<T> CreateColumn(ApplicationRequestContext requestContext, HtmlHelper htmlHelper, TableColumnSettings settings, TableColumn<T> defaultColumn = null, IFormatProvider formatProvider = null)

        {
            //var settings = Model.Model.Settings.PartAssemblyColumnSettings[id];
            //if ((settings == null && enabledByDefault == true) || (settings != null && settings.Enabled == true))
            {
                var column = defaultColumn ?? new TableColumn<T>();
                
                if (settings != null)
                {
                    if (column.Id == null)
                    {
                        column.Id = settings.Id;
                    }

                    ApplySettings(requestContext, htmlHelper, column, settings, formatProvider);
                }

                return column;
            }
        }

#if NET5
        public static void ApplySettings(ApplicationRequestContext requestContext, IHtmlHelper htmlHelper, TableColumn<T> column, TableColumnSettings settings, IFormatProvider formatProvider = null)
#else
        public static void ApplySettings(ApplicationRequestContext requestContext, HtmlHelper htmlHelper, TableColumn<T> column, TableColumnSettings settings, IFormatProvider formatProvider = null)
#endif
        {
            column.Ignore = settings.Enabled == false;

            if (settings.CaptionKey != null)
            {
                column.Caption = requestContext.GetApplicationTextTranslation(settings.CaptionKey);
            }

            if (settings.Order != int.MinValue)
            {
                column.Order = settings.Order;
            }

            if (settings.ClassNames != null)
            {
                column.CellClassName = settings.ClassNames;
                column.ClassName = settings.ClassNames;
            }

            if (settings.HideEmpty.HasValue == true)
            {
                column.AutoHide = settings.HideEmpty.Value;
            }

            if (settings.CellSettings.Count > 0)
            {
                var oldCellTagFormatter = column.CellTagFormatter;

                column.CellTagFormatter = (row, cellTag) =>
                {
                    if (oldCellTagFormatter != null)
                    {
                        oldCellTagFormatter(row, cellTag);
                    }

                    var rowSettings = settings.CellSettings.GetFirstMatch(row);
                    if (rowSettings != null)
                    {
                        if (rowSettings.ClassNames != null)
                        {
                            cellTag.AddCssClass(rowSettings.ClassNames);
                        }

                        if (rowSettings.Attributes != null)
                        {
                            foreach (var attribute in rowSettings.Attributes)
                            {
                                var value = attribute.GetValue(row, formatProvider: formatProvider);
                                cellTag.MergeAttribute(attribute.Name, value);
                            }
                        }
                    }
                };

                if (column.ValueFunction != null)
                {
                    var oldValueFunction = column.ValueFunction;
                    column.ValueFunction = row =>
                    {
                        var value = oldValueFunction(row);
                        if (value != null)
                        {
                            var rowSettings = settings.CellSettings.GetFirstMatch(row);
                            if (rowSettings != null && rowSettings.Formatter != null)
                            {
                                value = rowSettings.Formatter.GetFormattedValue(value, new FormatterContext<T>(requestContext, row), formatProvider);
                            }
                        }
                        return value;
                    };
                }
            }


            var customSettings = settings as TableCustomColumnSettings;
            if (customSettings != null)
            {
                if (customSettings.Template != null || (customSettings.ValueProviders != null && customSettings.ValueProviders.Count > 0))
                {
                    column.ValueFunction = row => customSettings.GetHtml(row, htmlHelper, new FormatterContext<T>(requestContext, row), formatProvider);
                }
            }
        }
    }

    //public class TableColumnCollection<T> : DictionaryCollection<string, TableColumn<T>>
    //{
    //    protected override string GetKeyForItem(TableColumn<T> item)
    //    {
    //        return item.Id;
    //    }
    //}

    public class TableDefinition<T>
    {
        public TableDefinition()
        {
            AutoHideEmptyColumns = true;

            SelectionMode = TableSelectionMode.Single;
        }

        /// <summary>
        /// Gets or sets if the table should omit columns that do not contains any cells with value (only applies to column create from the Property property)
        /// </summary>
        public bool AutoHideEmptyColumns { get; set; }

        public bool Hottrack { get; set; }

        public TableSelectionMode SelectionMode { get; set; }

        public Action<T, TagBuilder> RowFormatter { get; set; }

        public Func<T, TagBuilder> RowProvider { get; set; }

        public Action<TagBuilder> DefaultHeaderCellFormatter { get; set; }

        public bool IncludeColumnGroup { get; set; }

        public DataViewOptions Pagination { get; set; }
    }

    public enum TableSelectionMode { None, Single, Multi }

    public static class TableHelper
    {
        public static MvcHtmlString Table<T>(this HtmlHelper helper, IEnumerable<T> rows, IEnumerable<TableColumn<T>> columns, object htmlAttributes = null, TableDefinition<T> definition = null)
        {
            var tableDefinition = definition ?? new TableDefinition<T>();
            var tableRows = rows ?? new List<T>();

            // Get all valid columns (displayed)
            var validColumns = columns.Where(c =>
            {
                if (c.Ignore == true) return false;
                if (c.HideFunction != null)
                {
                    return c.HideFunction(tableRows) == false;
                }
                else if (c.ValueFunction != null && ((c.AutoHide.HasValue == true && c.AutoHide.Value == true) || (c.AutoHide.HasValue == false && tableDefinition.AutoHideEmptyColumns == true)))
                {
                    return Helpers.AnyValue(tableRows, c.ValueFunction); // TODO Values is evaluated twice!!
                }
                return true;
            }).OrderBy(c => c.Order).ToArray();

            // TODO Use a TextWriter instead!
            Action<StringBuilder> tableRenderer = html =>
            {
                var tableTag = new TagBuilder("table");

                if (htmlAttributes != null)
                {
                    var attributeBuilder = htmlAttributes as AttributeBuilder;
                    if (attributeBuilder != null)
                    {
                        tableTag.MergeAttributes(attributeBuilder);
                    }
                    else
                    {
                        tableTag.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                    }
                }

                tableTag.MergeAttribute("data-selection-mode", tableDefinition.SelectionMode == TableSelectionMode.Single ? "single" : tableDefinition.SelectionMode == TableSelectionMode.Multi ? "multiple" : "none");

                if (tableDefinition.Hottrack == true)
                {
                    tableTag.AddCssClass("hottrack");
                }

                //tableTag.MergeAttribute("data-multivendor-control", "Table");
                tableTag.AddCssClass("multivendor-table");

                html.Append(tableTag.ToString(TagRenderMode.StartTag));

                // Add column group
                if (tableDefinition.IncludeColumnGroup == true)
                {
                    foreach (var column in validColumns)
                    {
                        var columnTag = new TagBuilder("col");

                        if (column.Id != null)
                        {
                            columnTag.MergeAttribute("data-id", column.Id);
                        }

                        if (column.ClassName != null)
                        {
                            columnTag.AddCssClass(column.ClassName);
                        }
                        html.Append(columnTag.ToString(TagRenderMode.SelfClosing));
                    }
                }

                // Header
                html.Append("<thead>");
                html.Append("<tr>");

                foreach (var column in validColumns)
                {
                    string headerCellHtml = column.HeaderTemplate != null ? column.HeaderTemplate(tableRows).ToString() : column.Caption ?? null;

                    if (Helpers.BeginsWithTag(headerCellHtml, "th") == false)
                    {
                        var headerCellTag = new TagBuilder("th");
                        if (string.IsNullOrEmpty(column.Id) == false)
                        {
                            headerCellTag.GenerateId(column.Id);
                        }

                        var columnHtmlAttributes = Helpers.GetHtmlAttributes(column.HtmlAttributes);

                        if (string.IsNullOrEmpty(column.ClassName) == false)
                        {
                            if (columnHtmlAttributes == null)
                            {
                                headerCellTag.AddCssClass(column.ClassName);
                            }
                            else
                            {
                                columnHtmlAttributes = Helpers.MergeHtmlAttributes(new Dictionary<string, object> { { "class", column.ClassName } }, columnHtmlAttributes);
                            }
                        }

                        if (columnHtmlAttributes != null)
                        {
                            headerCellTag.MergeAttributes(columnHtmlAttributes);
                        }

                        headerCellTag.InnerHtml = headerCellHtml;

                        if (tableDefinition.DefaultHeaderCellFormatter != null)
                        {
                            tableDefinition.DefaultHeaderCellFormatter(headerCellTag);
                        }

                        html.Append(headerCellTag.ToString(TagRenderMode.Normal));
                    }
                    else
                    {
                        html.AppendLine(headerCellHtml);
                    }

                }

                html.Append("</tr>");
                html.Append("</thead>");

                // Body
                html.Append("<tbody>");

                foreach (var row in tableRows)
                {
                    var rowTag = tableDefinition.RowProvider == null ? new TagBuilder("tr") : tableDefinition.RowProvider(row);

                    if (rowTag != null)
                    {
                        if (tableDefinition.RowFormatter != null)
                        {
                            tableDefinition.RowFormatter(row, rowTag);
                        }

                        rowTag.Write(html, TagRenderMode.StartTag);

                        foreach (var descriptor in validColumns)
                        {
                            var cellTag = new TagBuilder("td");

                            if (string.IsNullOrEmpty(descriptor.CellClassName) == false)
                            {
                                cellTag.AddCssClass(descriptor.CellClassName);
                            }

                            if (descriptor.CellTagFormatter != null)
                            {
                                descriptor.CellTagFormatter(row, cellTag);
                            }

                            cellTag.Write(html, TagRenderMode.StartTag);

                            if (descriptor.ValueFunction != null)
                            {
                                string cellHtml = descriptor.ValueFunction(row);
                                if (string.IsNullOrEmpty(cellHtml) == false)
                                {
                                    html.Append(cellHtml);
                                }
                            }
                            else if (descriptor.CellTemplate != null)
                            {
                                string cellHtml = descriptor.CellTemplate(row).ToString();
                                if (string.IsNullOrEmpty(cellHtml) == false)
                                {
                                    html.Append(cellHtml);
                                }
                            }
                            else if (descriptor.CellContent != null)
                            {
                                string cellHtml = descriptor.CellContent.GetContentHtml(helper, row);
                                if (string.IsNullOrEmpty(cellHtml) == false)
                                {
                                    html.Append(cellHtml);
                                }
                            }

                            cellTag.Write(html, TagRenderMode.EndTag);
                        }

                        rowTag.Write(html, TagRenderMode.EndTag);
                    }
                }

                html.Append("</tbody>");

                html.AppendLine(tableTag.ToString(TagRenderMode.EndTag));
            };

            //if (tableDefinition.Pagination != null)
            //{
            //    return DataViewHelper.DataView(helper, tableDefinition.Pagination, contentRenderer: tableRenderer);
            //}
            //else
            {
                var html = new StringBuilder();
                tableRenderer(html);
                return MvcHtmlString.Create(html.ToString());
            }
        }

        public static void ApplySettings<T, TIdentifier>(this IList<TableColumn<T>> columns, ApplicationRequestContext requestContext, HtmlHelper htmlHelper, TableColumnSettingsDictionary<TIdentifier> settings, IFormatProvider formatProvider = null)
            where TIdentifier : Identifier
        {
            var columnsWithSettings = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var column in columns)
            {
                var columnSettings = settings[column.Id];
                if (columnSettings != null && columnSettings.Item.CreateNew != true)
                {
                    columnsWithSettings.Add(column.Id);
                    TableColumn<T>.ApplySettings(requestContext, htmlHelper, column, columnSettings.Item);
                }
            }

            foreach (var columnSettingsObj in settings)
            {
                var columnSettings = columnSettingsObj.Item;
                string id = columnSettings.Id;
                if (columnSettings.CreateNew != false && (id == null || columnsWithSettings.Contains(id) == false) && ((columnSettings.ValueProviders != null && columnSettings.ValueProviders.Count > 0) || columnSettings.Template != null))
                {
                    var column = TableColumn<T>.CreateColumn(requestContext, htmlHelper, columnSettings, formatProvider: formatProvider);
                    if (column.Ignore == false)
                    {
                        columns.Add(column);
                    }
                }
            }
        }
    }
}