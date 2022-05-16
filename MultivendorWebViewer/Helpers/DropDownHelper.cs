#if NET5
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif
using MultivendorWebViewer.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.WebPages;
#if NET452
using System.Web.Mvc;
using System.Web.WebPages;
#endif

namespace MultivendorWebViewer.Helpers
{
    public class DropDown : PopUp
    {
        public Func<object, HelperResult> Template { get; set; }

        public DropDownDirection Direction { get; set; }

        public override IDictionary<string, object> ToAttributes()
        {
            var dictionary = base.ToAttributes();
            if (Direction != DropDownDirection.Inherit) dictionary["direction"] = Direction == DropDownDirection.Down ? "down" : "up";
            return dictionary;
        }

        static DropDown()
        {
            DropDown.Default = new DropDown();
        }

        public static DropDown Default { get; set; }
    }

    public enum DropDownDirection { Inherit, Up, Down }

    public static class DropDownHelper
    {
        public static MvcHtmlString DropDown(this HtmlHelper htmlHelper, HtmlContent content = null, DropDown options = null, PopupActivation activation = null, object htmlAttributes = null)
        {
            using (var htmlWriter = new StringWriter(CultureInfo.CurrentUICulture))
            {
                DropDownHelper.DropDown(htmlHelper, htmlWriter, content, options, activation, htmlAttributes);

                return MvcHtmlString.Create(htmlWriter.ToString());
            }
        }

        public static void DropDown(this HtmlHelper htmlHelper, TextWriter htmlWriter, HtmlContent content = null, DropDown options = null, PopupActivation activation = null, object htmlAttributes = null)
        {
            if (options == null)
            {
                options = MultivendorWebViewer.Helpers.DropDown.Default;
            }

            if (options.Template != null)
            {
                content = options.Template(content).ToString();
            }

            var attributes = Helpers.GetHtmlAttributes(htmlAttributes);

            Helpers.MergeClassAttribute(attributes, "multivendor-drop-down");

            PopUpHelper.PopUp(htmlHelper, htmlWriter, content, options, activation, attributes);
        }
    }

    public class CompoBoxOptions
    {
        public bool MultiSelect { get; set; }

        public string PlaceHolder { get; set; }

        public ICollection<SelectListGroup> SelectListGroups { get; set; }

        public FilteringMode FilteringMode { get; set; }

        public string NoMatchesText { get; set; }

        public bool Disabled { get; set; }

        public bool DisableEnterKey { get; set; }

        public string SelectText { get; set; }

        public int FilterMinChars { get; set; }

        public bool AllowNew { get; set; }

        public bool Required { get; set; }

        public bool ShowAll { get; set; }

        public string TabIndex { get; set; }
    }

    public enum FilteringMode { None, Server, Client };

    public static class ComboBoxHelper
    {
        public static MvcHtmlString ComboBox(this HtmlHelper htmlHelper, string name = null, string text = null, string value = null, CompoBoxOptions options = null, object htmlAttributes = null)
        {
            using (var writer = new StringWriter())
            {
                RenderComboBoxInternal(htmlHelper, writer, name, text, value, null, null, options, htmlAttributes);
                return new MvcHtmlString(writer.ToString());
            }
        }

        public static MvcHtmlString ComboBox(this HtmlHelper htmlHelper, string name, string itemsUrl, string text = null, string value = null, CompoBoxOptions options = null, object htmlAttributes = null)
        {
            using (var writer = new StringWriter())
            {
                RenderComboBoxInternal(htmlHelper, writer, name, text, value, itemsUrl, null, options, htmlAttributes);
                return new MvcHtmlString(writer.ToString());
            }
        }

        public static MvcHtmlString ComboBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> items, string text = null, string value = null, CompoBoxOptions options = null, object htmlAttributes = null)
        {
            using (var writer = new StringWriter())
            {
                RenderComboBoxInternal(htmlHelper, writer, name, text, value, null, items, options, htmlAttributes);
                return new MvcHtmlString(writer.ToString());
            }
        }

        public static MvcHtmlString ComboBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> items, string text = null, string value = null, CompoBoxOptions options = null, object htmlAttributes = null)
        {
#if NET5
            var provider = new ModelExpressionProvider(htmlHelper.MetadataProvider).CreateModelExpression(htmlHelper.ViewData, expression);
            object model = provider.Model;
            if (model != null)
            {
                value = model.ToString();
            }
#else
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            if (metadata.Model != null)
            {
                if (value == null)
                {
                    value = metadata.Model.ToString();
                }
            }
#endif


            using (var writer = new StringWriter())
            {
#if NET5
                RenderComboBoxInternal(htmlHelper, writer, htmlHelper.GetExpressionText(expression), text, value, null, items, options, htmlAttributes);
#else
                RenderComboBoxInternal(htmlHelper, writer, ExpressionHelper.GetExpressionText(expression), text, value, null, items, options, htmlAttributes);
#endif
                return new MvcHtmlString(writer.ToString());
            }
        }

        public static MvcHtmlString ComboBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string itemLoadUrl, string text = null, string value = null, CompoBoxOptions options = null, object htmlAttributes = null)
        {
#if NET5
            var provider = new ModelExpressionProvider(htmlHelper.MetadataProvider).CreateModelExpression(htmlHelper.ViewData, expression);
            object model = provider.Model;
            if (model != null)
            {
                value = model.ToString();
            }
#else
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            if (metadata.Model != null)
            {
                if (value == null)
                {
                    value = metadata.Model.ToString();
                }
            }
#endif

            using (var writer = new StringWriter())
            {
#if NET5
                RenderComboBoxInternal(htmlHelper, writer, htmlHelper.GetExpressionText(expression), text, value, itemLoadUrl, null, options, htmlAttributes);
#else
                RenderComboBoxInternal(htmlHelper, writer, ExpressionHelper.GetExpressionText(expression), text, value, itemLoadUrl, null, options, htmlAttributes);
#endif
                return new MvcHtmlString(writer.ToString());
            }
        }

        public static void RenderComboBox(this HtmlHelper htmlHelper, TextWriter writer, string name = null, string text = null, string value = null, CompoBoxOptions options = null, object htmlAttributes = null)
        {
            RenderComboBoxInternal(htmlHelper, writer, name, text, value, null, null, options, htmlAttributes);
        }

        public static void RenderComboBox(this HtmlHelper htmlHelper, TextWriter writer, string itemsUrl, string name = null, string text = null, string value = null, CompoBoxOptions options = null, object htmlAttributes = null)
        {
            RenderComboBoxInternal(htmlHelper, writer, name, text, value, itemsUrl, null, options, htmlAttributes);
        }

        public static void RenderComboBox(this HtmlHelper htmlHelper, IEnumerable<SelectListItem> items, string name = null, string text = null, string value = null, CompoBoxOptions options = null, object htmlAttributes = null)
        {
            RenderComboBoxInternal(htmlHelper, htmlHelper.ViewContext.Writer, name, text, value, null, items, options, htmlAttributes);
        }

        public static void RenderComboBox(this HtmlHelper htmlHelper, TextWriter writer, IEnumerable<SelectListItem> items, string name = null, string text = null, string value = null, CompoBoxOptions options = null, object htmlAttributes = null)
        {
            RenderComboBoxInternal(htmlHelper, writer, name, text, value, null, items, options, htmlAttributes);
        }

        private static void RenderComboBoxInternal(HtmlHelper htmlHelper, TextWriter writer, string name, string text, string value, string itemsUrl, IEnumerable<SelectListItem> items, CompoBoxOptions options, object htmlAttributes)
        {
            if (options == null) options = new CompoBoxOptions();

            if (value == null && options.MultiSelect == false && items != null && items.Any(i => i.Selected && i.Value != null))
            {
                value = items.First(i => i.Selected && i.Value != null).Value;
            }

            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            if (text == null && items != null)
            {
                var textItem = items.Where(i => i.Selected == true && i.Text != null && i.Value == value).FirstOrDefault() ?? items.Where(i => i.Selected == true && i.Text != null).FirstOrDefault();
                if (textItem != null)
                {
                    text = textItem.Text ?? textItem.Value;
                }
            }

            var control = new TagBuilder("div");

            if (htmlAttributes != null)
            {
                var attributeBuilder = htmlAttributes as AttributeBuilder ?? new AttributeBuilder(htmlAttributes);
                control.MergeAttributes(attributeBuilder);
            }

            control.AddCssClass("multivendor-combobox");

            if (itemsUrl != null)
            {
                control.MergeAttribute("data-url", itemsUrl);
            }

            if (options.MultiSelect == true)
            {
                control.AddCssClass("multi-select");
            }

            if (options.AllowNew == true)
            {
                control.AddCssClass("allow-new");
            }

            if (options.Required == true)
            {
                control.AddCssClass("required");
            }

            if (options.ShowAll == true)
            {
                control.AddCssClass("show-all");
            }

            if (options.FilteringMode != FilteringMode.None)
            {
                control.MergeAttribute("data-filtering", options.FilteringMode.ToString().ToLowerInvariant());
            }

            if (options.FilterMinChars != 0)
            {
                control.MergeAttribute("data-min-chars", options.FilterMinChars.ToString());
            }

            control.MergeAttribute("data-enter-key-disabled", options.DisableEnterKey == true ? "true" : "false");

            control.Write(writer, TagRenderMode.StartTag);

            //if (options.FilteringMode != FilteringMode.None)
            //{
            var input = new TagBuilder("input");
            input.MergeAttribute("type", "text"); // "search"
            //input.MergeAttribute("name", fullName);
            input.MergeAttribute("value", text);
            input.MergeAttribute("autocomplete", "xyz");

            if (options.PlaceHolder != null)
            {
                input.MergeAttribute("placeholder", options.PlaceHolder);
            }

            if (options.FilteringMode == FilteringMode.None)
            {
                input.MergeAttribute("readonly", "true");
            }

            if (options.Disabled)
            {
                input.MergeAttribute("disabled", "disabled");
            }

            if (string.IsNullOrEmpty(options.TabIndex) == false)
            {
                input.MergeAttribute("tabindex", options.TabIndex);
            }

            input.Write(writer, TagRenderMode.SelfClosing);
            //}
            //else
            //{
            //    var dropDownContainer = new TagBuilder("div");

            //}

            var hiddenInput = new TagBuilder("input");
            hiddenInput.MergeAttribute("type", "hidden");
            hiddenInput.MergeAttribute("name", fullName);
            hiddenInput.MergeAttribute("value", value);
            hiddenInput.Write(writer, TagRenderMode.SelfClosing);

            writer.Write("<i></i>");

            //if (items != null)
            {
                var itemsDropDown = new TagBuilder("div");
                itemsDropDown.AddCssClass("items-dropdown");
                itemsDropDown.Write(writer, TagRenderMode.StartTag);

                writer.Write("<div>");

                if (options.FilteringMode != FilteringMode.None)
                {
                    string noMatchesString = options.NoMatchesText ?? "No search hit";
                    if (string.IsNullOrEmpty(noMatchesString) == false)
                    {
                        writer.Write("<span class=\"no-matches\">");
                        writer.Write(noMatchesString);
                        writer.Write("</span>");
                    }

                    string okString = options.SelectText ?? "Ok";
                    if (string.IsNullOrEmpty(okString) == false)
                    {
                        writer.Write("<h3 class=\"ok\">");
                        writer.Write(okString);
                        writer.Write("</h3>");
                    }
                }

                var itemsContainer = new TagBuilder("ul");
                itemsContainer.AddCssClass("items-container");

                if (options.SelectListGroups != null && options.SelectListGroups.Count > 0)
                {
                    itemsContainer.AddCssClass("groups");
                }

                itemsContainer.Write(writer, TagRenderMode.StartTag);

                if (items != null)
                {
                    RenderComboBoxItemsInternal(htmlHelper, writer, items, options);
                }

                itemsContainer.Write(writer, TagRenderMode.EndTag);

                writer.Write("</div>");

                itemsDropDown.Write(writer, TagRenderMode.EndTag);
            }

            control.Write(writer, TagRenderMode.EndTag);
        }

        public static MvcHtmlString ComboBoxItems(this HtmlHelper htmlHelper, IEnumerable<SelectListItem> items, CompoBoxOptions options = null)
        {
            using (var writer = new StringWriter())
            {
                RenderComboBoxItemsInternal(htmlHelper, writer, items, options ?? new CompoBoxOptions());
                return new MvcHtmlString(writer.ToString());
            }
        }

        private static void RenderComboBoxItemsInternal(HtmlHelper htmlHelper, TextWriter writer, IEnumerable<SelectListItem> items, CompoBoxOptions options)
        {
            //if (options.SelectListGroups != null && options.SelectListGroups.Count > 0)
            //{
            //    var itemsGroup = items.GroupBy(i => i.Group);
            //}

            if (options.Required == false)
            {
                writer.Write("<li class=\"clear\"><span>Clear</span></li>");
            }

            bool multiSelect = options.MultiSelect;

            foreach (var item in items)
            {
                var itemBuilder = new TagBuilder("li");

                var htmlItem = item as ExtendedSelectListItem;

                if (htmlItem != null)
                {
                    if (htmlItem.HtmlAttributes != null)
                    {
                        itemBuilder.MergeAttributes(htmlItem.HtmlAttributes);
                    }

                    if (htmlItem.DataAttributes != null)
                    {
                        itemBuilder.MergeAttributes(Helpers.GetHtmlAttributes(htmlItem.DataAttributes));
                    }

                    if (htmlItem.HtmlContent != null && item.Text != null)
                    {
                        itemBuilder.MergeAttribute("data-text", item.Text);
                    }

                    if (htmlItem.FilterValue != null)
                    {
                        itemBuilder.MergeAttribute("data-filter-value", htmlItem.FilterValue);
                    }

                    if (htmlItem.AlwaysVisible == true)
                    {
                        itemBuilder.AddCssClass("visible");
                    }
                }

                if (item.Value != null)
                {
                    itemBuilder.MergeAttribute("data-value", item.Value);
                }

                if (item.Selected == true)
                {
                    itemBuilder.AddCssClass("selected");
                }

                itemBuilder.Write(writer, TagRenderMode.StartTag);

                if (multiSelect == true)
                {
                    if (item.Selected == true)
                    {
                        writer.Write("<input class=\"selector\" type=\"checkbox\" checked");
                    }
                    else
                    {
                        writer.Write("<input class=\"selector\" type=\"checkbox\"");
                    }

                    if (item.Disabled == true)
                    {
                        writer.Write(" disabled/>");
                    }
                    else
                    {
                        writer.Write("/>");
                    }
                }

                if (htmlItem != null && htmlItem.HtmlContent != null)
                {
                    htmlItem.HtmlContent.WriteContentHtml(htmlHelper, writer, htmlItem.Model);
                }
                else if (item.Text != null)
                {
                    writer.Write(item.Text);
                }

                itemBuilder.Write(writer, TagRenderMode.EndTag);
            }
        }
    }

    public static class SelectHelper
    {
        public static MvcHtmlString Select(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> items = null, bool multiSelect = false, bool enablesearch = true, int maxItemShow = 0, string selectText = null, bool allowUnknownOption = false, string selectDatasourceUrl = null, object htmlAttributes = null, PopUp popUp = null)
        {
            var stringBuilder = new StringBuilder();
            using (var htmlWriter = new StringWriter(stringBuilder))
            {
                SelectHelper.SelectListInternal(htmlHelper, htmlWriter, name, items, multiSelect, enablesearch, maxItemShow, selectText, allowUnknownOption, selectDatasourceUrl, htmlAttributes, popUp);
            }
            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        private static void SelectListInternal(HtmlHelper htmlHelper, TextWriter htmlWriter, string name, IEnumerable<SelectListItem> items, bool multiSelect, bool enablesearch, int maxItemShow, string selectText, bool allowUnknownOption, string selectDatasourceUrl, object htmlAttributes, PopUp popUp)
        {
            items = items == null ? null : items.GroupBy(i => i.Value).Select(group => group.First());
            bool submitForm = true;
            string identifier = "container-" + name.Trim();
            var selectControl = new TagBuilder("div");

            selectControl.AddCssClass("multivendor-select");
            selectControl.AddCssClass(identifier);
            selectControl.Attributes.Add("data-input-popup-activation-class", ".multivendor-select" + (string.IsNullOrEmpty(identifier) ? string.Empty : "." + identifier.Trim()));
            if (!string.IsNullOrEmpty(selectDatasourceUrl))
                selectControl.Attributes.Add("data-select-data-source-url", selectDatasourceUrl);
            selectControl.Write(htmlWriter, TagRenderMode.StartTag);
            bool errorSelectionData = false;
            if (!multiSelect && items != null && items.Distinct().Where(p => p.Selected).Count() > 1)
            {
                errorSelectionData = true;
            }
            string selectedText = string.Empty;
            //Add a fake select to enable data to form submit
            if (submitForm)
            {
                var selectElement = new TagBuilder("select");

                selectElement.Attributes.Add("name", name);
                selectElement.Attributes.Add("id", name);
                selectElement.Attributes.Add("style", "display:none; overflow:hidden;height:0px;width:0px;");
                if (htmlAttributes != null)
                {
                    selectElement.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                }

                selectElement.Write(htmlWriter, TagRenderMode.StartTag);
                if (string.IsNullOrEmpty(selectDatasourceUrl) && items != null)
                {
                    var selectDefaultOptionElement = new TagBuilder("option");
                    //selectDefaultOptionElement.Attributes.Add("style", "display:none; overflow:hidden;height:0px;width:0px;");
                    selectDefaultOptionElement.Attributes.Add("value", "-1");
                    selectDefaultOptionElement.Write(htmlWriter, TagRenderMode.StartTag);
                    selectDefaultOptionElement.Write(htmlWriter, TagRenderMode.EndTag);
                    foreach (var item in items.Distinct())
                    {
                        if (!errorSelectionData && item.Selected)
                        {
                            selectedText += string.IsNullOrEmpty(selectedText) ? item.Text.Trim() : ", " + item.Text.Trim();
                        }
                        htmlWriter.Write("<option  value=\"" + item.Value + "\" " + (!errorSelectionData && item.Selected ? "selected=\"selected\"" : string.Empty) + " > " + item.Text + "</option>");
                    }
                }
                selectElement.Write(htmlWriter, TagRenderMode.EndTag);
            }
            //End fake select

            htmlWriter.Write("<input  class=\"multivendor-select-input" + (!string.IsNullOrEmpty(selectDatasourceUrl) ? " dynamic" : " static") + "\"" + (enablesearch && string.IsNullOrEmpty(selectDatasourceUrl) ? string.Empty : "readonly") + "type=\"text\" placeholder=\"" + selectText + "\" value=\"" + selectedText + "\" >");
            var selectButtonTag = new TagBuilder("button");
            selectButtonTag.AddCssClass("multivendor-select-icon-container");
            selectButtonTag.MergeAttribute("type", "button");
            selectButtonTag.Write(htmlWriter, TagRenderMode.StartTag);
            htmlWriter.Write("<span class=\"multivendor-select-icon\"></span>");
            selectButtonTag.Write(htmlWriter, TagRenderMode.EndTag);
            
            //Generate pop Up
            var menuItems = new List<MenuItem>();
            if (string.IsNullOrEmpty(selectDatasourceUrl) && items != null)
            {

                foreach (var item in items.Distinct())
                {

                    StringBuilder optionBuilder = new StringBuilder();
                    //insert search checkbox 
                    if (multiSelect)
                        optionBuilder.Append("<div>" + CheckBoxHelper.CheckBoxExt(htmlHelper, isChecked: item.Selected, isEnabled: !item.Disabled) + "</div>");
                    //add displayed text
                    optionBuilder.Append("<span>").Append(item.Text).Append("</span>");
                    menuItems.Add(new MenuItem
                    {
                        Content = MvcHtmlString.Create(optionBuilder.ToString()),
                        HtmlAttributes = new { data_selected = errorSelectionData ? false : item.Selected, data_option_value = item.Value, data_option_text = item.Text },
                    });
                }
            }
            //
            var menuContent = MenuHelper.Menu(htmlHelper, menuItems, settings: new MenuSettings { Hottrack = true, DropdownSelectButton = true }, htmlAttributes: new { @class = "select-Item-list", data_multiselect = multiSelect, data_allowUnknownOption = allowUnknownOption });

            if (popUp == null) popUp = new PopUp();
            if (popUp.AutoClose == null) popUp.AutoClose = false;
            if (popUp.CloseOnClick == null) popUp.CloseOnClick = !multiSelect;
            //if (popUp.HorizontalAlignment != HorizontalAlignment.None) popUp.HorizontalAlignment = HorizontalAlignment.Right;
            //if (popUp.VerticalAlignment != VerticalAlignment.None) popUp.VerticalAlignment = VerticalAlignment.Bottom;

            if (popUp.HorizontalAlignment == HorizontalAlignment.None) popUp.HorizontalAlignment = HorizontalAlignment.Right;
            if (popUp.VerticalAlignment == VerticalAlignment.None) popUp.VerticalAlignment = VerticalAlignment.Bottom;
            
            if (popUp.Top == null && popUp.Bottom == null) popUp.Top = 0;
            if (popUp.Left == null && popUp.Right == null) popUp.Right = 0;

            PopUpHelper.PopUp(htmlHelper, htmlWriter, menuContent,//new HtmlContent( menuItems),
                popUp,
                new PopupActivation
                {
                    ActivationMode = PopupActivationModes.Click,
                    ActivatorMode = PopupSelectorMode.Closest,
                    Activator = ".multivendor-select" + (string.IsNullOrEmpty(identifier) ? string.Empty : "." + identifier.Trim())
                },
                htmlAttributes: new { @class = "select", data_popup_activation_class = ".multivendor-select" + (string.IsNullOrEmpty(identifier) ? string.Empty : "." + identifier.Trim()), data_max_item_count = maxItemShow, data_default_select_Text = selectText, data_selected_text = selectedText }
                );
            //generate popup from the following menuItems
            selectControl.Write(htmlWriter, TagRenderMode.EndTag);
        }
        //Extended Select

        public static MvcHtmlString ExtendedSelect(this HtmlHelper htmlHelper, string name, string selectedValue = null, IEnumerable<ExtendedSelectListItem> items = null, bool multiSelect = false, bool enablesearch = true, int maxItemShow = 0, string selectText = null, bool allowUnknownOption = false, string selectDatasourceUrl = null, object htmlAttributes = null, PopUp popUp = null)
        {
            var stringBuilder = new StringBuilder();
            using (var htmlWriter = new StringWriter(stringBuilder))
            {
                SelectHelper.ExtendedSelectListInternal(htmlHelper, htmlWriter, name, selectedValue, items, multiSelect, enablesearch, maxItemShow, selectText, allowUnknownOption, selectDatasourceUrl, htmlAttributes, popUp);
            }
            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        private static void ExtendedSelectListInternal(HtmlHelper htmlHelper, TextWriter htmlWriter, string name, string selectedValue, IEnumerable<ExtendedSelectListItem> items, bool multiSelect, bool enablesearch, int maxItemShow, string selectText, bool allowUnknownOption, string selectDatasourceUrl, object htmlAttributes, PopUp popUp)
        {
            items = items == null ? null : items.GroupBy(i => i.Value).Select(group => group.First());
            bool submitForm = true;
            string identifier = "container-" + name.Trim();
            var selectControl = new TagBuilder("div");

            selectControl.AddCssClass("multivendor-select");
            selectControl.AddCssClass(identifier);
            selectControl.Attributes.Add("data-input-popup-activation-class", ".multivendor-select" + (string.IsNullOrEmpty(identifier) ? string.Empty : "." + identifier.Trim()));
            if (!string.IsNullOrEmpty(selectDatasourceUrl))
                selectControl.Attributes.Add("data-select-data-source-url", selectDatasourceUrl);

            selectControl.Write(htmlWriter, TagRenderMode.StartTag);
            
            bool errorSelectionData = false;
            if (!multiSelect && items != null && items.Distinct().Where(p => p.Selected).Count() > 1)
            {
                errorSelectionData = true;
            }
            string selectedText = string.Empty;

            //Add a fake select to enable data to form submit
            if (submitForm)
            {
                var selectElement = new TagBuilder("select");

                selectElement.Attributes.Add("name", name);
                selectElement.Attributes.Add("id", name);
                selectElement.Attributes.Add("style", "display:none; overflow:hidden;height:0px;width:0px;");
                if (htmlAttributes != null)
                {
                    selectElement.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                }

                selectElement.Write(htmlWriter, TagRenderMode.StartTag);
                if (string.IsNullOrEmpty(selectDatasourceUrl) && items != null)
                {
                    var selectDefaultOptionElement = new TagBuilder("option");
                    //selectDefaultOptionElement.Attributes.Add("style", "display:none; overflow:hidden;height:0px;width:0px;");
                    selectDefaultOptionElement.Attributes.Add("value", "-1");
                    selectDefaultOptionElement.Write(htmlWriter, TagRenderMode.StartTag);
                    selectDefaultOptionElement.Write(htmlWriter, TagRenderMode.EndTag);
                    foreach (var item in items.Distinct())
                    {
                        bool selected = item.Selected;
                        if (!String.IsNullOrEmpty(selectedValue))
                        {
                            selected = item.Value.Trim() == selectedValue.Trim();
                        }

                        if (!errorSelectionData && selected)
                        {
                            selectedText += string.IsNullOrEmpty(selectedText) ? item.Text.Trim() : ", " + item.Text.Trim();
                        }

                        var selectOptionElement = new TagBuilder("option");
                        if (item.HtmlAttributes != null)
                        {
                            selectOptionElement.MergeAttributes(item.HtmlAttributes);
                            //selectOptionElement.MergeAttributes(item.HtmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(item.HtmlAttributes));
                        }

                        selectOptionElement.Attributes.Add("value", item.Value);
                        if (!errorSelectionData && selected)
                            selectOptionElement.Attributes.Add("selected", "selected");
                        selectOptionElement.Write(htmlWriter, TagRenderMode.StartTag);
                        htmlWriter.Write(item.Text);
                        selectOptionElement.Write(htmlWriter, TagRenderMode.EndTag);
                    }
                }

                selectElement.Write(htmlWriter, TagRenderMode.EndTag);
            }
            //End fake select

            htmlWriter.Write("<input  class=\"multivendor-select-input" + (!string.IsNullOrEmpty(selectDatasourceUrl) ? " dynamic" : " static") + "\"" + (enablesearch && string.IsNullOrEmpty(selectDatasourceUrl) ? string.Empty : "readonly") + "type=\"text\" autocomplete=\"multivendor-autocomplete-control\" placeholder=\"" + selectText + "\" value=\"" + selectedText + "\" >");
            var selectButtonTag = new TagBuilder("button");
            selectButtonTag.AddCssClass("multivendor-select-icon-container");
            selectButtonTag.MergeAttribute("type", "button");
            selectButtonTag.Write(htmlWriter, TagRenderMode.StartTag);
            htmlWriter.Write("<span class=\"multivendor-select-icon\"></span>");
            selectButtonTag.Write(htmlWriter, TagRenderMode.EndTag);
            //Generate pop Up
            var menuItems = new List<MenuItem>();
            if (string.IsNullOrEmpty(selectDatasourceUrl) && items != null)
            {
                foreach (var item in items.Distinct())
                {
                    bool selected = item.Selected;
                    if (!String.IsNullOrEmpty(selectedValue))
                    {
                        selected = item.Value.Trim() == selectedValue.Trim();
                    }
                    StringBuilder optionBuilder = new StringBuilder();
                    //insert search checkbox 
                    if (multiSelect)
                        optionBuilder.Append("<div>" + CheckBoxHelper.CheckBoxExt(htmlHelper, isChecked: selected, isEnabled: !item.Disabled) + "</div>");
                    //add displayed text
                    optionBuilder.Append("<span>").Append(item.Text).Append("</span>");
                    menuItems.Add(new MenuItem
                    {
                        Content = MvcHtmlString.Create(optionBuilder.ToString()),
                        HtmlAttributes = new { data_selected = errorSelectionData ? false : selected, data_option_value = item.Value, data_option_text = item.Text },
                    });
                }
            }
            //
            var menuContent = MenuHelper.Menu(htmlHelper, menuItems, settings: new MenuSettings { Hottrack = true, DropdownSelectButton = true }, htmlAttributes: new { @class = "select-Item-list", data_multiselect = multiSelect, data_allowUnknownOption = allowUnknownOption });

            if (popUp == null) popUp = new PopUp();
            if (popUp.AutoClose == null) popUp.AutoClose = false;
            if (popUp.CloseOnClick == null) popUp.CloseOnClick = !multiSelect;
            //if (popUp.HorizontalAlignment != HorizontalAlignment.None) popUp.HorizontalAlignment = HorizontalAlignment.Right;
            //if (popUp.VerticalAlignment != VerticalAlignment.None) popUp.VerticalAlignment = VerticalAlignment.Bottom;

            if (popUp.HorizontalAlignment == HorizontalAlignment.None) popUp.HorizontalAlignment = HorizontalAlignment.Right;
            if (popUp.VerticalAlignment == VerticalAlignment.None) popUp.VerticalAlignment = VerticalAlignment.Bottom;
            
            if (popUp.Top == null && popUp.Bottom == null) popUp.Top = 0;
            if (popUp.Left == null && popUp.Right == null) popUp.Right = 0;

            PopUpHelper.PopUp(htmlHelper, htmlWriter, menuContent,//new HtmlContent( menuItems),
                popUp,
                new PopupActivation
                {
                    ActivationMode = PopupActivationModes.Click,
                    ActivatorMode = PopupSelectorMode.Closest,
                    Activator = ".multivendor-select" + (string.IsNullOrEmpty(identifier) ? string.Empty : "." + identifier.Trim())
                },
                htmlAttributes: new { @class = "select", data_popup_activation_class = ".multivendor-select" + (string.IsNullOrEmpty(identifier) ? string.Empty : "." + identifier.Trim()), data_max_item_count = maxItemShow, data_default_select_Text = selectText, data_selected_text = selectedText }
                );
            //generate popup from the following menuItems
            selectControl.Write(htmlWriter, TagRenderMode.EndTag);
        }

        public class SelectListItemDefinition
        {
            public string OptionValue { get; set; }

            public string OptionText { get; set; }

            public bool Selected { get; set; }

            public HtmlContent Html { get; set; }

            //public string PartialViewName { get; set; }

            public AttributeBuilder HtmlAttributes { get; set; }
        }

        public static MvcHtmlString RenderSelectListItems<T>(this HtmlHelper htmlHelper, IEnumerable<T> items, Func<T, SelectListItemDefinition> itemDefinitionProvider, bool multiSelect = false)
        {
            var stringBuilder = new StringBuilder();
            using (var htmlWriter = new StringWriter(stringBuilder))
            {
                SelectHelper.RenderSelectListItems(htmlHelper, htmlWriter, items, itemDefinitionProvider, multiSelect);
            }
            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        public static void RenderSelectListItems<T>(this HtmlHelper htmlHelper, TextWriter htmlWriter, IEnumerable<T> items, Func<T, SelectListItemDefinition> itemDefinitionProvider, bool multiSelect = false)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                var definition = itemDefinitionProvider(item);
                var itemTag = new TagBuilder("li");
                var attributes = definition.HtmlAttributes ?? new AttributeBuilder();
                attributes.Attr("data-option-value", definition.OptionValue ?? definition.OptionText ?? item.ToString());
                string optionText = definition.OptionText ?? definition.OptionValue ?? item.ToString();
                attributes.Attr("data-option-text", optionText);
                attributes.Attr("data-selected", definition.Selected);

                itemTag.MergeAttributes(attributes);

                itemTag.Write(htmlWriter, TagRenderMode.StartTag);
                if (multiSelect)
                    htmlWriter.Write("<div>" + CheckBoxHelper.CheckBoxExt(htmlHelper, isChecked: definition.Selected) + "</div>");

                htmlWriter.Write("<div>");
                if (definition.Html != null)
                {
                    definition.Html.WriteContentHtml(htmlHelper, htmlWriter, item);
                }
                else
                {
                    htmlWriter.Write(optionText);
                }
                htmlWriter.Write("</div>");

                itemTag.Write(htmlWriter, TagRenderMode.EndTag);
            }
        }
    }
}