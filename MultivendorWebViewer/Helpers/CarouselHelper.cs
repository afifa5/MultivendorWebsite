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
#endif
using System.Xml.Serialization;
#if NET5
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif
using MultivendorWebViewer.Common;

namespace MultivendorWebViewer.Helpers
{
    public class CarouselSettings 
    {
        public CarouselSettings()
        {
            ShowNext = true;
            ShowPrevious = true;
        }

        [XmlIgnore]
        public bool? ShowNext { get; set; }

        [XmlAttribute("show-next")]
        public string ShowNextSerializable { get { return ShowNext.ToString(); } set { ShowNext = value.ToNullableBool(); } }

        [XmlAttribute("show-next-icon")]
        public string ShowNextIcon { get; set; }

        [XmlIgnore]
        public bool? ShowPrevious { get; set; }

        [XmlAttribute("show-previous")]
        public string ShowPreviousSerializable { get { return ShowPrevious.ToString(); } set { ShowPrevious = value.ToNullableBool(); } }

        [XmlAttribute("show-previous-icon")]
        public string ShowPreviousIcon { get; set; }

        [XmlIgnore]
        public bool? HoverShowNavigators { get; set; }

        [XmlAttribute("show-navigators-hover")]
        public string HoverShowNavigatorsSerializable { get { return HoverShowNavigators.ToString(); } set { HoverShowNavigators = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? ShowThumbnails { get; set; }

        [XmlAttribute("show-thumbnails")]
        public string ShowThumbnailsSerializable { get { return ShowThumbnails.ToString(); } set { ShowThumbnails = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? OmitEmpty { get; set; }

        [XmlAttribute("omit-empty")]
        public string OmitEmptySerializable { get { return OmitEmpty.ToString(); } set { OmitEmpty = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? ShowIndicators { get; set; }

        [XmlAttribute("show-indicators")]
        public string ShowIndicatorsSerializable { get { return ShowIndicators.ToString(); } set { ShowIndicators = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? HoverShowIndicators { get; set; }

        [XmlAttribute("show-indicators-hover")]
        public string HoverShowIndicatorsSerializable { get { return HoverShowIndicators.ToString(); } set { HoverShowIndicators = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? IndicatorNavigation { get; set; }

        [XmlAttribute("indicator-navigation")]
        public string IndicatorNavigationSerializable { get { return IndicatorNavigation.ToString(); } set { IndicatorNavigation = value.ToNullableBool(); } }

        [XmlIgnore]
        public CarouselIndicatorLayout? IndicatorsLayout { get; set; }

        [XmlAttribute("indicators-layout")]
        public string IndicatorsLayoutSerializable {  get { return IndicatorsLayout.ToString(); } set { IndicatorsLayout = value != null ? (CarouselIndicatorLayout?)Enum.Parse(typeof(CarouselIndicatorLayout), value, true) : null; } }

        [XmlIgnore]
        public HtmlContent IndicatorContent { get; set; }

        [XmlIgnore]
        public int? AutoSlideTime { get; set; }

        [XmlAttribute("auto-slide-time")]
        public string AutoSlideTimeSerializable { get { return AutoSlideTime.ToString(); } set { AutoSlideTime = value.ToNullableInt(); } }

        //[XmlAttribute("max-slides-visible")]
        //public int MaxSlidesVisible { get; set; }

        [XmlIgnore]
        public bool? OutOfBoundsSlidesVisible { get; set; }

        [XmlAttribute("outofbounds-slides-visible")]
        public string OutOfBoundsSlidesVisibleSerializable { get { return OutOfBoundsSlidesVisible.ToString(); } set { OutOfBoundsSlidesVisible = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? PartialSlidesVisible { get; set; }

        [XmlAttribute("partial-slides-visible")]
        public string PartialSlidesVisibleSerializable { get { return PartialSlidesVisible.ToString(); } set { PartialSlidesVisible = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? AllSlidesVisible { get; set; }

        [XmlAttribute("all-slides-visible")]
        public string AllSlidesVisibleSerializable { get { return AllSlidesVisible.ToString(); } set { AllSlidesVisible = value.ToNullableBool(); } }

        private string changeMode;
        [XmlAttribute("change-mode")]
        public string ChangeMode { get { return changeMode ?? CarouselSettings.ChangeModeSlide; } set { changeMode = value != null ? value.ToLowerInvariant() : null; } }

        [XmlIgnore]
        public bool? WrapNavigation { get; set; }

        [XmlAttribute("wrap-navigation")]
        public string WrapNavigationSerializable { get { return WrapNavigation.ToString(); } set { WrapNavigation = value.ToNullableBool(); } }

        [XmlAttribute("class")]
        public string ClassNames { get; set; }

        [XmlIgnore]
        public float? MouseOverZoomFactor { get; set; }

        [XmlAttribute("mouse-over-zoom")]
        public string MouseOverZoomFactorSerializable { get { return MouseOverZoomFactor.ToString(); } set { MouseOverZoomFactor = (float?)value.ToNullableDecimal(); } }


        protected  void MergeWithCore(CarouselSettings other)
        {

            if (other != null)
            {
                if (other.AllSlidesVisible.HasValue == true) AllSlidesVisible = other.AllSlidesVisible.Value;
                if (other.AutoSlideTime.HasValue == true) AutoSlideTime = other.AutoSlideTime.Value;
                if (other.ChangeMode != null) ChangeMode = other.ChangeMode;
                if (other.ClassNames != null) ClassNames = other.ClassNames;
                if (other.HoverShowIndicators.HasValue == true) HoverShowIndicators = other.HoverShowIndicators;
                if (other.HoverShowNavigators.HasValue == true) HoverShowNavigators = other.HoverShowNavigators;
                if (other.IndicatorNavigation.HasValue == true) IndicatorNavigation = other.IndicatorNavigation;
                if (other.IndicatorsLayout.HasValue == true) IndicatorsLayout = other.IndicatorsLayout;
                if (other.MouseOverZoomFactor.HasValue == true) MouseOverZoomFactor = other.MouseOverZoomFactor;
                if (other.OmitEmpty.HasValue == true) OmitEmpty = other.OmitEmpty;
                if (other.OutOfBoundsSlidesVisible.HasValue == true) OutOfBoundsSlidesVisible = other.OutOfBoundsSlidesVisible;
                if (other.PartialSlidesVisible.HasValue == true) PartialSlidesVisible = other.PartialSlidesVisible;
                if (other.ShowIndicators.HasValue == true) ShowIndicators = other.ShowIndicators;
                if (other.ShowNext.HasValue == true) ShowNext = other.ShowNext;
                if (other.ShowPrevious.HasValue == true) ShowPrevious = other.ShowPrevious;
                if (other.ShowNextIcon != null) ShowNextIcon = other.ShowNextIcon;
                if (other.ShowPreviousIcon != null) ShowPreviousIcon = other.ShowPreviousIcon;
                if (other.ShowThumbnails.HasValue == true) ShowThumbnails = other.ShowThumbnails;
                if (other.WrapNavigation.HasValue == true) WrapNavigation = other.WrapNavigation;
            }
        }

        public const string ChangeModeSlide = "slide";

        public const string ChangeModeSwap = "swap";
    }

    [Flags]
    public enum CarouselIndicatorLayout
    {
        [XmlEnum("left")] Left = 0x1, [XmlEnum("center")] Center = 0x2, [XmlEnum("left")] Right = 0x4, [XmlEnum("top")] Top = 0x8, [XmlEnum("middle")] Middle = 0x10, [XmlEnum("bottom")] Bottom = 0x20, [XmlEnum("below")] Below = 0x40, [XmlEnum("above")] Above = 0x80,
        Horizontal = Left | Center | Right, Vertical = Top | Middle | Bottom | Above | Below
    }

    public class CarouselSlide 
    {
        public bool Ignore { get; set; }

        private AttributeBuilder attributes;
        public AttributeBuilder Attributes { get { return attributes ?? (attributes = new AttributeBuilder()); } set { attributes = value; } }

        public bool HasAttributes { get { return attributes != null && attributes.Count > 0; } }

        public string Caption { get; set; }

        public HtmlContent Content { get; set; }

        public bool OverrideContent { get; set; }

        public HtmlContent ThumbnailContent { get; set; }

        public string ThumbnailCaption { get; set; }

        public bool Active { get; set; }

        public AttributeBuilder AddToAttributes(AttributeBuilder attributeBuilder, object obj = null, IFormatProvider formatProvider = null)
        {
            attributeBuilder.Class("slide");
            if (Caption != null) attributeBuilder.Attr("caption", Caption);
            if (HasAttributes == true)
            {
                attributeBuilder.Attr(Attributes);
            }
            return attributeBuilder;
        }

        public virtual void WriteContentHtml(HtmlHelper htmlHelper, TextWriter writer, int index, ApplicationRequestContext requestContext)
        {
            if (Content != null)
            {
                if (OverrideContent == false)
                {
                    var sliderTag = new TagBuilder("div");

                    var attributeBuilder = new AttributeBuilder("slide");//.Attr("data-index", index);
                    if (Active == true) attributeBuilder.Class("active");
                    //if (Caption != null) attributeBuilder.Attr("caption", requestContext.GetApplicationTextTranslation(Caption));
                    if (HasAttributes == true)
                    {
                        attributeBuilder.Attr(Attributes);
                    }

                    sliderTag.MergeAttributes(attributeBuilder);
                
                    sliderTag.Write(writer, TagRenderMode.StartTag);

                    Content.WriteContentHtml(htmlHelper, writer);

                    sliderTag.Write(writer, TagRenderMode.EndTag);
                }
                else
                {
                    Content.WriteContentHtml(htmlHelper, writer);
                }
            }
        }

        public virtual void WriteThumbnailContentHtml(HtmlHelper htmlHelper, TextWriter writer, int index, ApplicationRequestContext requestContext)
        {
            if (ThumbnailContent != null)
            {
                var thumbnailTag = new TagBuilder("div");
                var attributeBuilder = new AttributeBuilder("thumb").Attr("data-index", index);
                if (Active == true) attributeBuilder.Class("active");
                //if (ThumbnailCaption != null) attributeBuilder.Attr("caption", requestContext.GetApplicationTextTranslation(ThumbnailCaption));
                thumbnailTag.MergeAttributes(attributeBuilder);

                thumbnailTag.Write(writer, TagRenderMode.StartTag);

                ThumbnailContent.WriteContentHtml(htmlHelper, writer);

                thumbnailTag.Write(writer, TagRenderMode.EndTag);
            }
        }
    }

    public static class CarouselHelper
    {
        public static MvcHtmlString Carousel(this HtmlHelper htmlHelper, IEnumerable<CarouselSlide> slides, CarouselSettings settings = null, object htmlAttributes = null, ApplicationRequestContext requestContext = null)
        {
            //if (requestContext == null) requestContext = htmlHelper.ViewContext.GetApplicationRequestContext();

            using (var writer = new StringWriter())
            {
                CarouselInternal(htmlHelper, writer, slides, null, null, settings, htmlAttributes, requestContext);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        public static MvcHtmlString Carousel(this HtmlHelper htmlHelper, HtmlContent slidesContent, int slidesCount, CarouselSettings settings = null, object htmlAttributes = null, ApplicationRequestContext requestContext = null)
        {
            //if (requestContext == null) requestContext = htmlHelper.ViewContext.GetApplicationRequestContext();

            using (var writer = new StringWriter())
            {
                CarouselInternal(htmlHelper, writer, null, slidesContent, slidesCount, settings, htmlAttributes, requestContext);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        public static void RenderCarousel(this HtmlHelper htmlHelper, TextWriter writer, IEnumerable<CarouselSlide> slides, CarouselSettings settings = null, object htmlAttributes = null, ApplicationRequestContext requestContext = null)
        {
            CarouselInternal(htmlHelper, writer, slides, null, null, settings, htmlAttributes, requestContext );
        }

        public static void RenderCarousel(this HtmlHelper htmlHelper, TextWriter writer, HtmlContent slidesContent, int slidesCount, CarouselSettings settings = null, object htmlAttributes = null, ApplicationRequestContext requestContext = null)
        {
            CarouselInternal(htmlHelper, writer, null, slidesContent, slidesCount, settings, htmlAttributes, requestContext );
        }

        private static void CarouselInternal(this HtmlHelper htmlHelper, TextWriter writer, IEnumerable<CarouselSlide> slides, HtmlContent slidesContent, int? slidesCount = null, CarouselSettings settings = null, object htmlAttributes = null, ApplicationRequestContext requestContext = null)
        {
            if (settings == null)
            {
                settings = new CarouselSettings();
            }

            var validSlides = slides != null ? slides.Where(s => s.Ignore == false).ToArray() : null;
            int validSlidesCount = validSlides != null ? validSlides.Length : (slidesCount ?? 1);

            if (validSlidesCount > 0 || settings.OmitEmpty != true)
            {
                var controlTag = new TagBuilder("section");
                if (htmlAttributes != null) controlTag.MergeAttributes(htmlAttributes);
                controlTag.AddCssClass("multivendor-web-carousel");
                if (settings.ClassNames != null) controlTag.AddCssClass(settings.ClassNames);
                if (settings.OutOfBoundsSlidesVisible == true) controlTag.AddCssClass("outofbounds-visible");
                if (settings.PartialSlidesVisible == true) controlTag.AddCssClass("partial-visible");
                if (settings.AllSlidesVisible == true) controlTag.AddCssClass("all-visible");

                if (validSlidesCount > 1)
                {
                    if (settings.HoverShowNavigators != false) controlTag.AddCssClass("navigators-hover");
                    //if (settings.MaxSlidesVisible > 1) controlTag.MergeAttribute("data-max-visible", settings.MaxSlidesVisible.ToString());
                    if (settings.IndicatorNavigation == true) controlTag.AddCssClass("indicator-navigation");
                    if (settings.HoverShowIndicators == true) controlTag.AddCssClass("indicators-hover");
                    if (settings.IndicatorsLayout.HasValue == true)
                    {
                        CarouselIndicatorLayout indicatorsLayout = settings.IndicatorsLayout.Value;
                        if ((indicatorsLayout & CarouselIndicatorLayout.Horizontal) == 0) indicatorsLayout |= CarouselIndicatorLayout.Center;
                        if ((indicatorsLayout & CarouselIndicatorLayout.Vertical) == 0) indicatorsLayout |= CarouselIndicatorLayout.Bottom;
                        if (indicatorsLayout != (CarouselIndicatorLayout.Center | CarouselIndicatorLayout.Bottom)) controlTag.MergeAttribute("data-indicators-layout", settings.IndicatorsLayout.Value.ToString().ToLowerInvariant());
                    }
                    if (settings.WrapNavigation == true) controlTag.AddCssClass("wrap-navigation");
                    if (settings.AutoSlideTime > 0) controlTag.MergeAttribute("data-auto-slide", settings.AutoSlideTime.ToString());
                    controlTag.MergeAttribute("data-change-mode", settings.ChangeMode);
                }

                controlTag.MergeAttribute("data-slides", validSlidesCount.ToString());
                if (settings.MouseOverZoomFactor > 1) controlTag.MergeAttribute("data-mouseover-zoom", settings.MouseOverZoomFactor.ToString());



                controlTag.Write(writer, TagRenderMode.StartTag);

                // Slides
                var slidesContainerTag = new TagBuilder("section");
                slidesContainerTag.AddCssClass("slides-container");

                slidesContainerTag.Write(writer, TagRenderMode.StartTag);

                if (slidesContent != null)
                {
                    slidesContent.WriteContentHtml(htmlHelper, writer);
                }
                else
                {
                    for (int index = 0; index < validSlides.Length; index++)
                    {
                        var slide = validSlides[index];
                        slide.WriteContentHtml(htmlHelper, writer, index, requestContext);
                    }
                }

                slidesContainerTag.Write(writer, TagRenderMode.EndTag);

                // Indicators
                if (settings.ShowIndicators.HasValue == true ? settings.ShowIndicators.Value : validSlidesCount > 1)
                {
                    var indicatorsContainerTag = new TagBuilder("section");
                    indicatorsContainerTag.AddCssClass("indicators-container");
                    indicatorsContainerTag.Write(writer, TagRenderMode.StartTag);
                    for (int index = 0; index < validSlidesCount; index++)
                    {
                        var indicatorTag = new TagBuilder("div");
                        if (validSlides != null)
                        {
                            var slide = validSlides[index];
                            if (slide.Active == true) indicatorTag.AddCssClass("active");
                            if (settings.IndicatorContent == null)
                            {
                                indicatorTag.Write(writer, TagRenderMode.Normal);
                            }
                            else
                            {
                                indicatorTag.Write(writer, TagRenderMode.StartTag);
                                settings.IndicatorContent.WriteContentHtml(htmlHelper, writer, model: slide);
                                indicatorTag.Write(writer, TagRenderMode.EndTag);

                                //indicatorTag.InnerHtml.AppendHtml(settings.IndicatorContent.RenderContentHtml(htmlHelper, slide));
                                //indicatorTag.WriteTo(writer, HtmlEncoder.Default);
                            }
                        }
                        else
                        {
                            if (settings.IndicatorContent == null)
                            {
                                indicatorTag.Write(writer, TagRenderMode.Normal);
                            }
                            else
                            {
                                indicatorTag.Write(writer, TagRenderMode.StartTag);
                                settings.IndicatorContent.WriteContentHtml(htmlHelper, writer, model: null);
                                indicatorTag.Write(writer, TagRenderMode.EndTag);
                            }
                        }
                    }
                    indicatorsContainerTag.Write(writer, TagRenderMode.EndTag);
                }

                if (validSlidesCount > 1)
                {
                    // Navigators
                    if (settings.ShowPrevious.HasValue == true ? settings.ShowPrevious.Value : settings.ShowPreviousIcon != null)
                    {
                        writer.Write("<span class=\"nav prev icon fa fa-less-than\">");
                        writer.Write("</span>");
                    }

                    if (settings.ShowNext.HasValue == true ? settings.ShowNext.Value : settings.ShowNextIcon != null)
                    {
                       
                        writer.Write("<span class=\"nav next icon fa fa-greater-than\">");
                        writer.Write("</span>");
                    }

                    // Thumbnails
                    if (settings.ShowThumbnails != false && validSlides != null && validSlides.Where(s => s.ThumbnailContent != null).HasMinCount(2) == true)
                    {
                        var thumbnailsTag = new TagBuilder("section");
                        thumbnailsTag.AddCssClass("thumbnails-container");
                        thumbnailsTag.Write(writer, TagRenderMode.StartTag);

                        for (int index = 0; index < validSlides.Length; index++)
                        {
                            var slide = validSlides[index];
                            if (slide.ThumbnailContent != null)
                            {
                                slide.WriteThumbnailContentHtml(htmlHelper, writer, index, requestContext);
                            }
                        }

                        thumbnailsTag.Write(writer, TagRenderMode.EndTag);
                    }
                }

                controlTag.Write(writer, TagRenderMode.EndTag);
            }
        }
    }
}