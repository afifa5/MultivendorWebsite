using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#if NET452
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
#endif
#if NET5
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif
using MultivendorWebViewer.Common;


namespace MultivendorWebViewer.Helpers
{
    public static class CheckBoxHelper
    {
        public static MvcHtmlString CheckBoxExt(this HtmlHelper htmlHelper, string name = null, bool isChecked = false, bool isEnabled = true, object htmlAttributes = null)
        {
            var stringBuilder = new StringBuilder();
            using (var htmlWriter = new StringWriter(stringBuilder))
            {
                CheckBoxHelper.CheckBoxInternal(htmlHelper, htmlWriter, "", name, isChecked, isEnabled, htmlAttributes);
            }
            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        public static MvcHtmlString Switch(this HtmlHelper htmlHelper, string name = null, bool isChecked = false, bool isEnabled = true, object htmlAttributes = null)
        {
            var stringBuilder = new StringBuilder();
            using (var htmlWriter = new StringWriter(stringBuilder))
            {
                CheckBoxHelper.CheckBoxInternal(htmlHelper, htmlWriter, "switch", name, isChecked, isEnabled, htmlAttributes);
            }
            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        internal static void CheckBoxInternal(HtmlHelper htmlHelper, TextWriter htmlWriter, string type, string name, bool isChecked, bool isEnabled, object htmlAttributes)
        {
            IDictionary<string, object> htmlAttributesDict;
            var attributesBuilder = htmlAttributes as AttributeBuilder;
            if (attributesBuilder != null)
            {
                htmlAttributesDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                attributesBuilder.AddToDictionary(htmlAttributesDict);
            }
            else
            {
                var htmlAttributesArg = htmlAttributes as IDictionary<string, object>;
                if (htmlAttributesArg != null)
                {
                    htmlAttributesDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    foreach (var kvp in htmlAttributesArg)
                    {
                        htmlAttributesDict[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    htmlAttributesDict = Helpers.GetHtmlAttributes(htmlAttributes);
                }
            }

            if (!isEnabled)
            {
                htmlAttributesDict["disabled"] = "disabled";
            }
            string classNames = "multivendor-checkbox " + type ?? string.Empty;
            Helpers.MergeClassAttribute(htmlAttributesDict, classNames);

            if (name != null)
            {
#if NET5
                htmlHelper.CheckBox(name, isChecked, htmlAttributesDict).WriteTo(htmlWriter, System.Text.Encodings.Web.HtmlEncoder.Default);
#else
                htmlWriter.Write(InputExtensions.CheckBox(htmlHelper, name, isChecked, htmlAttributesDict).ToString());
#endif
            }
            else
            {
                if (isChecked == true)
                {
                    htmlWriter.Write("<input type=\"checkbox\" checked");
                }
                else
                {
                    htmlWriter.Write("<input type=\"checkbox\"");
                }
               
                foreach (var kvp in htmlAttributesDict)
                {
                    htmlWriter.Write(" ");
                    htmlWriter.Write(kvp.Key);
                    if (kvp.Value != null)
                    {
                        htmlWriter.Write("=\"");
                        htmlWriter.Write(htmlHelper.AttributeEncode(kvp.Value));
                        htmlWriter.Write("\"");
                    }
                }

                htmlWriter.Write("/>");

                //var inputTag = new TagBuilder("input");
                //inputTag.MergeAttribute("type", "checkbox");
                //if (isChecked == true)
                //{
                //    inputTag.MergeAttribute("checked", "checked");
                //}
                //inputTag.MergeAttributes(htmlAttributesDict);
                //inputTag.Write(htmlWriter, TagRenderMode.SelfClosing);
            }

            htmlWriter.Write("<span></span>");


        }
    }
}