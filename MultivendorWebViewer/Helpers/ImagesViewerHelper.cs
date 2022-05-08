using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultivendorWebViewer.Common;
#if NET452
using System.Web.Mvc;
using System.Web.WebPages;
using System.Web.Routing;
#endif
using System.IO;
using System.Globalization;
using MultivendorWebViewer.Models;
using System.Collections.Concurrent;
using System.Web;
#if NET5
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
#endif

namespace MultivendorWebViewer.Helpers
{
    //    public enum ImageViewLoadMode { Direct, Delayed, Programmatically }

    //    public enum ImageViewZoomMode { None, FitWidth, FitHeight, FitFull }

    //    public class ImageFileExtensionAttribute : Attribute
    //    {
    //        public ImageFileExtensionAttribute(params string[] extensions)
    //        {
    //            Extensions = extensions;
    //        }

    //        public string[] Extensions { get; private set; }

    //        public int Order { get; set; }
    //    }

    //    //[ImageFileExtension("*", Order = 100)]
    //    public class ImageViewDefinition
    //    {
    //        public ImageViewDefinition()
    //        {
    //            ToolbarDefinition = ImageViewToolbarDefinition.Default;
    //            LoadMode = ImageViewLoadMode.Direct;
    //            CaptionTagName = "h3";
    //            ZoomMode = ImageViewZoomMode.FitWidth;
    //        }

    //        public object Model { get; set; }

    //        public string Id { get; set; }

    //        public string FileName { get; set; }

    //        public string Url { get; set; }

    //        public double? Width { get; set; }

    //        public double? Height { get; set; }

    //        public string LoadUrl { get; set; }

    //        public string Caption { get; set; }

    //        public string CaptionTagName { get; set; }

    //        public string ThumbUrl { get; set; }

    //        public string ThumbCaption { get; set; }

    //        public bool Ignore { get; set; }

    //        public ImageViewLoadMode LoadMode { get; set; }

    //        public ImageViewZoomMode ZoomMode { get; set; }

    //        public object HtmlAttributes { get; set; }

    //        public Func<ImageViewDefinition, HelperResult> HeaderTemplate { get; set; }

    //        public HtmlContent ImageTemplate { get; set; }

    //        public HtmlContent Template { get; set; }

    //        public IEnumerable<HtmlContent> LayerTemplates { get; set; }

    //        public virtual bool? ToolbarAvailable { get; set; }

    //        public ImageViewToolbarDefinition ToolbarDefinition { get; set; }

    //        public virtual void RenderHtml(HtmlHelper htmlHelper, StringBuilder htmlBuilder, ImagesViewerSettings settings = null, object htmlAttributes = null)
    //        {
    //            if (Ignore == true)
    //            {
    //                return;
    //            }

    //            if (Template != null)
    //            {
    //                Template.WriteContentHtml(htmlHelper, htmlBuilder);
    //                return;
    //            }

    //            var layoutDefinitions = new List<LayoutDefinition>(2);
    //            bool hasCaption = string.IsNullOrEmpty(Caption) == false;
    //            string zoomMode = ZoomMode == ImageViewZoomMode.FitWidth ? "fit-width" : ZoomMode == ImageViewZoomMode.FitHeight ? "fit-height" : ZoomMode == ImageViewZoomMode.FitFull ? "fit-full" : null;

    //            if (LoadMode == ImageViewLoadMode.Direct)
    //            {
    //                var toolbarDefinition = settings != null ? settings.ToolbarDefinition : ToolbarDefinition;
    //                bool hasToolbar = toolbarDefinition != null;
    //                bool toolbarAvailabe = hasToolbar == true && ((settings != null ? settings.ToolbarAvailable : ToolbarAvailable) ?? true);

    //                if (toolbarAvailabe == true || hasCaption == true || HeaderTemplate != null)
    //                {
    //                    string captionTagName = settings != null ? settings.CaptionTagName : CaptionTagName;

    //                    layoutDefinitions.Add(new LayoutDefinition
    //                    {
    //                        OmitEmpty = false,
    //                        ClassNames = "header",
    //                        Content = new HtmlContent((HtmlHelper headerHelper, StringBuilder headerHtml) =>
    //                        {
    //                            if (HeaderTemplate == null)
    //                            {
    //                                if (hasCaption == true)
    //                                {
    //                                    headerHtml.Append('<').Append(captionTagName).Append(" class=\"caption\">").Append(Caption);
    //                                }

    //                                if (toolbarAvailabe == true)
    //                                {
    //                                    var selectedZoomItem = toolbarDefinition.ToolbarItems.FirstOrDefault(i => Helpers.GetHtmlAttributeValue(i.HtmlAttributes, "data-zoom-mode") as string == zoomMode);
    //                                    if (selectedZoomItem != null)
    //                                    {
    //                                        selectedZoomItem.Checked = true;
    //                                    }

    //                                    ToolBarHelper.ToolBar(headerHelper, headerHtml, toolbarDefinition.ToolbarItems, toolbarDefinition.ToolBarSettings, new { @class = "view-main-tools" });
    //                                }

    //                                if (hasCaption == true)
    //                                {
    //                                    headerHtml.Append("</").Append(captionTagName).Append('>');
    //                                }
    //                            }
    //                            else
    //                            {
    //                                headerHtml.Append(HeaderTemplate(this).ToString());
    //                            }
    //                        })
    //                    });
    //                }

    //                layoutDefinitions.Add(new LayoutDefinition
    //                {
    //                    OmitEmpty = false,
    //                    ClassNames = "view",
    //                    FillWeight = 1,
    //                    Content = new HtmlContent((HtmlHelper viewHelper, StringBuilder viewHtml) =>
    //                    {
    //                        viewHtml.Append("<div class=\"image-container\">");

    //                        if (LayerTemplates != null && LayerTemplates.Any() == true)
    //                        {
    //                            viewHtml.Append("<div class=\"layer-container\">");
    //                            foreach (var layer in LayerTemplates)
    //                            {
    //                                layer.WriteContentHtml(viewHelper, viewHtml, this);
    //                            }
    //                            viewHtml.Append("</div>");
    //                        }

    //                        if (ImageTemplate == null)
    //                        {
    //                            if (Url != null)
    //                            {
    //                                viewHtml.Append("<img class=\"image\" src=\"" + Url + "\"/>");
    //                            }
    //                        }
    //                        else
    //                        {
    //                            ImageTemplate.WriteContentHtml(viewHelper, viewHtml, this);
    //                            //viewHtml.Append(ImageTemplate(this).ToString());
    //                        }

    //                        viewHtml.Append("</div>");
    //                    })
    //                });
    //            }


    //            var viewAttributes = Helpers.MergeHtmlAttributes(HtmlAttributes, htmlAttributes);

    //            //var viewAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(HtmlAttributes);

    //            var classNames = new List<string> { "multivendor-web-imageview" };

    //            if (LoadMode == ImageViewLoadMode.Direct)
    //            {
    //                classNames.Add("loaded");
    //            }
    //            else
    //            {
    //                viewAttributes.Add("data-load-mode", LoadMode.ToString().ToLowerInvariant());
    //            }

    //            Helpers.MergeClassAttribute(viewAttributes, classNames);

    //            if (zoomMode != null)
    //            {
    //                viewAttributes.Add("data-zoom-mode", zoomMode);
    //            }

    //            if (Width.HasValue == true && Height.HasValue == true)
    //            {
    //                viewAttributes.Add("data-image-width", Width);
    //                viewAttributes.Add("data-image-height", Height);
    //            }

    //            if (Id != null)
    //            {
    //                viewAttributes.Add("data-id", Id);
    //            }

    //            if (LoadUrl != null)
    //            {
    //                viewAttributes.Add("data-load-url", LoadUrl);
    //            }

    //            LayoutHelper.Layout(htmlHelper, htmlBuilder, layoutDefinitions, new LayoutSettings { Direction = LayoutDirection.Vertical }, viewAttributes);
    //        }
    //    }

    //    public class InlineImageViewDefinition : ImageViewDefinition
    //    {

    //    }

    //    public class PartialImageViewDefinition : PartialImageViewDefinitionBase
    //    {
    //        public string Name { get; set; }

    //        public override string ViewName
    //        {
    //            get { return Name; }
    //        }
    //    }

    //    public class FileExtensionImageViewDefinition : PartialImageViewDefinitionBase
    //    {
    //        public string Extension { get; set; }

    //        public override string ViewName
    //        {
    //            get { return string.Concat("_", Extension.TrimStart('.').ToLowerInvariant(), "IllustrationView"); }
    //        }

    //        public static new bool HasImageView(ControllerContext context, string extension)
    //        {
    //            return PartialImageViewDefinitionBase.GetImageView(context, string.Concat("_", extension.TrimStart('.').ToLowerInvariant(), "IllustrationView")) != null;
    //        }
    //    }

    //    public abstract class PartialImageViewDefinitionBase : ImageViewDefinition
    //    {
    //        public PartialImageViewDefinitionBase()
    //        {
    //            ToolbarAvailable = false;
    //        }

    //        public override void RenderHtml(HtmlHelper htmlHelper, StringBuilder htmlBuilder, ImagesViewerSettings settings = null, object htmlAttributes = null)
    //        {
    //            if (Ignore == true)
    //            {
    //                return;
    //            }

    //#if NET5
    //            using (var stringWriter = new StringWriter(htmlBuilder))
    //            {
    //                var viewContext = ViewHelpers.CreateViewContext(htmlHelper, model: Model, writer: stringWriter);
    //                var view = PartialImageViewDefinitionBase.GetImageView(viewContext, ViewName);
    //                if (view != null)
    //                {
    //                    view.Render(viewContext);
    //                }
    //            }
    //#else
    //            var viewContext = Model != null ? ViewHelpers.CreateViewContext(htmlHelper, model: Model) : htmlHelper.ViewContext;

    //            var view = PartialImageViewDefinitionBase.GetImageView(viewContext, ViewName);
    //            try
    //            {
    //                if (view != null)
    //                {
    //                    using (var stringWriter = new StringWriter(htmlBuilder))
    //                    {
    //                        view.Render(viewContext, stringWriter);

    //                    }
    //                }
    //            }
    //            catch (Exception) { }
    //#endif

    //        }

    //        public abstract string ViewName { get; }

    //        private static WriteSynchronicedConcurrentDictionary<string, IView> cachedViews = new WriteSynchronicedConcurrentDictionary<string, IView>();
    //#if NET5
    //        protected static IView GetImageView(ActionContext context, string viewName)
    //        {
    //            string controllerName = context.ActionDescriptor.RouteValues["controller"];
    //#else
    //        protected static IView GetImageView(ControllerContext context, string viewName)
    //        {
    //            string controllerName = context.RouteData.GetRequiredString("controller");
    //#endif

    //            string cacheKey = string.Concat(controllerName, "/", viewName);

    //            return cachedViews.GetOrAddOnce(cacheKey, k1 =>
    //            {
    //#if NET5
    //                var view = ViewHelpers.FindPartialView(context, viewName).View;
    //#else
    //                var view = ViewEngines.Engines.FindPartialView(context, viewName).View;
    //#endif

    //                if (view == null && controllerName != "Illustration")
    //                {
    //                    string illCacheKey = string.Concat("~/Views/Illustration", "/", viewName, ".cshtml");
    //#if NET5
    //                    return cachedViews.GetOrAdd(illCacheKey, k2 => ViewHelpers.FindPartialView(context, k2).View);
    //#else
    //                    return cachedViews.GetOrAdd(illCacheKey, k2 => ViewEngines.Engines.FindPartialView(context, k2).View);
    //#endif
    //                }
    //                return view;
    //            });
    //        }

    //        public static bool HasImageView(ControllerContext context, string viewName)
    //        {
    //            return PartialImageViewDefinitionBase.GetImageView(context, viewName) != null;
    //        }
    //    }

    //    //public class InlineSvgImageViewDefinition : ImageViewDefinition
    //    //{
    //    //    public override void RenderHtml(HtmlHelper htmlHelper, StringBuilder htmlBuilder, ImagesViewerSettings settings = null, object htmlAttributes = null)
    //    //    {
    //    //        base.RenderHtml(htmlHelper, htmlBuilder, settings, htmlAttributes);
    //    //    }
    //    //}

    //    public class ImageViewDefinitionProvider : SingletonBase<ImageViewDefinitionProvider>
    //    {
    //        public virtual ImageViewDefinition Create(ControllerContext context, IImageViewModel image)
    //        {
    //            if (image.FileName.IsNullOrEmpty()) throw new ArgumentNullException("image.FileName");

    //            string extension = Path.GetExtension(image.FileName).ToLowerInvariant();
    //            if (image.Height != 0 && image.Width != 0)
    //            {
    //                if (FileExtensionImageViewDefinition.HasImageView(context, extension) == true)
    //                {
    //                    return new FileExtensionImageViewDefinition { Extension = extension };
    //                }
    //            }
    //            switch (extension)
    //            {
    //                case ".pvz":
    //                    return new PartialImageViewDefinition { Name = "_creoIllustrationView" };
    //                case ".xv2":
    //                    return new PartialImageViewDefinition { Name = "_xvlPlayerIllustrationView" };
    //                default:
    //                    return Instance.Create<ImageViewDefinition>();
    //            }
    //            //if (image.FileName == "A54354-3200-C5A.svg")
    //            //{
    //            //    var appRequestCtx = context.Controller.GetApplicationRequestContext();

    //            //    var imageViewer = new ImageViewDefinition();
    //            //    imageViewer.ImageTemplate = new HtmlContent((HtmlHelper htmlHelper, StringBuilder htmlBuilder) =>
    //            //    {
    //            //        var svgString = System.Text.Encoding.UTF8.GetString(appRequestCtx.SiteContext.ImageManager.GetImageContent(image.Id));
    //            //        htmlBuilder.Append("<div class=\"hotspot-layer embedded-hotspots image\">");
    //            //        htmlBuilder.Append(svgString);
    //            //        //@Html.Raw(svgString)
    //            //        htmlBuilder.Append("</div>");
    //            //    });
    //            //    return imageViewer;
    //            //}


    //        }
    //    }

    //    public class ImageViewToolbarDefinition
    //    {
    //        public ImageViewToolbarDefinition()
    //        {
    //            ZoomInLabel = DefaultZoomInLabel;
    //            ZoomInIcon = DefaultZoomInIcon;
    //            ZoomOutLabel = DefaultZoomOutLabel;
    //            ZoomOutIcon = DefaultZoomOutIcon;

    //            ToolbarItems = DefaultToolbarItems;
    //            ToolBarSettings = DefaultToolbarSettings;
    //        }

    //        public List<ToolBarItem> ToolbarItems { get; set; }

    //        public ToolBarSettings ToolBarSettings { get; set; }

    //        public string ZoomInLabel { get; set; }

    //        public IconDescriptor ZoomInIcon { get; set; }

    //        public string ZoomOutLabel { get; set; }

    //        public IconDescriptor ZoomOutIcon { get; set; }

    //        protected virtual string DefaultZoomInLabel { get { return "Zoom in"; } }

    //        protected virtual IconDescriptor DefaultZoomInIcon
    //        {
    //            get
    //            {
    //                var requestContext = ApplicationRequestContext.Current;
    //                if (requestContext == null) throw new ArgumentNullException("ApplicationRequestContext");

    //                return requestContext.SiteContext.GetIcon(Icons.ZoomIn);
    //            }
    //        }

    //        protected virtual string DefaultZoomOutLabel { get { return "Zoom out"; } }

    //        protected virtual IconDescriptor DefaultZoomOutIcon
    //        {
    //            get
    //            {
    //                var requestContext = ApplicationRequestContext.Current;
    //                if (requestContext == null) throw new ArgumentNullException("ApplicationRequestContext");

    //                return requestContext.SiteContext.GetIcon(Icons.ZoomOut);
    //            }
    //        }

    //        protected virtual List<ToolBarItem> DefaultToolbarItems
    //        {
    //            get
    //            {
    //                var requestContext = ApplicationRequestContext.Current;
    //                if (requestContext == null) throw new ArgumentNullException("ApplicationRequestContext");

    //                return new List<ToolBarItem> 
    //                {
    //                    new ToolBarItem { Name = "zoomIn", Icon = ZoomInIcon, Label = ZoomInLabel },
    //                    new ToolBarItem { Name = "zoomOut", Icon = ZoomOutIcon, Label = ZoomOutLabel },
    //                    new ToolBarItem { Name = "zoomFullWidth", Icon = requestContext.SiteContext.GetIcon(Icons.ZoomFullWidth), Label = "Adjust to width", CheckType = CheckType.Radio, HtmlAttributes = new { data_zoom_mode = "fit-width" } },
    //                    new ToolBarItem { Name = "zoomFull", Icon = requestContext.SiteContext.GetIcon(Icons.ZoomFull), Label = "Adjust to full", CheckType = CheckType.Radio, HtmlAttributes = new { data_zoom_mode = "fit-full" } },
    //                    /*new ToolBarItem { Name = "redLine", Icon = Icons.Pen, Label = "Redline", DropDownItems = new [] {
    //                        new MenuItem { Icon = Icons.Paint, Label = "Paint" },
    //                        new MenuItem { Icon = Icons.Write, Label = "Text" },
    //                        new MenuItem { Icon = Icons.Save, Label = "Save" },
    //                        new MenuItem { Icon = Icons.Remove, Label = "Clear" }
    //                    }, DropDown = new DropDown { Direction  = DropDownDirection.Down }}*/
    //                };
    //            }
    //        }

    //        protected virtual ToolBarSettings DefaultToolbarSettings
    //        {
    //            get { return new ToolBarSettings { ContentDirection = LayoutDirection.Horizontal, DisplayLabels = false, Highlight = true, Size = ToolBarSize.Medium, Direction = LayoutDirection.Horizontal }; }
    //        }

    //        static ImageViewToolbarDefinition()
    //        {
    //            ImageViewToolbarDefinition.Default = new ImageViewToolbarDefinition();
    //        }

    //        public static ImageViewToolbarDefinition Default { get; set; }
    //    }

    //    public class ImagesViewerSettings
    //    {
    //        public ImagesViewerSettings()
    //        {
    //            TagName = "div";
    //            CaptionTagName = "h3";
    //            ThumbCaptionTagName = "h6";

    //            ThumbnailListSettings = new ListSettings<ImageViewDefinition>
    //            {
    //                SelectionMode = ListSelectMode.Single,
    //                Hottrack = true,
    //                Direction = LayoutDirection.Horizontal
    //            };
    //        }

    //        //public ImageCapabilities Capabilities { get; set; }

    //        //public string LoadUrl { get; set; }

    //        //public string ImageViewUrl { get; set; }

    //        //public string ThumbsUrl { get; set; }

    //        //public bool PreloadImages { get; set; }

    //        public bool AllowNoSelectedImage { get; set; }

    //        public ImageViewDefinition SelectedImage { get; set; }

    //        public string TagName { get; set; }

    //        public string CaptionTagName { get; set; }

    //        public string ThumbCaptionTagName { get; set; }

    //        public Func<ImageViewDefinition, HelperResult> ThumbnailTemplate { get; set; }

    //        public ListSettings<ImageViewDefinition> ThumbnailListSettings { get; set; }

    //        public bool? ToolbarAvailable { get; set; }

    //        public ImageViewToolbarDefinition ToolbarDefinition { get; set; }

    //        static ImagesViewerSettings()
    //        {
    //            ImagesViewerSettings.Default = new ImagesViewerSettings();
    //        }

    //        public static ImagesViewerSettings Default { get; set; }
    //    }

    //    public static class ImagesViewerHelper
    //    {
    //        public static MvcHtmlString ImagesViewer(this HtmlHelper htmlHelper, IEnumerable<ImageViewDefinition> images, ImagesViewerSettings settings = null, object htmlAttributes = null)
    //        {
    //            var htmlBuilder = new StringBuilder();
    //            ImagesViewerHelper.ImagesViewer(htmlHelper, htmlBuilder, images, settings, htmlAttributes);
    //            return MvcHtmlString.Create(htmlBuilder.ToString());
    //        }

    //        public static void ImagesViewer(this HtmlHelper htmlHelper, StringBuilder htmlBuilder, IEnumerable<ImageViewDefinition> images, ImagesViewerSettings settings = null, object htmlAttributes = null)
    //        {
    //            if (settings == null)
    //            {
    //                settings = ImagesViewerSettings.Default;
    //            }

    //            var imageList = images.ToArray();
    //            int selectedImageIndex = Array.IndexOf(imageList, settings.SelectedImage);
    //            if (settings.AllowNoSelectedImage == false && selectedImageIndex == -1)
    //            {
    //                selectedImageIndex = 0;
    //            }

    //            var layoutDefinitions = new List<LayoutDefinition>(2);

    //            bool multipleImages = imageList.Length > 1;
    //            if (multipleImages == true)
    //            {
    //                layoutDefinitions.Add(new LayoutDefinition
    //                {
    //                    ClassNames = "thumbnails-container",
    //                    Content = new HtmlContent((HtmlHelper thumbListHelper, StringBuilder thumbListHtml) =>
    //                    {
    //                        var listSettings = settings.ThumbnailListSettings ?? new ListSettings<ImageViewDefinition>();
    //                        if (listSettings.ItemFormatter == null)
    //                        {
    //                            listSettings.ItemFormatter = (ImageViewDefinition thumbImage, int index, TagBuilder thumbTag) =>
    //                            {
    //                                if (thumbImage.Id != null)
    //                                {
    //                                    thumbTag.MergeAttribute("data-id", thumbImage.Id);
    //                                }

    //                                if (selectedImageIndex == index)
    //                                {
    //                                    thumbTag.AddCssClass("selected");
    //                                }

    //                                //if (thumbImage.Url != null)
    //                                //{
    //                                //    thumbTag.MergeAttribute("data-url", thumbImage.Url);
    //                                //}
    //                            };
    //                        }

    //                        using (var thumbListHtmlWriter = new StringWriter(thumbListHtml, CultureInfo.InvariantCulture))
    //                        {
    //                            ListHelper.List(thumbListHelper, thumbListHtmlWriter, imageList, (thumbHelper, thumbHtmlWriter, thumbImage) =>
    //                            {
    //                                if (settings.ThumbnailTemplate == null)
    //                                {
    //                                    string src = thumbImage.ThumbUrl ?? thumbImage.Url;

    //                                    thumbHtmlWriter.Write("<img src=\"");
    //                                    thumbHtmlWriter.Write(src);
    //                                    thumbHtmlWriter.Write("\"/>");
    //                                }
    //                                else
    //                                {
    //                                    settings.ThumbnailTemplate(thumbImage).WriteTo(thumbHtmlWriter);
    //                                }


    //                                if (thumbImage.ThumbCaption != null)
    //                                {
    //                                    thumbHtmlWriter.Write('<');
    //                                    thumbHtmlWriter.Write(settings.ThumbCaptionTagName);
    //                                    thumbHtmlWriter.Write(" class=\"caption\">");
    //                                    thumbHtmlWriter.Write(thumbImage.ThumbCaption);
    //                                    thumbHtmlWriter.Write("</");
    //                                    thumbHtmlWriter.Write(settings.ThumbCaptionTagName);
    //                                    thumbHtmlWriter.Write('>');
    //                                }
    //                            }, listSettings, new { @class = "thumbnails-list" });
    //                        }
    //                    })
    //                });
    //            }

    //            var imageContainerHtmlAttributes = new RouteValueDictionary();
    //            //if (settings.ImageLoadUrl != null)
    //            //{
    //            //    imageContainerHtmlAttributes.Add("data-image-load-url", settings.ImageLoadUrl);
    //            //}

    //            layoutDefinitions.Add(new LayoutDefinition
    //            {
    //                OmitEmpty = false,
    //                FillWeight = 1,
    //                ClassNames = "views-container",
    //                HtmlAttributes = imageContainerHtmlAttributes,
    //                Content = new HtmlContent((HtmlHelper imagesHelper, StringBuilder imagesHtml) =>
    //                {
    //                    foreach (var image in imageList)
    //                    {
    //                        var imageTag = new TagBuilder("div");

    //                        //if (image.Id != null)
    //                        //{
    //                        //    imageTag.MergeAttribute("data-id", image.Id);
    //                        //}

    //                        //if (image.LoadUrl != null)
    //                        //{
    //                        //    imageTag.MergeAttribute("data-image-view-url", image.LoadUrl);
    //                        //}

    //                        imageTag.AddCssClass("view-container");

    //                        if (multipleImages == false || imageList[selectedImageIndex] == image)
    //                        {
    //                            imageTag.AddCssClass("selected");
    //                        }

    //                        imageTag.Write(imagesHtml, TagRenderMode.StartTag);

    //                        image.RenderHtml(imagesHelper, imagesHtml, settings);

    //                        //if (preload == true)
    //                        //{
    //                        //    imageTag.Write(imagesHtml, TagRenderMode.StartTag);
    //                        //    image.RenderHtml(imagesHelper, imagesHtml, settings);
    //                        //}
    //                        //else
    //                        //{
    //                        //    //if (image.Url != null)
    //                        //    //{
    //                        //    //    imageTag.MergeAttribute("data-url", image.Url);
    //                        //    //}

    //                        //    imageTag.AddCssClass("unloaded");

    //                        //    imageTag.Write(imagesHtml, TagRenderMode.StartTag);
    //                        //}

    //                        imageTag.Write(imagesHtml, TagRenderMode.EndTag);
    //                    }
    //                })
    //            });

    //            var layoutHtmlAttributes = htmlAttributes is IDictionary<string, object> ? new RouteValueDictionary((IDictionary<string, object>)htmlAttributes) : new RouteValueDictionary(htmlAttributes);

    //            //if (settings.ImageViewUrl != null)
    //            //{
    //            //    layoutHtmlAttributes.Add("data-image-view-url", settings.ImageViewUrl);
    //            //}

    //            Helpers.MergeClassAttribute(layoutHtmlAttributes, "multivendor-web-imagesview");

    //            LayoutHelper.Layout(htmlHelper, htmlBuilder, layoutDefinitions, new LayoutSettings { Direction = LayoutDirection.Vertical }, layoutHtmlAttributes);
    //        }

    //        public static MvcHtmlString ImageViewer(this HtmlHelper htmlHelper, ImageViewDefinition image, ImagesViewerSettings settings = null, object htmlAttributes = null)
    //        {
    //            var htmlBuilder = new StringBuilder();
    //            image.RenderHtml(htmlHelper, htmlBuilder, settings, htmlAttributes);
    //            return MvcHtmlString.Create(htmlBuilder.ToString());
    //        }
    //    }
}
