#if NET5
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
#if NET452
using System.Web.Mvc;
#endif

namespace MultivendorWebViewer.Helpers
{
    public class ExtendedSelectListItem : SelectListItem
    {
        public ExtendedSelectListItem() { }

        public ExtendedSelectListItem(SelectListItem prototype)
        {
            if (prototype != null)
            {
                Disabled = prototype.Disabled;
                Group = prototype.Group;
                Selected = prototype.Selected;
                Text = prototype.Text;
                Value = prototype.Value;
            }
        }

        public HtmlContent HtmlContent { get; set; }

        public AttributeBuilder HtmlAttributes { get; set; }

        public object DataAttributes { get; set; }

        public object Model { get; set; }

        public string FilterValue { get; set; }

        public bool AlwaysVisible { get; set; }
    }

#if NET452
    // TODO NET5 Shafi
    public static class ExtendedSelectExtensions
    {
        internal static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType)
        {
            System.Web.Mvc.ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }

        public static MvcHtmlString ExtendedDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<ExtendedSelectListItem> selectList)
        {
            return ExtendedDropDownList(htmlHelper, name, selectList, (string)null, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString ExtendedDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<ExtendedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return ExtendedDropDownListHelper(htmlHelper, null, name, selectList, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString ExtendedDropDownListHelper(this HtmlHelper htmlHelper, ModelMetadata metadata, string expression, IEnumerable<ExtendedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return SelectInternal(htmlHelper, metadata, optionLabel, expression, selectList, false, htmlAttributes);
        }

        public static MvcHtmlString ExtendedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<ExtendedSelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            //var provider = new ModelExpressionProvider(htmlHelper.MetadataProvider).CreateModelExpression(htmlHelper.ViewData, expression);
            //return SelectInternal(htmlHelper, provider.Metadata, optionLabel, htmlHelper.GetExpressionText(expression), selectList, false /* allowMultiple */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            return SelectInternal(htmlHelper, metadata, optionLabel, ExpressionHelper.GetExpressionText(expression), selectList, false /* allowMultiple */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, ModelMetadata metadata, string optionLabel, string name, IEnumerable<ExtendedSelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
                throw new ArgumentException("No name");

            if (selectList == null)
                throw new ArgumentException("No selectlist");

            object defaultValue = (allowMultiple)
                ? htmlHelper.GetModelStateValue(fullName, typeof(string[]))
                : htmlHelper.GetModelStateValue(fullName, typeof(string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (defaultValue == null)
                defaultValue = htmlHelper.ViewData.Eval(fullName);

            if (defaultValue != null)
            {
                IEnumerable defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
                IEnumerable<string> values = from object value in defaultValues
                                             select Convert.ToString(value, CultureInfo.CurrentCulture);
                HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
                List<ExtendedSelectListItem> newSelectList = new List<ExtendedSelectListItem>();

                foreach (ExtendedSelectListItem item in selectList)
                {
                    item.Selected = (item.Value != null)
                        ? selectedValues.Contains(item.Value)
                        : selectedValues.Contains(item.Text);
                    newSelectList.Add(item);
                }
                selectList = newSelectList;
            }

            // Convert each ListItem to an <option> tag
            StringBuilder listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
                listItemBuilder.Append(
                    ListItemToOption(new ExtendedSelectListItem()
                    {
                        Text = optionLabel,
                        Value = String.Empty,
                        Selected = false
                    }));

            foreach (ExtendedSelectListItem item in selectList)
            {
                listItemBuilder.Append(ListItemToOption(item));
            }

            TagBuilder tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
            tagBuilder.GenerateId(fullName);
            if (allowMultiple)
                tagBuilder.MergeAttribute("multiple", "multiple");

            // If there are any errors for a named field, we add the css attribute.

            //ModelStateEntry modelState;
            //if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))

            System.Web.Mvc.ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(fullName, metadata));

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        internal static string ListItemToOption(ExtendedSelectListItem item)
        {
            TagBuilder builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(item.HtmlAttributes));
            return builder.ToString(TagRenderMode.Normal);
        }

    }
#endif
}