#if NET5
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif
using MultivendorWebViewer.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
#if NET452
using System.Web.Mvc;
using System.Web.WebPages;
#endif
using System.Xml.Serialization;

namespace MultivendorWebViewer.Helpers
{
    public class MenuItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }

        public string Label { get; set; }

        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public bool IncludedHash { get; set; }

        public IconDescriptor Icon { get; set; }


        public object HtmlAttributes { get; set; }
    
        public bool Ignore { get; set; }

        public bool Disabled { get; set; }

        public bool Selected { get; set; }

        public IEnumerable<MenuItem> Items { get; set; }

        public HtmlContent Content { get; set; }

        public object Model { get; set; }

        public CheckType CheckType { get; set; }

        public bool Checked { get; set; }

        public string CheckGroup { get; set; }

        public MenuItem Copy()
        {
            return (MenuItem)MemberwiseClone();
        }
    }

    //public class MenuSettings
    //{
    //    static MenuSettings()
    //    {
    //        Default = new MenuSettings();
    //    }

    //    public MenuSettings()
    //    {
    //        Hottrack = true;

    //        SelectionMode = ListSelectMode.None;
    //    }

    //    public bool CloseButton { get; set; }

    //    public bool DropdownSelectButton { get; set; }
    //    public bool Hottrack { get; set; }

    //    public ListSelectMode SelectionMode { get; set; }

    //    public bool? DisplaySelect { get; set; }

    //    //public bool Check { get; set; }

    //    //public bool CheckOnClick { get; set; }

    //    //public IconDescriptor CheckIcon { get; set; }

    //    public static MenuSettings Default { get; private set; }
    //}

    public static class MenuHelper
    {
        public static MvcHtmlString Menu(this HtmlHelper htmlHelper, IEnumerable<MenuItem> items, object htmlAttributes = null, MenuSettings settings = null)
        {
            using (var htmlWriter = new StringWriter(CultureInfo.CurrentUICulture))
            {
                Menu(htmlHelper, htmlWriter, items, htmlAttributes, settings);

                return MvcHtmlString.Create(htmlWriter.ToString());
            }
        }

        public static void Menu(this HtmlHelper htmlHelper, TextWriter htmlWriter, IEnumerable<MenuItem> items, object htmlAttributes = null, MenuSettings settings = null)
        {
            if (settings == null)
            {
                settings = MenuSettings.Default;
            }
            if(settings.CloseButton)
            //Adding the close button on the menu
            AddCloseButton(htmlHelper, htmlWriter, items, htmlAttributes, settings);
            
            if (settings.DropdownSelectButton)
                //Adding the close button on the menu
                AddDropdownSelectButton(htmlHelper, htmlWriter, items, htmlAttributes, settings);
            var menuTag = new TagBuilder("ul");

            menuTag.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            if (settings.Hottrack == true)
            {
                menuTag.AddCssClass("hottrack");
            }

            menuTag.MergeAttribute("data-selection-mode", settings.SelectionMode == ListSelectMode.Single ? "single" : settings.SelectionMode == ListSelectMode.Multi ? "multiple" : "none");
            
            if (settings.DisplaySelect == true || settings.DisplaySelect.HasValue == false && settings.SelectionMode != ListSelectMode.None)
            {
                menuTag.AddCssClass("display-check");
            }

            //if (settings.Check == true)
            //{
            //    menuTag.AddCssClass("check");

            //    if (settings.CheckOnClick == true)
            //    {
            //        menuTag.AddCssClass("check-click");
            //    }
            //}

            menuTag.AddCssClass("multivendor-menu");

            menuTag.Write(htmlWriter, TagRenderMode.StartTag);

            RenderItems(htmlHelper, items, htmlWriter, settings);

            menuTag.Write(htmlWriter, TagRenderMode.EndTag);
        }

        public static void AddCloseButton(this HtmlHelper htmlHelper, TextWriter htmlWriter, IEnumerable<MenuItem> items, object htmlAttributes = null, MenuSettings settings = null)
        {

            //if (settings.CloseButton == false)
              //  return;

            var menuCloseContainerTag = new TagBuilder("div");

            menuCloseContainerTag.AddCssClass("popup-menu-close-container");

            menuCloseContainerTag.Write(htmlWriter, TagRenderMode.StartTag);

            var menuCloseButtonTag = new TagBuilder("button");

            menuCloseButtonTag.AddCssClass("popup-menu-close-button");

            menuCloseButtonTag.MergeAttribute("type", "button");

            menuCloseButtonTag.Write(htmlWriter, TagRenderMode.StartTag);

            menuCloseButtonTag.AddCssClass("popup-menu-close-container");

            var menuCloseIconTag= new TagBuilder("span");

            //icon class
            menuCloseIconTag.MergeAttribute("class", "icon fa fa-times");

            menuCloseIconTag.Write(htmlWriter, TagRenderMode.SelfClosing);

            menuCloseButtonTag.Write(htmlWriter, TagRenderMode.EndTag);

            menuCloseContainerTag.Write(htmlWriter, TagRenderMode.EndTag);

        }
        public static void AddDropdownSelectButton(this HtmlHelper htmlHelper, TextWriter htmlWriter, IEnumerable<MenuItem> items, object htmlAttributes = null, MenuSettings settings = null)
        {

            //if (settings.CloseButton == false)
            //  return;

            var menuCloseContainerTag = new TagBuilder("div");

            menuCloseContainerTag.AddCssClass("dropdown-close-container");

            menuCloseContainerTag.Write(htmlWriter, TagRenderMode.StartTag);

            var menuCloseButtonTag = new TagBuilder("button");

            menuCloseButtonTag.AddCssClass("dropdown-close-button");

            menuCloseButtonTag.MergeAttribute("type", "button");

            menuCloseButtonTag.Write(htmlWriter, TagRenderMode.StartTag);
            var menuCloseIconTag = new TagBuilder("span");
            //icon class
            menuCloseIconTag.MergeAttribute("class", "icon fa fa-times");

            menuCloseIconTag.Write(htmlWriter, TagRenderMode.SelfClosing);

            menuCloseButtonTag.Write(htmlWriter, TagRenderMode.EndTag);

            menuCloseContainerTag.Write(htmlWriter, TagRenderMode.EndTag);

        }
        private static void RenderItems(HtmlHelper htmlHelper, IEnumerable<MenuItem> items, TextWriter htmlWriter, MenuSettings settings)
        {
            foreach (var item in items.Where(i => i.Ignore == false))
            {
                var itemTag = new TagBuilder("li");

                if (item.HtmlAttributes != null)
                {
                    itemTag.MergeAttributes(Helpers.GetHtmlAttributes(item.HtmlAttributes));
                }

                if (item.Disabled == true)
                {
                    itemTag.AddCssClass("disabled");
                }

                if (item.Selected == true)
                {
                    itemTag.AddCssClass("selected");
                }

                if (item.Checked == true)
                {
                    itemTag.AddCssClass("checked");
                }

                if (item.Id != null)
                {
                    itemTag.GenerateId(item.Id);
                }

                if (item.Name != null)
                {
                    itemTag.MergeAttribute("data-name", item.Name);
                }

                if (item.Value != null)
                {
                    itemTag.MergeAttribute("data-value", item.Value.ToString());
                }

                //if (item.Checked == true)
                //{
                //    itemTag.AddCssClass("checked");
                //}

                if (item.CheckType != CheckType.None)
                {
                    itemTag.AddCssClass("checkable");
                    if (item.CheckType != CheckType.Normal)
                    {
                        var values = new Dictionary<string, int>();
                        if (item.CheckType.HasFlag(CheckType.Radio)) values.Add("Radio", 1);
                        if (item.CheckType.HasFlag(CheckType.Toggle)) values.Add("Toggle", 1);
                        itemTag.MergeAttribute("data-check-type", Newtonsoft.Json.JsonConvert.SerializeObject(values));
                    }

                    if (item.CheckGroup != null)
                    {
                        itemTag.MergeAttribute("data-check-group", item.CheckGroup);
                    }
                }

                itemTag.Write(htmlWriter, TagRenderMode.StartTag);

                //if (settings.Check == true && settings.CheckIcon != null)
                //{
                //    settings.CheckIcon.WriteHtml(htmlWriter, classNames: "check-icon");
                //}



                TagBuilder linkTag = null;
                
                if (item.Url != null)
                {
                    linkTag = new TagBuilder("a");
                    linkTag.MergeAttribute("href", item.Url);
                    linkTag.MergeAttribute("target", item.UrlTarget);
                    if (item.IncludedHash == true)
                    {
                        linkTag.AddCssClass("include-hash");
                    }
                    linkTag.Write(htmlWriter, TagRenderMode.StartTag);
                }

                //if (item.Icon != null)
                //{
                //    IconHelper.IconInternal(item.Icon, htmlWriter);
                //}

                if (item.Label != null)
                {
                    htmlWriter.Write(item.Label);
                }

                if (item.Content != null)
                {
                    item.Content.WriteContentHtml(htmlHelper, htmlWriter, item.Model ?? item);
                }

                if (item.Items != null && item.Items.Any(i => i.Ignore == false) == true)
                {
                    Action<HtmlHelper, TextWriter> dropDownHtml = (dropDownHelper, dropDownWriter) =>
                    {
                        var itemsTag = new TagBuilder("ul");

                        if (settings.Hottrack == true)
                        {
                            itemsTag.AddCssClass("hottrack");
                        }

                        itemsTag.AddCssClass("multivendor-menu");

                        itemsTag.Write(dropDownWriter, TagRenderMode.StartTag);

                        RenderItems(dropDownHelper, item.Items, dropDownWriter, settings);

                        itemsTag.Write(dropDownWriter, TagRenderMode.EndTag);
                    };

                    DropDownHelper.DropDown(htmlHelper, htmlWriter, new HtmlContent(dropDownHtml),
                        new DropDown
                        {
                            AutoClose = false,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Left = 0,
                            VerticalAlignment = VerticalAlignment.Top,
                            Top = -12,
                            Direction = DropDownDirection.Down,
                        },
                        new PopupActivation
                        {
                            ActivationMode = PopupActivationModes.MouseEnter,
                            ActivatorMode = PopupSelectorMode.Closest,
                            Activator = "li"
                        });
                }
                
                if (linkTag != null)
                {
                    linkTag.Write(htmlWriter, TagRenderMode.EndTag);
                }

                itemTag.Write(htmlWriter, TagRenderMode.EndTag);
            }
        }
    }
}