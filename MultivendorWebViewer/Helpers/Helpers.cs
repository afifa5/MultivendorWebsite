using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;

using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using System.Globalization;
using MultivendorWebViewer.Configuration;
using System.ComponentModel;

namespace MultivendorWebViewer.Helpers
{
    public static class HtmlHelpers
    {
        public static IDictionary<string, object> ToHtmlAttributes(this HtmlAttributeCollection htmlAttributes, object obj, object otherHtmlAttributes = null)
        {
            IDictionary<string, object> dict;
            if (otherHtmlAttributes != null)
            {
                var tmpDict = otherHtmlAttributes as IDictionary<string, object>;
                if (tmpDict != null)
                {
                    dict = new Dictionary<string, object>();
                    foreach (var kvp in tmpDict)
                    {
                        dict.Add(kvp.Key, kvp.Value);
                    }
                }
                else
                {
                    dict = HtmlHelper.AnonymousObjectToHtmlAttributes(otherHtmlAttributes);
                }
            }
            else
            {
                dict = new Dictionary<string, object>();
            }

            if (htmlAttributes != null)
            {
                htmlAttributes.AddToHtmlAttributes(obj, dict);
            }

            return dict;
        }

        public static string GetHtml(this HtmlHelper htmlHelper, object item, IList<ValueProvider> valueProviders = null, Formatter format = null, string htmlTemplate = null, string htmlSeparator = null, FormatterContext formatterContext = null, IFormatProvider formatProvider = null, IEqualityComparer<object> distinctComparer = null)

        {
            if (formatProvider == null) formatProvider = CultureInfo.CurrentUICulture;

            if (valueProviders.Count > 0)
            {
                // Most common
                if (valueProviders.Count == 1)
                {
                    if (htmlTemplate == null)
                    {
                        object value = valueProviders[0].GetFormattedValue(item, formatterContext, formatProvider);

                        var values = value is string ? null : value as IEnumerable;
                        if (values == null)
                        {
                            // Most common way (one provider, one value)
                            return format != null ? (format.GetFormattedValue(value, formatterContext, formatProvider) ?? string.Empty) : value as string ?? Formatter.ToFormattedString(value, formatProvider);
                        }
                        else
                        {
                            var valuesArray = values.OfType<object>().Where(v => v != null).ToArray();
                            if (valuesArray.Length == 0) return string.Empty;
                            // One provider, several values
                            if (htmlSeparator != null)
                            {
                                string joinedValues = string.Join(htmlSeparator, valuesArray);
                                return format != null ? format.GetFormattedValue(joinedValues, formatterContext, formatProvider) : joinedValues;
                            }
                            else
                            {
                                return format != null ? format.GetFormattedValue(string.Concat(valuesArray)) : string.Concat(valuesArray);
                            }
                        }
                    }
                    else
                    {
                        object value = valueProviders[0].GetValue(item, formatterContext);

                        using (var writer = new StringWriter(formatProvider))
                        {
                            htmlHelper.RenderPartial(htmlTemplate, writer, model: value);
                            return writer.ToString();
                        }
                    }
                }
                else
                {
                    if (htmlTemplate == null)
                    {
                        var combinedValues = valueProviders.SelectMany(vp =>
                        {
                            object value = vp.GetFormattedValue(item, formatterContext, formatProvider);
                            var values = value as IEnumerable<object>;
                            return values ?? new[] { value };
                        }).Where(v => v != null);

                        if (distinctComparer != null)
                        {
                            combinedValues = combinedValues.Distinct(distinctComparer);
                        }

                        if (htmlSeparator != null)
                        {
                            string joinedValues = string.Join(htmlSeparator, combinedValues);
                            return format != null ? format.GetFormattedValue(joinedValues, formatterContext, formatProvider) : joinedValues;
                        }
                        else
                        {
                            return format != null ? format.GetFormattedValue(string.Concat(combinedValues)) : string.Concat(combinedValues);
                        }
                    }
                    else
                    {
                        var combinedValues = valueProviders.SelectMany(vp =>
                        {
                            object value = vp.GetValue(item, formatterContext);
                            var values = value as IEnumerable<object>;
                            return values ?? new[] { value };
                        });

                        using (var writer = new StringWriter(formatProvider))
                        {
                            htmlHelper.RenderPartial(htmlTemplate, writer, model: combinedValues.ToArray());
                            return writer.ToString();
                        }
                    }
                }
            }
            else
            {
                if (htmlTemplate != null)
                {
                    using (var writer = new StringWriter(formatProvider))
                    {
                        htmlHelper.RenderPartial(htmlTemplate, writer, model: item);
                        return writer.ToString();
                    }
                }
            }

            return string.Empty;
        }
    }
    public static class Helpers
    {
        public static bool IsNullOrEmpty(this MvcHtmlString htmlString)
        {
            return htmlString == null || MvcHtmlString.IsNullOrEmpty(htmlString) == true;
        }

        //public static MvcHtmlString FormattedText(this HtmlHelper htmlHelper, string text, string classNames = null, TextDisplaySettings settings = null, NavigationUrl url = null, Action<TextWriter, string> textWriterHandler = null, ApplicationRequestContext requestContext = null)
        //{
        //    using (var writer = new StringWriter())
        //    {
        //        RenderTextInternal(htmlHelper, writer, text, null, new AttributeBuilder(classNames), settings, true, url, textWriterHandler, requestContext);
        //        return MvcHtmlString.Create(writer.ToString());
        //    }
        //}
        //public static MvcHtmlString FormattedText(this HtmlHelper htmlHelper, string text, string tagName, string classNames = null, TextDisplaySettings settings = null, NavigationUrl url = null, Action<TextWriter, string> textWriterHandler = null, ApplicationRequestContext requestContext = null)
        //{
        //    using (var writer = new StringWriter())
        //    {
        //        RenderTextInternal(htmlHelper, writer, text, tagName, new AttributeBuilder(classNames), settings, true, url, textWriterHandler, requestContext);
        //        return MvcHtmlString.Create(writer.ToString());
        //    }
        //}

        //public static void RenderText(this HtmlHelper htmlHelper, string text, object htmlAttributes = null, TextDisplaySettings settings = null, NavigationUrl url = null, Action<TextWriter, string> textWriterHandler = null, ApplicationRequestContext requestContext = null)
        //{
        //    RenderTextInternal(htmlHelper, htmlHelper.ViewContext.Writer, text, null, new AttributeBuilder(htmlAttributes), settings, false, url, textWriterHandler, requestContext);
        //}

        //public static void RenderText(this HtmlHelper htmlHelper, TextWriter writer, string text, object htmlAttributes = null, TextDisplaySettings settings = null, NavigationUrl url = null, Action<TextWriter, string> textWriterHandler = null, ApplicationRequestContext requestContext = null)
        //{
        //    RenderTextInternal(htmlHelper, writer, text, null, new AttributeBuilder(htmlAttributes), settings, false, url, textWriterHandler, requestContext);
        //}

        //public static void RenderFormattedText(this HtmlHelper htmlHelper, string text, string tagName = null, object htmlAttributes = null, TextDisplaySettings settings = null, NavigationUrl url = null, Action<TextWriter, string> textWriterHandler = null, ApplicationRequestContext requestContext = null)
        //{
        //    RenderTextInternal(htmlHelper, htmlHelper.ViewContext.Writer, text, tagName, new AttributeBuilder(htmlAttributes), settings, true, url, textWriterHandler, requestContext);
        //}

        //public static void RenderFormattedText(this HtmlHelper htmlHelper, TextWriter writer, string text, string tagName = null, object htmlAttributes = null, TextDisplaySettings settings = null, NavigationUrl url = null, Action<TextWriter, string> textWriterHandler = null, ApplicationRequestContext requestContext = null)
        //{
        //    RenderTextInternal(htmlHelper, writer, text, tagName, new AttributeBuilder(htmlAttributes), settings, true, url, textWriterHandler, requestContext);
        //}

        //private static void RenderTextInternal(HtmlHelper htmlHelper, TextWriter htmlWriter, string text, string tagName, AttributeBuilder attributeBuilder, TextDisplaySettings settings, bool formatted, NavigationUrl url, Action<TextWriter, string> textWriterHandler, ApplicationRequestContext requestContext)
        //{
        //    if (settings == null)
        //    {
        //        settings = TextDisplaySettings.Empty;
        //    }
        //    else
        //    {
        //        if (settings.Layout.HasValue == true && settings.Layout.Value.HasFlag(ComponentLayoutMode.Hidden) == true) // TODO Remove??
        //        {
        //            return;
        //        }

        //        if (requestContext == null)
        //        {
        //            requestContext = htmlHelper.ViewContext.GetApplicationRequestContext();
        //        }

        //        settings.AddToAttributes(attributeBuilder, null, new FormatterContext { ApplicationRequestContext = requestContext });
        //    }

        //    //// Classes
        //    //if (classNames != null)
        //    //{
        //    //    htmlWriter.Write("<span class=\"");
        //    //    htmlWriter.Write(classNames);
        //    //    if (settings.Class != null)
        //    //    {
        //    //        htmlWriter.Write(" ");
        //    //        htmlWriter.Write(settings.Class);
        //    //    }
        //    //    htmlWriter.Write("\">");
        //    //}
        //    //else if (settings.Class != null)
        //    //{
        //    //    htmlWriter.Write("<span class=\"");
        //    //    htmlWriter.Write(settings.Class);
        //    //    htmlWriter.Write("\">");
        //    //}
        //    //else
        //    //{
        //    //    htmlWriter.Write("<span>");
        //    //}

        //    if (settings.Tag != null)
        //    {
        //        tagName = settings.Tag;
        //    }

        //    // If we have a label and the tag is not a span, it will be rendered outside the text tag
        //    if (settings.Label != null && settings.LabelTag != "span")
        //    {
        //        if (requestContext == null) requestContext = htmlHelper.ViewContext.GetApplicationRequestContext();
        //        string labelText = requestContext.GetApplicationTextTranslation(settings.Label);
        //        LabelInternal(htmlHelper, htmlWriter, settings, labelText, null, settings.LabelClass != null ? new AttributeBuilder(settings.LabelClass) : null, requestContext);
        //    }

        //    if (attributeBuilder.Count > 0)
        //    {
        //        if (url.IsNullOrNone() == false && settings.NavigationDisabled != true)
        //        {
        //            url.WriteStartTag(htmlWriter, attributeBuilder);
        //            if (string.IsNullOrEmpty(tagName) == false)
        //            {
        //                htmlWriter.Write('<');
        //                htmlWriter.Write(tagName);
        //                htmlWriter.Write('>');
        //            }
        //        }
        //        else if (string.IsNullOrEmpty(tagName) == true)
        //        {
        //            htmlWriter.Write("<span ");
        //            attributeBuilder.WriteTo(htmlWriter);
        //            htmlWriter.Write('>');
        //        }
        //        else
        //        {
        //            htmlWriter.Write('<');
        //            htmlWriter.Write(tagName);
        //            htmlWriter.Write(' ');
        //            attributeBuilder.WriteTo(htmlWriter);
        //            htmlWriter.Write('>');
        //        }
        //    }
        //    else
        //    {
        //        if (url.IsNullOrNone() == false && settings.NavigationDisabled == false)
        //        {
        //            url.WriteStartTag(htmlWriter, null);
        //            if (tagName != null)
        //            {
        //                htmlWriter.Write('<');
        //                htmlWriter.Write(tagName);
        //                htmlWriter.Write('>');
        //            }
        //        }
        //        /*else if (tagName == null)
        //        {
        //            htmlWriter.Write("<span>");
        //        }*/
        //        else if (string.IsNullOrEmpty(tagName) == false)
        //        {
        //            htmlWriter.Write('<');
        //            htmlWriter.Write(tagName);
        //            htmlWriter.Write('>');
        //        }
        //    }

        //    //if (settings.BackDropSetting != null && settings.BackDropSetting.Element == true)
        //    //{
        //    //    htmlWriter.Write("<div>");
        //    //}      

        //    // If we have a label and the tag is a span, it will be rendered insideside the text tag
        //    if (settings.Label != null && settings.LabelTag == "span")
        //    {
        //        if (requestContext == null) requestContext = htmlHelper.ViewContext.GetApplicationRequestContext();
        //       string  labelText = requestContext.GetApplicationTextTranslation(settings.Label);
        //       LabelInternal(htmlHelper, htmlWriter, settings, labelText, null, settings.LabelClass != null ? new AttributeBuilder(settings.LabelClass) : null, requestContext);
        //        //htmlWriter.Write("<label>");
        //        //htmlWriter.Write(requestContext.GetApplicationTextTranslation(settings.Label));
        //        //htmlWriter.Write("</label>");
        //    }

        //    if (text != null)
        //    {
        //        //text = text.Replace(",", ",&shy");
        //        text = text.Replace(",", ",\u200B"); // TODO Insert break possibility at ','

        //        //string[] textParts = text.Split(' ');

        //        if (settings.Format != null && formatted == false)
        //        {
        //            if (requestContext == null) requestContext = htmlHelper.ViewContext.GetApplicationRequestContext();
        //            text = settings.Format.GetFormattedValue(text, new FormatterContext(requestContext, null), requestContext.DisplayCulture);
        //        }

        //        if (text != null)
        //        {
        //            if (settings.MaxLength.HasValue == true)
        //            {
        //                if (text.Length > settings.MaxLength.Value)
        //                {
        //                    if (settings.MaxLengthEllipsis == true)
        //                    {
        //                        text = text.Substring(0, settings.MaxLength.Value - 1) + "…";
        //                    }
        //                    else
        //                    {
        //                        text = text.Substring(0, settings.MaxLength.Value);
        //                    }
        //                }
        //            }
        //            if (textWriterHandler == null)
        //            {
        //                htmlWriter.Write(text);
        //            }
        //            else
        //            {
        //                textWriterHandler(htmlWriter, text);
        //            }
        //        }
        //    }

        //    if (url.IsNullOrNone() == false && settings.NavigationDisabled != true)
        //    {
        //        if (string.IsNullOrEmpty(tagName) == false)
        //        {
        //            htmlWriter.Write("</");
        //            htmlWriter.Write(tagName);
        //            htmlWriter.Write('>');
        //        }
        //        htmlWriter.Write("</a>");
        //    }
        //    else if (string.IsNullOrEmpty(tagName) == true)
        //    {
        //        if (attributeBuilder.Count > 0)
        //        {
        //            htmlWriter.Write("</span>");
        //        }
        //    }
        //    else
        //    {
        //        htmlWriter.Write("</");
        //        htmlWriter.Write(tagName);
        //        htmlWriter.Write('>');
        //    }
        //}

        //public static MvcHtmlString ComponentLabel(this HtmlHelper htmlHelper, IComponentLabel settings, string fallbackLabelText = null, string tagName = null, object htmlAttributes = null, ApplicationRequestContext requestContext = null)
        //{
        //    if (settings == null) settings = ComponentSettings.Empty;
        //    string labelText;
        //    if (fallbackLabelText == null)
        //    {
        //        if (string.IsNullOrEmpty(settings.Label) == true) return MvcHtmlString.Empty;
        //        if (requestContext == null) requestContext = htmlHelper.ViewContext.GetApplicationRequestContext();
        //        labelText = requestContext.GetApplicationTextTranslation(settings.Label);
        //    }
        //    else
        //    {
        //        if (settings.Label == null)
        //        {
        //            labelText = fallbackLabelText;
        //        }
        //        else if (settings.Label.Length == 0)
        //        {
        //            return MvcHtmlString.Empty;
        //        }
        //        else
        //        {
        //            if (requestContext == null) requestContext = htmlHelper.ViewContext.GetApplicationRequestContext();
        //            labelText = requestContext.GetApplicationTextTranslation(settings.Label);
        //        }
        //    }

        //    using (var writer = new StringWriter())
        //    {
        //        LabelInternal(htmlHelper, writer, settings, labelText, tagName, AttributeBuilder.TryCreate(htmlAttributes), requestContext);
        //        return MvcHtmlString.Create(writer.ToString());
        //    }
        //}
        //public static void RenderComponentLabel(this HtmlHelper htmlHelper, IComponentLabel settings = null, string fallbackLabelText = null, string tagName = null, object htmlAttributes = null, ApplicationRequestContext requestContext = null)
        //{
        //    Helpers.RenderComponentLabelInternal(htmlHelper, htmlHelper.ViewContext.Writer, settings ?? ComponentSettings.Empty, fallbackLabelText, tagName, htmlAttributes, requestContext);
        //}

        //public static void RenderComponentLabel(this HtmlHelper htmlHelper, TextWriter htmlWriter, IComponentLabel settings = null, string fallbackLabelText = null, string tagName = null, object htmlAttributes = null, ApplicationRequestContext requestContext = null)
        //{
        //    Helpers.RenderComponentLabelInternal(htmlHelper, htmlWriter, settings ?? ComponentSettings.Empty, fallbackLabelText, tagName, htmlAttributes, requestContext);
        //}

        //public static void RenderComponentLabelInternal(this HtmlHelper htmlHelper, TextWriter htmlWriter, IComponentLabel settings, string fallbackLabelText, string tagName, object htmlAttributes, ApplicationRequestContext requestContext)
        //{
        //    string labelText;
        //    if (fallbackLabelText == null)
        //    {
        //        if (string.IsNullOrEmpty(settings.Label) == true) return;
        //        labelText = (requestContext ?? htmlHelper.ViewContext.GetApplicationRequestContext()).GetApplicationTextTranslation(settings.Label);
        //    }
        //    else
        //    {
        //        if (settings.Label == null)
        //        {
        //            labelText = fallbackLabelText;
        //        }
        //        else if (settings.Label.Length == 0)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            if (requestContext == null) requestContext = htmlHelper.ViewContext.GetApplicationRequestContext();
        //            labelText = requestContext.GetApplicationTextTranslation(settings.Label);
        //        }
        //    }

        //    LabelInternal(htmlHelper, htmlWriter, settings, labelText, tagName, AttributeBuilder.TryCreate(htmlAttributes), requestContext);
        //}


        //private static void LabelInternal(HtmlHelper htmlHelper, TextWriter htmlWriter, IComponentLabel settings, string labelText, string tagName, AttributeBuilder attributes, ApplicationRequestContext requestContext)
        //{
        //    if (settings.LabelTag != null)
        //    {
        //        tagName = settings.LabelTag;
        //    }
        //    /*else if (settings.Tag != null)
        //    {
        //        tagName = settings.Tag;
        //    }*/
        //    else if (tagName == null)
        //    {
        //        tagName = "label";
        //    }

        //    htmlWriter.Write('<');
        //    htmlWriter.Write(tagName);
        //    if (attributes != null) 
        //    {
        //        htmlWriter.Write(' ');
        //        var additionalAttributes = (settings.Layout & ComponentLayoutMode.Collapseable) != 0 ? new AttributeBuilder((settings.Layout & ComponentLayoutMode.Collapsed) != 0 ? "collapse-label collapsed" : "collapse-label") : null;
        //        attributes.WriteTo(htmlWriter, additionalAttributes);
        //    }
        //    else
        //    {
        //        if (settings.Layout != null && (settings.Layout & ComponentLayoutMode.Collapseable) != 0)
        //        {
        //            if ((settings.Layout & ComponentLayoutMode.Collapsed) != 0)
        //            {
        //                htmlWriter.Write(" class=\"collapse-label collapsed\"");
        //            }
        //            else
        //            {
        //                htmlWriter.Write(" class=\"collapse-label\"");
        //            }
        //        }
        //    }

        //    if (settings.LabelFor != null)
        //    {
        //        if (StringComparer.OrdinalIgnoreCase.Equals(tagName, "label") == true)
        //        {
        //            htmlWriter.Write(" for=\"");
        //            htmlWriter.Write(settings.LabelFor);
        //            htmlWriter.Write('\"');
        //        }
        //        htmlWriter.Write(" data-for=\"");
        //        htmlWriter.Write(settings.LabelFor);
        //        htmlWriter.Write('\"');
        //    }

        //    if (settings.LabelShort == null)
        //    {
        //        htmlWriter.Write('>');
        //        htmlWriter.Write(labelText);
        //    }
        //    else
        //    {
        //        if (requestContext == null) requestContext = htmlHelper.ViewContext.GetApplicationRequestContext();
        //        htmlWriter.Write(" data-label=\"");
        //        htmlWriter.Write(labelText);
        //        htmlWriter.Write("\" data-label-short=\"");
        //        htmlWriter.Write(requestContext.GetApplicationTextTranslation(settings.LabelShort));
        //        htmlWriter.Write("\">");
        //    }

        //    htmlWriter.Write("</");
        //    htmlWriter.Write(tagName);
        //    htmlWriter.Write('>');
        //}
#if NET5
        public static IHtmlContent Raws(this HtmlHelper htmlHelper, string separator, params string[] values)
#else
        public static IHtmlString Raws(this HtmlHelper htmlHelper, string separator, params string [] values)
#endif
        {
            return htmlHelper.Raw(string.Join(separator, values));
        }

#if NET5
        public static IHtmlContent Raws(this HtmlHelper htmlHelper, string separator, params object[] values)
#else
        public static IHtmlString Raws(this HtmlHelper htmlHelper, string separator, params object[] values)
#endif

        {
            return htmlHelper.Raw(string.Join(separator, values));
        }

#if NET5
        public static IHtmlContent ClassAttr(this HtmlHelper htmlHelper, params string[] classNames)
#else
        public static IHtmlString ClassAttr(this HtmlHelper htmlHelper, params string[] classNames)
#endif
        {
            StringBuilder stringBuilder = null; 

            foreach (string className in classNames)
            {
                if (className != null)
                {
                    string trimmedClassName = className.Trim();
                    if (trimmedClassName.Length > 0)
                    {
                        if (stringBuilder == null)
                        {
                            stringBuilder = new StringBuilder();
                            stringBuilder.Append("class=\"");
                        }
                        else
                        {
                            stringBuilder.Append(" ");
                        }
                        stringBuilder.Append(trimmedClassName.Trim());

                    }
                }
            }
            
            if (stringBuilder != null)
            {
                stringBuilder.Append("\"");
                return htmlHelper.Raw(stringBuilder.ToString());
            }
            return MvcHtmlString.Empty;
        }

        //public static IHtmlString DataAttr(this HtmlHelper htmlHelper, string name, object value, Func<bool> predicate = null)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();

        //    if (name.StartsWith("data-") == false) stringBuilder.Append("data-");
        //    stringBuilder.Append(name).Append("=\"").Append(value as string ?? value.ToString()).Append("\"");

        //    return htmlHelper.Raw(stringBuilder.ToString());
        //}

        public static AttributeBuilder Class(this HtmlHelper htmlHelper, string className, Func<bool> predicate = null)
        {
            return new AttributeBuilder().Class(className, predicate);
        }

        public static AttributeBuilder Attr(this HtmlHelper htmlHelper, IDictionary<string, object> attributes)
        {
            var builder = new AttributeBuilder();
            foreach (var kvp in attributes)
            {
                builder.Attr(kvp.Key, kvp.Value);
            }
            return builder;
        }

        public static AttributeBuilder Attr(this HtmlHelper htmlHelper, string name, object value)
        {
            return new AttributeBuilder().Attr(name, value);
        }

        public static AttributeBuilder Attr(this HtmlHelper htmlHelper, string name, Func<object> value, Func<bool> predicate)
        {
            return new AttributeBuilder().Attr(name, value, predicate);
        }

        public static AttributeBuilder Attr(this HtmlHelper htmlHelper, string name1, object value1, string name2, object value2, Func<bool> predicate = null)
        {
            return new AttributeBuilder().Attr(name1, value1, name2, value2, predicate);
        }

        public static AttributeBuilder DataAttr(this HtmlHelper htmlHelper, string name, object value, Func<bool> predicate = null)
        {
            return new AttributeBuilder().DataAttr(name, value, predicate);
        }

        public static AttributeBuilder DataAttr(this HtmlHelper htmlHelper, string name1, object value1, string name2, object value2, Func<bool> predicate = null)
        {      
            return new AttributeBuilder().DataAttr(name1, value1, name2, value2, predicate);
        }

        public static AttributeBuilder DataAttr(this HtmlHelper htmlHelper, string name1, object value1, string name2, object value2, string name3, object value3, Func<bool> predicate = null)
        {
            return new AttributeBuilder().DataAttr(name1, value1, name2, value2, name3, value3, predicate);
        }

        public static AttributeBuilder DataAttr(this HtmlHelper htmlHelper, string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, Func<bool> predicate = null)
        {
            return new AttributeBuilder().DataAttr(name1, value1, name2, value2, name3, value3, name4, value4, predicate);
        }

        public static AttributeBuilder DataAttr(this HtmlHelper htmlHelper, string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, Func<bool> predicate = null)
        {
            return new AttributeBuilder().DataAttr(name1, value1, name2, value2, name3, value3, name4, value4, name5, value5, predicate);
        }

        public static AttributeBuilder DataAttr(this HtmlHelper htmlHelper, string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, Func<bool> predicate = null)
        {
            return new AttributeBuilder().DataAttr(name1, value1, name2, value2, name3, value3, name4, value4, name5, value5, name6, value6, predicate);
        }

        public static AttributeBuilder DataAttr(this HtmlHelper htmlHelper, string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, Func<bool> predicate = null)
        {
            return new AttributeBuilder().DataAttr(name1, value1, name2, value2, name3, value3, name4, value4, name5, value5, name6, value6, name7, value7, predicate);
        }

        public static AttributeBuilder DataAttr(this HtmlHelper htmlHelper, string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, string name8, object value8, Func<bool> predicate = null)
        {
            return new AttributeBuilder().DataAttr(name1, value1, name2, value2, name3, value3, name4, value4, name5, value5, name6, value6, name7, value7, name8, value8, predicate);
        }

        public static AttributeBuilder Attr(this HtmlHelper htmlHelper, IAddToAttributes attributeAdder, Func<bool> predicate = null, object obj = null,  IFormatProvider formatProvider = null)
        {
            return new AttributeBuilder().Attr(attributeAdder, predicate, obj,  formatProvider);
        }

        public static bool IsDebug(this HtmlHelper htmlHelper)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public static MvcHtmlString Text(this HtmlHelper helper, string text)
        {
            if (text == null)
            {
                return MvcHtmlString.Empty;
            }

            int length = text.Length;

            if (length == 0)
            {
                return MvcHtmlString.Empty;
            }

            var builder = new StringBuilder(text.Length * 2 + 1);

            char[] tokens = new char[] { '\r', '\n' };

            int startIndex = 0;

            while (startIndex < length)
            {
                int tokenIndex = text.IndexOfAny(tokens, startIndex);

                if (tokenIndex != -1)
                {
                    builder.Append(HttpUtility.HtmlEncode(text.Substring(startIndex, tokenIndex - startIndex)));
                    char token = text[tokenIndex];

                    if (token == '\r')
                    {
                        if (text[tokenIndex + 1] != '\n')
                        {
                            startIndex = tokenIndex + 1;
                        }
                        else
                        {
                            startIndex = tokenIndex + 2;
                        }
                    }
                    else
                    {
                        if (text[tokenIndex + 1] != '\r')
                        {
                            startIndex = tokenIndex + 1;
                        }
                        else
                        {
                            startIndex = tokenIndex + 2;
                        }
                    }

                    builder.Append("<br />");
                }
                else
                {
                    builder.Append(HttpUtility.HtmlEncode(text.Substring(startIndex)));
                    return MvcHtmlString.Create(builder.ToString());
                }
            }

            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString Text(this HtmlHelper helper, string text, bool htmlEncode)
        {
            if (text == null) return MvcHtmlString.Empty;

            if (htmlEncode == true)
            {
                return new MvcHtmlString(HttpUtility.HtmlEncode(text));
            }
            else
            {
                return new MvcHtmlString(text);
            }
        }

        //public static MvcHtmlString Text(this HtmlHelper helper, string text, Formatter formatter, FormatterContext formattedContext, IFormatProvider formatProvider = null)
        //{
        //    if (text == null) return MvcHtmlString.Empty;

        //    if (formatter != null)
        //    {
        //        string formattedText = formatter.GetFormattedValue(text, formattedContext, formatProvider);

        //        if (formatter.HtmlEncode == true)
        //        {
        //            return new MvcHtmlString(text);
        //        }
        //        else
        //        {
        //            return new MvcHtmlString(HttpUtility.HtmlEncode(text));
        //        }
        //    }
        //    else
        //    {
        //        return new MvcHtmlString(HttpUtility.HtmlEncode(text));
        //    }
        //}

        public static IDictionary<string, object> GetHtmlAttributes(object htmlAttributes)
        {
            return htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        }

        public static IDictionary<string, object> GetHtmlAttributes(object htmlAttributes, bool createCopy, IEqualityComparer<string> comparer = null)
        {
            var htmlAttributesDict = htmlAttributes as IDictionary<string, object>;
            if (htmlAttributesDict != null)
            {
                if (createCopy == false) return htmlAttributesDict;
                var htmlAttributesDictCopy = comparer == null ? new Dictionary<string, object>(htmlAttributesDict.Count) : new Dictionary<string, object>(htmlAttributesDict.Count, comparer);
                foreach (var kvp in htmlAttributesDict)
                {
                    htmlAttributesDictCopy[kvp.Key] = kvp.Value;
                }
                return htmlAttributesDictCopy;
            }
            return HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        }

        public static object GetHtmlAttributeValue(object htmlAttributes, string key)
        {
            var dictionary = GetHtmlAttributes(htmlAttributes);
            if (dictionary != null)
            {
                return dictionary[key];
            }

            return null;
        }

        public static void MergeClassAttribute(IDictionary<string, object> dictionary, string classNames)
        {
            object existingClassNamesObj;
            if (dictionary.TryGetValue("class", out existingClassNamesObj) == true)
            {
                string existingClassNames = existingClassNamesObj as string;
                if (string.IsNullOrEmpty(existingClassNames) == false)
                {
                    dictionary["class"] = string.Concat(existingClassNames, " ", classNames);
                    return;
                }
            }
            dictionary["class"] = classNames;
        }

        public static void MergeClassAttribute(IDictionary<string, object> dictionary, IEnumerable<string> classNames)
        {
            Helpers.MergeClassAttribute(dictionary, string.Join(" ", classNames));
        }

        public static IDictionary<string, object> MergeHtmlAttributes(object attr1, object attr2, bool newInstance = true)
        {
            return Helpers.MergeHtmlAttributes(Helpers.GetHtmlAttributes(attr1), Helpers.GetHtmlAttributes(attr2), newInstance);
        }

        public static IDictionary<string, object> MergeHtmlAttributes(IDictionary<string, object> attr1, IDictionary<string, object> attr2, bool newInstance = true)
        {
            IDictionary<string, object> result = newInstance == true || attr1 == null ? new RouteValueDictionary(attr1) : attr1;

            if (attr2 != null)
            {
                foreach (var entry in attr2)
                {
                    if (entry.Key == "class")
                    {
                        object existingClassNames;
                        if (result.TryGetValue("class", out existingClassNames) == false)
                        {
                            result[entry.Key] = entry.Value;
                        }
                        else
                        {
                            result[entry.Key] = string.Concat(existingClassNames, " ", entry.Value);
                        }
                    }
                    else
                    {
                        result[entry.Key] = entry.Value;
                    }
                }
            }
            return result;
        }

        public static void AppendAttributes(this TagBuilder tagBuilder, StringBuilder stringBuilder)
        {
            foreach (var attribute in tagBuilder.Attributes)
            {
                string key = attribute.Key;
                if (String.Equals(key, "id", StringComparison.Ordinal) && String.IsNullOrEmpty(attribute.Value))
                {
                    continue;
                }
                string value = HttpUtility.HtmlAttributeEncode(attribute.Value);
                stringBuilder.Append(' ')
                    .Append(key)
                    .Append("=\"")
                    .Append(value)
                    .Append('"');
            }
        }

        public static void AppendAttributes(this TagBuilder tagBuilder, TextWriter writer)
        {
            foreach (var attribute in tagBuilder.Attributes)
            {
                string key = attribute.Key;
                if (String.Equals(key, "id", StringComparison.Ordinal) && String.IsNullOrEmpty(attribute.Value))
                {
                    continue;
                }
                string value = HttpUtility.HtmlAttributeEncode(attribute.Value);
                writer.Write(' ');
                writer.Write(key);
                writer.Write("=\"");
                writer.Write(value);
                writer.Write('"');
            }
        }


#if NET5
        [Obsolete("Change to proper usage of TagBuilder")]
#endif
        public static void Write(this TagBuilder tagBuilder, StringBuilder stringBuilder, TagRenderMode renderMode = TagRenderMode.Normal)
        {
            switch (renderMode)
            {
                case TagRenderMode.StartTag:
                    stringBuilder.Append('<').Append(tagBuilder.TagName);
                    Helpers.AppendAttributes(tagBuilder, stringBuilder);
                    stringBuilder.Append('>');
                    break;
                case TagRenderMode.EndTag:
                    stringBuilder.Append("</").Append(tagBuilder.TagName).Append('>');
                    break;
                case TagRenderMode.SelfClosing:
                    stringBuilder.Append('<').Append(tagBuilder.TagName);
                    Helpers.AppendAttributes(tagBuilder, stringBuilder);
                    stringBuilder.Append(" />");
                    break;
                default:
                    stringBuilder.Append('<').Append(tagBuilder.TagName);
                    Helpers.AppendAttributes(tagBuilder, stringBuilder);
                    stringBuilder.Append('>').Append(tagBuilder.InnerHtml).Append("</").Append(tagBuilder.TagName).Append('>');
                    break;
            }
        }

#if NET5
        [Obsolete("Change to proper usage of TagBuilder")]
#endif
        public static void Write(this TagBuilder tagBuilder, TextWriter writer, TagRenderMode renderMode = TagRenderMode.Normal)
        {
#if NET5
            switch (renderMode)
            {
                case TagRenderMode.StartTag:
                    tagBuilder.RenderStartTag().WriteTo(writer, HtmlEncoder.Default);
                    break;
                case TagRenderMode.EndTag:
                    tagBuilder.RenderEndTag().WriteTo(writer, HtmlEncoder.Default);
                    break;
                case TagRenderMode.SelfClosing:
                    tagBuilder.RenderSelfClosingTag().WriteTo(writer, HtmlEncoder.Default);
                    break;
                default:
                    if (tagBuilder.TagRenderMode == TagRenderMode.Normal)
                    {
                        tagBuilder.WriteTo(writer, HtmlEncoder.Default);
                    }
                    else
                    {
                        var tagRenderMode = tagBuilder.TagRenderMode;
                        tagBuilder.TagRenderMode = TagRenderMode.Normal;
                        tagBuilder.WriteTo(writer, HtmlEncoder.Default);
                        tagBuilder.TagRenderMode = tagRenderMode;
                    }
                    break;
            }
#else
            switch (renderMode)
            {
                case TagRenderMode.StartTag:
                    writer.Write('<');
                    writer.Write(tagBuilder.TagName);
                    Helpers.AppendAttributes(tagBuilder, writer);
                    writer.Write('>');
                    break;
                case TagRenderMode.EndTag:
                    writer.Write("</");
                    writer.Write(tagBuilder.TagName);
                    writer.Write('>');
                    break;
                case TagRenderMode.SelfClosing:
                    writer.Write('<');
                    writer.Write(tagBuilder.TagName);
                    Helpers.AppendAttributes(tagBuilder, writer);
                    writer.Write(" />");
                    break;
                default:
                    writer.Write('<');
                    writer.Write(tagBuilder.TagName);
                    Helpers.AppendAttributes(tagBuilder, writer);
                    writer.Write('>');
                    writer.Write(tagBuilder.InnerHtml);
                    writer.Write("</");
                    writer.Write(tagBuilder.TagName);
                    writer.Write('>');
                    break;
            }
#endif
        }

        public static bool BeginsWithTag(string html, string tag)
        {
            string delimitedTag = string.Concat("<", tag);
            return html != null && html.Length > delimitedTag.Length && html.StartsWith(delimitedTag) && (html[delimitedTag.Length] == ' ' || html[delimitedTag.Length] == '>');
        }

        public static bool AnyValue<TItem, TProperty>(IEnumerable<TItem> items, Func<TItem, TProperty> propertyGetter, bool allowEmptyCollections = false)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    var value = propertyGetter(item);
                    if (object.Equals(value, null) == false)
                    {
                        if (allowEmptyCollections == false)
                        {
                            var enumerable = value as IEnumerable;
                            if (enumerable != null && enumerable.GetEnumerator().MoveNext() == false)
                            {
                                continue;
                            }
                        }
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public static class HelpersExtensions
    {
     


        public static string GetHtml(this TableCustomColumnSettings columnSettings, object row, HtmlHelper htmlHelper, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)

        {
            IEqualityComparer<object> distinctComparer = null;
            if (columnSettings.Distinct == true)
            {
                var cultureInfo = formatProvider as CultureInfo;
                distinctComparer = new EqualityComparer(EqualityComparer<object>.Default, cultureInfo != null ? StringComparer.Create(cultureInfo, false) : StringComparer.CurrentCulture);
            }
            return HtmlHelpers.GetHtml(htmlHelper, row, valueProviders: columnSettings.ValueProviders, format: columnSettings.Format, htmlTemplate: columnSettings.Template, htmlSeparator: columnSettings.HtmlSeparator, formatterContext: formatterContext, formatProvider: formatProvider, distinctComparer: distinctComparer);
        }

        protected class EqualityComparer : IEqualityComparer<object>
        {
            public EqualityComparer(IEqualityComparer<object> objectComparer, IEqualityComparer<string> stringComparer)
            {
                ObjectComparer = objectComparer;
                StringComparer = stringComparer;
            }
            public IEqualityComparer<object> ObjectComparer { get; private set; }

            public IEqualityComparer<string> StringComparer { get; private set; }

            public new bool Equals(object x, object y)
            {
                string sx = x as string;
                if (sx != null)
                {
                    string sy = y as string;
                    if (sy != null)
                    {
                        return StringComparer.Equals(sx, sy);
                    }
                }

                return ObjectComparer.Equals(x, y);
            }

            public int GetHashCode(object obj)
            {
                string s = obj as string;
                return s != null ? StringComparer.GetHashCode(s) : ObjectComparer.GetHashCode(obj);
            }
        }

        //public static void ApplyOn(this MenuItemSettings setting, MenuItem menuItem, IApplicationRequestContext requestContext)
        //{
        //    if (setting.CheckGroup != null) menuItem.CheckGroup = setting.CheckGroup;
        //    if (setting.Checked.HasValue) menuItem.Checked = setting.Checked.Value;
        //    if (setting.CheckType.HasValue) menuItem.CheckType = setting.CheckType.Value;
        //    if (setting.Disabled.HasValue) menuItem.Disabled = setting.Disabled.Value;
        //    if (setting.Icon != null) menuItem.Icon = requestContext.SiteContext.GetIcon(setting.Icon);
        //    if (setting.Ignore.HasValue) menuItem.Ignore = setting.Ignore.Value;
        //    if (setting.Label != null) menuItem.Label = requestContext.GetApplicationTextTranslation(setting.Label);
        //    if (setting.Selected.HasValue) menuItem.Selected = setting.Selected.Value;
        //    //if (setting.SubMenu != null) menuItem.Items != null ? menuItem.Items = menuItem.Items.Concat(setting.SubMenu.Items.ToArray()).ToArray() : setting.SubMenu.Items.ToArray(); // TODO NOT CORRECT
        //    if (setting.Url != null) menuItem.Url = setting.Url;
        //    if (setting.UrlTarget != null) menuItem.UrlTarget = setting.UrlTarget;
        //}

        //public static void ApplyOn(this MenuItemSettings setting, ToolBarItem menuItem, IApplicationRequestContext requestContext)
        //{
        //    if (setting.CheckGroup != null) menuItem.CheckGroup = setting.CheckGroup;
        //    if (setting.Checked.HasValue) menuItem.Checked = setting.Checked.Value;
        //    if (setting.CheckType.HasValue) menuItem.CheckType = setting.CheckType.Value;
        //    if (setting.Icon != null) menuItem.Icon = requestContext.SiteContext.GetIcon(setting.Icon);
        //    if (setting.Ignore.HasValue) menuItem.Ignore = setting.Ignore.Value;
        //    if (setting.Label != null) menuItem.Label = requestContext.GetApplicationTextTranslation(setting.Label);
        //    //if (setting.SubMenu != null) menuItem.Items != null ? menuItem.Items = menuItem.Items.Concat(setting.SubMenu.Items.ToArray()).ToArray() : setting.SubMenu.Items.ToArray(); // TODO NOT CORRECT
        //    if (setting.Url != null) menuItem.Url = setting.Url;
        //    if (setting.UrlTarget != null) menuItem.UrlTarget = setting.UrlTarget;
        //}

        //public static IEnumerable<PropertyDescriptor> Apply(this PropertiesSettings settings, IEnumerable<PropertyDescriptor> properties, object obj, ApplicationRequestContext requestContext)
        //{
        //    if (settings.DisplaySpecifications == false)
        //    {
        //        properties = properties.Where(p => (p.Source is ISpecificationViewModel) == false);
        //    }

        //    var appliedProperties = settings.Properties.Apply(properties, obj, requestContext).AsArray();

        //    return appliedProperties;
        //}

        //public static IEnumerable<PropertyDescriptor> ApplyInDisplay(this PropertiesSettingsDictionary dictionary,  displayMode, IEnumerable<PropertyDescriptor> properties, object obj, IApplicationRequestContext requestContext)
        //{
        //    if (dictionary.Count == 0)
        //    {
        //        if (dictionary.DefaultSettings != null && dictionary.DefaultSettings.Properties.Count > 0)
        //        {
        //            return displayMode.HasValue == false || displayMode.Value.HasFlag(dictionary.DefaultSettings.Layout) == true ? dictionary.DefaultSettings.Properties.Apply(properties, obj, requestContext) : Enumerable.Empty<PropertyDescriptor>();
        //        }
        //        else
        //        {
        //            return displayMode.HasValue == false ? properties : Enumerable.Empty<PropertyDescriptor>();
        //        }
        //    }

        //    foreach (var setting in dictionary)
        //    {
        //        if (setting.DisplayIn(displayMode, ComponentLayoutMode.Body) == true)
        //        {
        //            return setting.Properties.Apply(properties, obj, requestContext);
        //        }
        //    }

        //    return Enumerable.Empty<PropertyDescriptor>();
        //}


        public static string GetHtml(this PropertyDescriptorSettings settings, HtmlHelper htmlHelper, object obj, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)

        {
            return HtmlHelpers.GetHtml(htmlHelper, obj, valueProviders: settings.ValueProviders, format: settings.Format, htmlTemplate: settings.CustomView, htmlSeparator: settings.HtmlSeparator, formatterContext: formatterContext, formatProvider: formatProvider);
        }

        //public static PropertyDescriptor Create(this PropertyDescriptorSettings settings, object obj, IApplicationRequestContext requestContext)
        //{
        //    var propertyDescriptor = new PropertyDescriptor { Id = settings.Id };
        //    settings.Apply(propertyDescriptor, requestContext);
        //    //propertyDescriptor.Source = obj;
        //    propertyDescriptor.Content = new HtmlContent(h => MvcHtmlString.Create(settings.GetHtml(h, obj)));
        //    return propertyDescriptor;
        //}

        //public static void Apply<T>(this PropertyDescriptorSettings settings, T propertyDescriptor, IApplicationRequestContext requestContext)
        //    where T : IPropertyDescriptor
        //{
        //    propertyDescriptor.Settings = settings;

        //    if (settings.Layout.HasValue == true)
        //    {
        //        propertyDescriptor.Layout = settings.Layout;
        //    }

        //    if (settings.Label != null)
        //    {
        //        propertyDescriptor.Name = requestContext.GetApplicationTextTranslation(settings.Label);
        //    }

        //    if (settings.Class != null)
        //    {
        //        propertyDescriptor.ClassName = propertyDescriptor.ClassName == null ? settings.Class : propertyDescriptor.ClassName + " " + settings.Class;
        //    }

        //    if (settings.Format != null && propertyDescriptor.Value != null) // TODO this should be lifted inte the property descriptor itself
        //    {
        //        propertyDescriptor.Value = settings.Format.GetFormattedValue(propertyDescriptor.Value);
        //    }

        //    if (settings.DisplayOrder.HasValue == true)
        //    {
        //        propertyDescriptor.Order = settings.DisplayOrder;
        //    }
        //}

        //public static IEnumerable<T> Apply<T>(this PropertyDescriptorSettingsDictionary directory, IEnumerable<T> properties, object obj, IApplicationRequestContext requestContext)
        //    where T : class, IPropertyDescriptor
        //{
        //    if (directory.Count == 0) return properties ?? Enumerable.Empty<T>();

        //    var propertiesCollection = properties as ICollection<IPropertyDescriptor>;
        //    var appliedProperties = propertiesCollection != null ? new List<T>(propertiesCollection.Count) : new List<T>();
        //    bool hasOrder = false;

        //    if (properties != null)
        //    {
        //        var wildcardSetting = directory.GetItem("*");

        //        foreach (var p in properties)
        //        {
        //            T copy = null;
        //            if (wildcardSetting != null) // TODO
        //            {
        //                copy = (T)p.Copy();
        //                wildcardSetting.Apply(copy, requestContext);
        //                if (copy.Order.HasValue == true)
        //                {
        //                    hasOrder = true;
        //                }
        //            }

        //            if (p.Id != null)
        //            {
        //                var settings = directory.GetItem(p.Id);

        //                if (settings != null)
        //                {
        //                    if ((settings.Layout.HasValue == false || settings.Layout.Value.HasFlag(ComponentLayoutMode.Hidden) == false) && settings.New == false)
        //                    {
        //                        copy = copy ?? (T)p.Copy();
        //                        settings.Apply(copy, requestContext);
        //                        if (copy.Order.HasValue == true)
        //                        {
        //                            hasOrder = true;
        //                        }
        //                    }
        //                    else if (copy == null)
        //                    {
        //                        continue;
        //                    }
        //                }
        //            }

        //            if (copy != null)
        //            {
        //                appliedProperties.Add(copy);
        //            }
        //            else if (directory.IsAnyHidden == true)
        //            {
        //                appliedProperties.Add(p);
        //            }
        //        }
        //    }

        //    foreach (var s in directory)
        //    {
        //        if (s.New == true || s.Id == null)
        //        {
        //            if (s.HideEmpty == true && s.ValueProviders?.Count > 0)
        //            {
        //                var formatterContext = new FormatterContext(requestContext);
        //                if (s.ValueProviders.Any(vp => string.IsNullOrWhiteSpace(vp.GetFormattedValue(obj, ",", formatterContext)) == false) == false) continue;
        //            }

        //            var p = (T)s.Create(obj, requestContext);
        //            appliedProperties.Add(p);
        //            if (p.Order.HasValue == true)
        //            {
        //                hasOrder = true;
        //            }
        //        }
        //    }

        //    if (hasOrder == true)
        //    {
        //        appliedProperties.Sort(p => p.Order ?? 0);
        //    }

        //    return appliedProperties;
        //}
    }


    public interface IAddToAttributes
    {
        AttributeBuilder AddToAttributes(AttributeBuilder attributeBuilder, object obj = null, IFormatProvider formatProvider = null);
    }
    public class AttributeBuilder : IHtmlString, IEnumerable<KeyValuePair<string, object>>

    {
        public class ClassList : List<string>
        {
            public ClassList() { }

            public ClassList(int capacity) : base(capacity) { }

            public ClassList(IEnumerable<string> items) : base(items) { }

            public bool HasClass(string className)
            {
                if (Count == 0) return false;
                foreach (string classNameEntry in this)
                {
                    int index = classNameEntry.IndexOf(className, StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                    {
                        if (classNameEntry.Length == className.Length) return true; // Early exit
                        if (index == 0 || classNameEntry[index - 1] == ' ') // Ok start  
                        {
                            int endIndex = index + className.Length;
                            if (endIndex == classNameEntry.Length || classNameEntry[endIndex] == ' ')
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            public override string ToString()
            {
                return string.Join(" ", (IEnumerable<string>)this);
            }

            public static implicit operator string(ClassList classList)
            {
                return classList.ToString();
            }
        }

        public AttributeBuilder()
        {
            attributes = new List<KeyValuePair<string, object>>();
        }

        public AttributeBuilder(string className)
        {
            attributes = new List<KeyValuePair<string, object>>();
            if (string.IsNullOrWhiteSpace(className) == false)
            {
                classElementIndex = 0;
                this.attributes.Add(new KeyValuePair<string, object>("class", className));
            }
        }

        public AttributeBuilder(string className1, string className2)
        {
            attributes = new List<KeyValuePair<string, object>>();
            var list = new ClassList(2);
            if (string.IsNullOrWhiteSpace(className1) == false) list.Add(className1);
            if (string.IsNullOrWhiteSpace(className2) == false) list.Add(className2);
            if (list.Count > 0)
            {
                classElementIndex = 0;
                attributes.Add(new KeyValuePair<string, object>("class", list));
            }
        }

        public AttributeBuilder(string className1, string className2, string className3)
        {
            attributes = new List<KeyValuePair<string, object>>();
            var list = new ClassList(3);
            if (string.IsNullOrWhiteSpace(className1) == false) list.Add(className1);
            if (string.IsNullOrWhiteSpace(className2) == false) list.Add(className2);
            if (string.IsNullOrWhiteSpace(className3) == false) list.Add(className3);
            if (list.Count > 0)
            {
                classElementIndex = 0;
                attributes.Add(new KeyValuePair<string, object>("class", list));
            }
        }

        public AttributeBuilder(string className1, string className2, string className3, string className4)
        {
            attributes = new List<KeyValuePair<string, object>>();
            var list = new ClassList(4);
            if (string.IsNullOrWhiteSpace(className1) == false) list.Add(className1);
            if (string.IsNullOrWhiteSpace(className2) == false) list.Add(className2);
            if (string.IsNullOrWhiteSpace(className3) == false) list.Add(className3);
            if (string.IsNullOrWhiteSpace(className4) == false) list.Add(className4);
            if (list.Count > 0)
            {
                classElementIndex = 0;
                attributes.Add(new KeyValuePair<string, object>("class", list));
            }
        }

        public AttributeBuilder(IEnumerable<string> classNames)
        {
            attributes = new List<KeyValuePair<string, object>>();

            var list = new ClassList();
            foreach (string className in classNames)
            {
                if (string.IsNullOrWhiteSpace(className) == false)
                {
                    list.Add(className);
                }
            }

            if (list.Count > 0)
            {
                classElementIndex = 0;
                attributes.Add(new KeyValuePair<string, object>("class", list));
            }

            //string classNamesStr = null;

            //if (classNames != null)
            //{
            //    if (classNames.Length < 5)
            //    {
            //        foreach (var className in classNames)
            //        {
            //            if (string.IsNullOrWhiteSpace(className) == false)
            //            {
            //                classNamesStr = classNamesStr == null ? className : string.Concat(classNamesStr, " ", className);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        var stringBuilder = new StringBuilder();
            //        foreach (var className in classNames)
            //        {
            //            if (string.IsNullOrWhiteSpace(className) == false)
            //            {
            //                if (stringBuilder.Length > 0)
            //                {
            //                    stringBuilder.Append(' ');
            //                }
            //                stringBuilder.Append(className);
            //            }
            //        }
            //        classNamesStr = stringBuilder.ToString();
            //    }
            //}

            //if (classNamesStr != null)
            //{
            //    classElementIndex = 0;
            //    this.attributes.Add(new KeyValuePair<string, object>("class", classNamesStr));
            //}
        }

        public AttributeBuilder(string className, object attributes)
            : this(className)
        {
            if (attributes != null)
            {
                Attr(attributes);
            }
        }

        public AttributeBuilder(object attributes)
        {
            string className = attributes as string;
            if (className != null)
            {
                this.attributes = new List<KeyValuePair<string, object>>();
                classElementIndex = 0;
                this.attributes.Add(new KeyValuePair<string, object>("class", className));
            }
            else
            {
                var otherBuilder = attributes as AttributeBuilder;
                if (otherBuilder != null)
                {
                    classElementIndex = otherBuilder.classElementIndex;

                    if (classElementIndex != -1)
                    {
                        var classList = otherBuilder.attributes[classElementIndex].Value as ClassList;
                        if (classList != null)
                        {
                            if (otherBuilder.Count > 1)
                            {
                                this.attributes = new List<KeyValuePair<string, object>>(otherBuilder.attributes);
                                this.attributes[classElementIndex] = new KeyValuePair<string, object>("class", new ClassList(classList));
                            }
                            else
                            {
                                this.attributes = new List<KeyValuePair<string, object>>();
                                this.attributes.Add(new KeyValuePair<string, object>("class", new ClassList(classList)));
                            }
                        }
                        else
                        {
                            this.attributes = new List<KeyValuePair<string, object>>(otherBuilder.attributes);
                        }
                    }
                    else
                    {
                        this.attributes = new List<KeyValuePair<string, object>>(otherBuilder.attributes);
                    }
                }
                else
                {
                    this.attributes = new List<KeyValuePair<string, object>>();
                    if (attributes != null)
                    {
                        Attr(attributes);
                    }
                }
            }
        }

        public AttributeBuilder(IEnumerable<KeyValuePair<string, object>> attributes)
        {
            var collection = attributes as ICollection<KeyValuePair<string, object>>;
            if (collection != null)
            {
                this.attributes = new List<KeyValuePair<string, object>>(collection.Count);
                Attr(attributes);
            }
            else
            {
                this.attributes = new List<KeyValuePair<string, object>>();
                if (attributes != null)
                {
                    Attr(attributes);
                }
            }
        }

        public AttributeBuilder(AttributeBuilder otherBuilder)
            : this()
        {
            if (otherBuilder != null)
            {
                classElementIndex = otherBuilder.classElementIndex;
                if (classElementIndex != -1)
                {
                    var classList = otherBuilder.attributes[classElementIndex].Value as ClassList;
                    if (classList != null)
                    {
                        if (otherBuilder.Count > 1)
                        {
                            this.attributes = new List<KeyValuePair<string, object>>(otherBuilder.attributes);
                            this.attributes[classElementIndex] = new KeyValuePair<string, object>("class", new ClassList(classList));
                        }
                        else
                        {
                            this.attributes = new List<KeyValuePair<string, object>>();
                            this.attributes.Add(new KeyValuePair<string, object>("class", new ClassList(classList)));
                        }
                    }
                    else
                    {
                        this.attributes = new List<KeyValuePair<string, object>>(otherBuilder.attributes);
                    }
                }
                else
                {
                    this.attributes = new List<KeyValuePair<string, object>>(otherBuilder.attributes);
                }
            }
        }

        public static AttributeBuilder Create(params object[] attributes)
        {
            return AttributeBuilder.TryCreate(attributes) ?? new AttributeBuilder();
        }

        public static AttributeBuilder TryCreate(object attributes)
        {
            var kvps = attributes as IEnumerable<KeyValuePair<string, object>>;
            if (kvps != null)
            {
                return new AttributeBuilder(kvps);
            }
            else
            {
                string classNames = attributes as string;
                if (string.IsNullOrWhiteSpace(classNames) == false)
                {
                    return new AttributeBuilder(classNames);
                }
                else if (attributes != null)
                {
                    var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
                    if (dict.Count > 0)
                    {
                        return new AttributeBuilder(dict);
                    }
                }
            }

            return null;
        }

        public static AttributeBuilder TryCreate(params object[] attributes)
        {
            return AttributeBuilder.TryCreate(null, attributes);
        }

        /// <summary>
        /// Creates an AttributeBuilder if any passed in attribute objects contains any items. Othwerwise null is returned.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static AttributeBuilder TryCreate(HtmlHelper htmlHelper, params object[] attributes)
        {          
            AttributeBuilder builder = null;
            for (int index = 0; index < attributes.Length; index++)
            {
                var attr = attributes[index];

                var kvps = attr as IEnumerable<KeyValuePair<string, object>>;
                if (kvps != null)
                {
                    var kvpCollection = kvps as ICollection<KeyValuePair<string, object>>;
                    if (kvpCollection == null || kvpCollection.Count > 0)
                    {
                        if (builder == null)
                        {
                            builder = new AttributeBuilder(kvps);
                        }
                        else
                        {
                            builder.Attr(kvps);
                        }
                    }
                }
                else if (attr != null)
                {
                    var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(attr);
                    if (dict.Count > 0)
                    {
                        if (builder == null)
                        {
                            builder = new AttributeBuilder(dict);
                        }
                        else
                        {
                            builder.Attr(dict);
                        }
                    }
                }
            }
            return builder;
        }

        private List<KeyValuePair<string, object>> attributes;

        private int classElementIndex = -1;

        public int Count { get { return attributes.Count; } }

        private void Add(string name, object value)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(name, "class") == false)
            {
                attributes.Add(new KeyValuePair<string, object>(name, value));
            }
            else
            {
                Class(value as string ?? (value != null ? value.ToString() : null));
            }

        }

        public object Attr(string name)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            for (int i = 0; i < attributes.Count; i++)
            {
                var attr = attributes[i];
                if (comparer.Equals(attr.Key, name) == true) return attr.Value;
            }
            return null;
        }

        public bool HasAttr(string name)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            for (int i = 0; i < attributes.Count; i++)
            {
                if (comparer.Equals(attributes[i].Key, name) == true) return true;
            }
            return false;
        }
		
		public bool HasAnyClass
        {
            get
            {
                if (classElementIndex >= 0)
                {
                    var classElement = attributes[classElementIndex];
                    var containerString = classElement.Value as string;
                    if (containerString != null)
                    {
                        return string.IsNullOrWhiteSpace(containerString) == false;
                    }
                    else
                    {
                        var list = (ClassList)classElement.Value;
                        foreach (string classListEntry in list)
                        {
                            if (string.IsNullOrWhiteSpace(classListEntry) == false) return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool HasClass(string className)
        {
            if (classElementIndex >= 0)
            {
                var comparer = StringComparer.OrdinalIgnoreCase;
                var classElement = attributes[classElementIndex];

                var containerString = classElement.Value as string;
                if (containerString != null)
                {
                    string[] currentClassNames = containerString.Split(' ');
                    foreach (var currentClassName in currentClassNames)
                    {
                        if (comparer.Equals(currentClassName, className) == true) return true;
                    }
                }
                else
                {
                    var list = (ClassList)classElement.Value;
                    foreach (string classListEntry in list)
                    {
                        string[] currentClassNames = classListEntry.Split(' ');
                        foreach (var currentClassName in currentClassNames)
                        {
                            if (comparer.Equals(currentClassName, className) == true) return true;
                        }
                    }
                }
            }
            return false;
        }

        public string Class()
        {
            if (classElementIndex >= 0)
            {
                var classElement = attributes[classElementIndex];
                //var containerString = classElement.Value as string;
                //return containerString ?? string.Join(" ", (List<string>)classElement.Value);
                string classNames = classElement.Value as string ?? (classElement.Value != null ? classElement.Value.ToString() : null);
                return classNames;
            }
            return null;
        }

        public AttributeBuilder Class(string className)
        {
            if (className == null) return this;
            if (classElementIndex >= 0)
            {
                var classElement = attributes[classElementIndex];

                ClassList list;
                var containerString = classElement.Value as string;
                if (containerString != null)
                {
                    list = new ClassList();
                    list.Add(containerString);
                    if (list.HasClass(className) == false)
                    {
                        list.Add(className);
                    }
                }
                else
                {
                    list = (ClassList)classElement.Value;
                    if (list.HasClass(className) == false)
                    {
                        list.Add(className);
                    }
                }

                attributes[classElementIndex] = new KeyValuePair<string, object>(classElement.Key, list);

                //string classNames = classElement.Value as string ?? (classElement.Value != null ? classElement.ToString() : null);
                //string newClassNames = classNames != null ? string.Concat(classNames, " ", className) : className;
                //attributes[classElementIndex] = new KeyValuePair<string, object>(classElement.Key, newClassNames);
            }
            else
            {
                classElementIndex = attributes.Count;
                attributes.Add(new KeyValuePair<string, object>("class", className));
            }
            return this;
        }

        public AttributeBuilder Class(string className, Func<bool> predicate)
        {
            if (className == null || (predicate != null && predicate() == false)) return this;
            if (classElementIndex >= 0)
            {
                var classElement = attributes[classElementIndex];

                ClassList list;
                var containerString = classElement.Value as string;
                if (containerString != null)
                {
                    list = new ClassList();
                    list.Add(containerString);
                    list.Add(className);
                }
                else
                {
                    list = (ClassList)classElement.Value;
                    list.Add(className);
                }

                attributes[classElementIndex] = new KeyValuePair<string, object>(classElement.Key, list);

                //string classNames = classElement.Value as string ?? (classElement.Value != null ? classElement.ToString() : null);
                //string newClassNames = classNames != null ? string.Concat(classNames, " ", className) : className;
                //attributes[classElementIndex] = new KeyValuePair<string, object>(classElement.Key, newClassNames);
            }
            else
            {
                classElementIndex = attributes.Count;
                attributes.Add(new KeyValuePair<string, object>("class", className));
            }
            return this;
        }

        public AttributeBuilder Class(Func<string> className, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return Class(className());
        }

        public AttributeBuilder Attr(string name, object value)
        {
            if (value == null || string.IsNullOrEmpty(name) == true) return this;
            Add(name, value);
            return this;
        }

        public AttributeBuilder Attr(string name, Func<object> value, Func<bool> predicate)
        {
            if (value == null || string.IsNullOrEmpty(name) == true || (predicate != null && predicate() == false)) return this;
            object val = value();
            if (val == null) return null;
            Add(name, val.ToString());
            return this;
        }

        public AttributeBuilder Attr(string name, Func<object> value, bool predicate)
        {
            if (value == null || string.IsNullOrEmpty(name) == true || predicate == false) return this;
            object val = value();
            if (val == null) return null;
            Add(name, val.ToString());
            return this;
        }

        public AttributeBuilder Attr(object attributes)
        {
            var keyValuePairs = attributes as IEnumerable<KeyValuePair<string, object>>;
            if (keyValuePairs != null)
            {
                Attr(keyValuePairs);
            }
            else if (attributes != null)
            {
                Attr(HtmlHelper.AnonymousObjectToHtmlAttributes(attributes));
            }
            return this;
        }

        //public AttributeBuilder Attr(IDictionary<string, object> attributes)
        //{
        //    foreach (var kvp in attributes)
        //    {
        //        Attr(kvp.Key, kvp.Value);
        //    }
        //    return this;
        //}

        public AttributeBuilder Attr(IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    Attr(kvp.Key, kvp.Value);
                }
            }
            return this;
        }

        public AttributeBuilder Attr(AttributeBuilder attributes)
        {
            if (attributes != null)
            {
                foreach (var kvp in attributes.attributes)
                {
                    Attr(kvp.Key, kvp.Value);
                }
            }
            return this;
        }

        public AttributeBuilder Attr(string name1, object value1, string name2, object value2, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return Attr(name1, value1).DataAttr(name2, value2);
        }

        public AttributeBuilder DataAttr(string name, object value, Func<bool> predicate = null)
        {
            if (value == null || string.IsNullOrEmpty(name) == true || (predicate != null && predicate() == false)) return this;
            Add(name.StartsWith("data-") == false ? string.Concat("data-", name) : name, value);
            return this;
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5).DataAttr(name6, value6);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5).DataAttr(name6, value6).DataAttr(name7, value7);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, string name8, object value8, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5).DataAttr(name6, value6).DataAttr(name7, value7).DataAttr(name8, value8);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, string name8, object value8, string name9, object value9, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5).DataAttr(name6, value6).DataAttr(name7, value7).DataAttr(name8, value8).DataAttr(name9, value9);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, string name8, object value8, string name9, object value9, string name10, object value10, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5).DataAttr(name6, value6).DataAttr(name7, value7).DataAttr(name8, value8).DataAttr(name9, value9).DataAttr(name10, value10);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, string name8, object value8, string name9, object value9, string name10, object value10,string name11, object value11, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5).DataAttr(name6, value6).DataAttr(name7, value7).DataAttr(name8, value8).DataAttr(name9, value9).DataAttr(name10, value10).DataAttr(name11, value11);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, string name8, object value8, string name9, object value9, string name10, object value10, string name11, object value11, string name12, object value12, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5).DataAttr(name6, value6).DataAttr(name7, value7).DataAttr(name8, value8).DataAttr(name9, value9).DataAttr(name10, value10).DataAttr(name11, value11).DataAttr(name12, value12);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, string name8, object value8, string name9, object value9, string name10, object value10, string name11, object value11, string name12, object value12,string name13, object value13, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5).DataAttr(name6, value6).DataAttr(name7, value7).DataAttr(name8, value8).DataAttr(name9, value9).DataAttr(name10, value10).DataAttr(name11, value11).DataAttr(name12, value12).DataAttr(name13,value13);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, string name8, object value8, string name9, object value9, string name10, object value10, string name11, object value11, string name12, object value12, string name13, object value13, string name14, object value14, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5).DataAttr(name6, value6).DataAttr(name7, value7).DataAttr(name8, value8).DataAttr(name9, value9).DataAttr(name10, value10).DataAttr(name11, value11).DataAttr(name12, value12).DataAttr(name13,value13).DataAttr(name14,value14);
        }

        public AttributeBuilder DataAttr(string name1, object value1, string name2, object value2, string name3, object value3, string name4, object value4, string name5, object value5, string name6, object value6, string name7, object value7, string name8, object value8, string name9, object value9, string name10, object value10, string name11, object value11, string name12, object value12, string name13, object value13, string name14, object value14, string name15, object value15, Func<bool> predicate = null)
        {
            if (predicate != null && predicate() == false) return this;
            return DataAttr(name1, value1).DataAttr(name2, value2).DataAttr(name3, value3).DataAttr(name4, value4).DataAttr(name5, value5).DataAttr(name6, value6).DataAttr(name7, value7).DataAttr(name8, value8).DataAttr(name9, value9).DataAttr(name10, value10).DataAttr(name11, value11).DataAttr(name12, value12).DataAttr(name13, value13).DataAttr(name14, value14).DataAttr(name15, value15);
        }

        public AttributeBuilder Attr(IAddToAttributes attributeAdder, Func<bool> predicate = null, object obj = null,  IFormatProvider formatProvider = null)
        {
            if (attributeAdder == null || (predicate != null && predicate() == false)) return this;
            return attributeAdder.AddToAttributes(this, obj,  formatProvider);
        }

        public string AttributeEncode(string value)
        {
            return (string.IsNullOrEmpty(value) == false) ? HttpUtility.HtmlAttributeEncode(value) : string.Empty;
        }

        public string AttributeEncode(object value)
        {
            string stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue.Length > 0)
                {
                    return HttpUtility.HtmlAttributeEncode(stringValue);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);
                if (stringValue.Length > 0)
                {
                    return HttpUtility.HtmlAttributeEncode(stringValue);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public void AttributeEncode(string value, TextWriter output)
        {
            HttpUtility.HtmlAttributeEncode(value, output);
        }

        public void AttributeEncode(object value, TextWriter output)
        {
            string stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue.Length > 0)
                {
                    HttpUtility.HtmlAttributeEncode(stringValue, output);
                }

            }
            else
            {
                stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);
                if (stringValue.Length > 0)
                {
                    HttpUtility.HtmlAttributeEncode(stringValue, output);
                }

            }
        }

        public string ToHtmlString()
        {
            return ToHtmlString((object)null);
        }

        public string ToHtmlString(object otherAttributes)
        {
            if (attributes.Count > 1 || otherAttributes != null)
            {
                string otherClasses = null;

                IDictionary<string, object> otherAttributeDictionary = null;
                AttributeBuilder otherAttributeBuilder = null;

                if (otherAttributes != null)
                {
                    otherAttributeBuilder = otherAttributes as AttributeBuilder;
                    if (otherAttributeBuilder != null)
                    {
                        otherClasses = otherAttributeBuilder.Class();
                    }
                    else
                    {
                        otherClasses = otherAttributes as string;
                        if (otherClasses == null)
                        {
                            otherAttributeDictionary = otherAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(otherAttributeDictionary);
                            object otherClassObj;
                            if (otherAttributeDictionary.TryGetValue("class", out otherClassObj) == true)
                            {
                                otherClasses = otherClassObj as string ?? (otherClassObj != null ? otherClassObj.ToString() : null);
                            }
                        }
                    }
                }

                var attrStringBuilder = new StringBuilder();
                for (int i = 0, c = attributes.Count; i < c; i++)
                {
                    var attr = attributes[i];
                    if (i > 0) attrStringBuilder.Append(' ');

                    string encodedValue = AttributeEncode(attr.Value);
                    if (i == classElementIndex && otherClasses != null)
                    {
                        encodedValue = string.Concat(encodedValue, " ", otherClasses);
                        otherClasses = null;
                    }

                    attrStringBuilder.Append(attr.Key).Append("=\"").Append(encodedValue).Append('"');
                }

                if (otherAttributeBuilder != null)
                {
                    for (int i = 0, c = otherAttributeBuilder.attributes.Count; i < c; i++)
                    {
                        if (otherClasses == null && i == otherAttributeBuilder.classElementIndex) continue;
                        if (attrStringBuilder.Length == 0) attrStringBuilder.Append(' ');
                        var attr = otherAttributeBuilder.attributes[i];
                        string encodedValue = AttributeEncode(attr.Value);
                        attrStringBuilder.Append(attr.Key).Append("=\"").Append(encodedValue).Append('"');
                    }
                }
                else if (otherAttributeDictionary != null)
                {
                    foreach (var kvp in otherAttributeDictionary)
                    {
                        if (otherClasses == null && StringComparer.OrdinalIgnoreCase.Equals(kvp.Key, "class")) continue;
                        if (attrStringBuilder.Length == 0) attrStringBuilder.Append(' ');
                        string encodedValue = AttributeEncode(kvp.Value);
                        attrStringBuilder.Append(kvp.Key).Append("=\"").Append(encodedValue).Append('"');
                    }
                }
                else if (otherClasses != null)
                {
                    if (attrStringBuilder.Length == 0) attrStringBuilder.Append(' ');
                    attrStringBuilder.Append("class=\"").Append(otherClasses).Append('"');
                }

                return attrStringBuilder.ToString();
                //return htmlHelper.Encode(attrStringBuilder.ToString());
            }
            else if (attributes.Count == 1)
            {
                var attr = attributes[0];
                return string.Concat(attr.Key, "=\"", AttributeEncode(attr.Value), '"');
                //return htmlHelper.Encode(string.Concat(attr.Key, "=\"", attr.Value, "\""));
            }

            return string.Empty;
        }

#if NET5
        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            WriteTo(writer, otherAttributes: null);
        }
#else
        public IHtmlString ToHtmlString(System.Web.WebPages.Html.HtmlHelper htmlHelper)
        {
            if (attributes.Count > 1)
            {
                var attrStringBuilder = new StringBuilder();
                for (int i = 0, c = attributes.Count; i < c; i++)
                {
                    var attr = attributes[i];
                    if (i > 0) attrStringBuilder.Append(' ');
                    attrStringBuilder.Append(attr.Key).Append("=\"").Append(htmlHelper.AttributeEncode(attr.Value)).Append('"');
                }
                return new MvcHtmlString(attrStringBuilder.ToString());
                //return htmlHelper.Encode(attrStringBuilder.ToString());
            }
            else if (attributes.Count == 1)
            {
                var attr = attributes[0];
                return new MvcHtmlString(string.Concat(attr.Key, "=\"", htmlHelper.AttributeEncode(attr.Value), '"'));
                //return htmlHelper.Encode(string.Concat(attr.Key, "=\"", attr.Value, "\""));
            }

            return new MvcHtmlString(string.Empty);
        }
#endif

        public void WriteTo(TextWriter writer, AttributeBuilder otherAttributes = null)
        {
            int otherClassIndex = otherAttributes != null ? otherAttributes.classElementIndex : -1;

            for (int i = 0, c = attributes.Count; i < c; i++)
            {
                var attr = attributes[i];
                if (i > 0) writer.Write(' ');
                writer.Write(attr.Key);
                writer.Write("=\"");
                if (classElementIndex != i || otherClassIndex == -1)
                {
                    AttributeEncode(attr.Value, writer);
                    //writer.Write(AttributeEncode(attr.Value));
                }
                else
                {
                    string otherClassNames = AttributeEncode(otherAttributes.attributes[otherClassIndex].Value);
                    string combinedClassNames = string.Concat(AttributeEncode(attr.Value), " ", otherClassNames);
                    writer.Write(combinedClassNames);
                    otherClassIndex = -1; // mark that we already printent a class attribute
                }
                writer.Write('"');
            }

            if (otherAttributes != null)
            {
                for (int i = 0, c = otherAttributes.Count; i < c; i++)
                {
                    if (i == otherAttributes.classElementIndex && otherClassIndex == -1) continue;
                    var attr = otherAttributes.attributes[i];
                    if (i > 0 || attributes.Count > 0) writer.Write(' ');
                    writer.Write(attr.Key);
                    writer.Write("=\"");
                    //writer.Write(AttributeEncode(attr.Value));
                    AttributeEncode(attr.Value, writer);
                    writer.Write('"');
                }
            }
        }

        public RouteValueDictionary ToHtmlAttributes(object otherHtmlAttributes = null)
        {
            var otherAttributeBuilder = otherHtmlAttributes as AttributeBuilder;

            RouteValueDictionary dictionary;
            if (otherAttributeBuilder == null)
            {
                dictionary = new RouteValueDictionary(otherHtmlAttributes);
                AddToDictionary(dictionary);
            }
            else
            {
                dictionary = new RouteValueDictionary();
                AddToDictionary(dictionary);
                otherAttributeBuilder.AddToDictionary(dictionary);
            }
            return dictionary;
        }

        public void AddToDictionary(IDictionary<string, object> dictionary)
        {
            for (int i = 0, c = attributes.Count; i < c; i++)
            {
                var attr = attributes[i];
                if (i != classElementIndex)
                {
                    dictionary[attr.Key] = attr.Value;
                }
                else if (attr.Value != null)
                {
                    string className = attr.Value as string ?? attr.Value.ToString();
                    object otherClassNames;
                    if (dictionary.TryGetValue("class", out otherClassNames) == true)
                    {
                        string classNames = otherClassNames as string ?? (otherClassNames != null ? otherClassNames.ToString() : null);
                        string newClassNames = classNames != null ? string.Concat(classNames, " ", className) : className;
                        dictionary[attr.Key] = newClassNames;
                    }
                    else
                    {
                        dictionary[attr.Key] = className;
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return attributes.GetEnumerator();
        }
    }
}