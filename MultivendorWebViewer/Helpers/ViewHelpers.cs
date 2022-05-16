#if NET5
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
#endif
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
#if NET452
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
#endif

namespace MultivendorWebViewer.Helpers
{
    public class ItemLayoutComponentOptions
    {
        //public AttributeBuilder Attributes { get; set; }

        public HtmlContent HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets if only HtmlContent should be rendered
        /// </summary>
        public bool HtmlContentOverrides { get; set; }

        //public NavigationSettings NavigationSettings { get; set; } // Should be in ComponentsSettings?

    }

    public class ItemLayoutOptions
    {
        //public string ItemTagName { get; set; }

        public AttributeBuilder ItemAttributes { get; set; }

    }

    public class ItemViewContext
    {
        public ItemViewContext()
        {
            ComponentsMode = ComponentsMode.All;
        }


        public ComponentsMode ComponentsMode { get; set; }

        public string ComponentClassNames { get; set; }



    }

    public static class ViewHelpers
    {
#if NET5
        //public static IHtmlHelper CreateHtmlHelper(this ActionContext actionContext, object model = null)
        //{
        //    var requestServices = actionContext.HttpContext.RequestServices;
        //    var newHelper = requestServices.GetRequiredService<IHtmlHelper>();

        //    if (newHelper is IViewContextAware contextable)
        //    {
        //        var newViewContext = new ViewContext(viewContext, viewContext.View, viewData, viewContext.Writer);
        //        contextable.Contextualize(newViewContext);
        //    }

        //    return newHelper;
        //}

        public static ViewContext CreateViewContext(this ActionContext actionContext, IView view, TextWriter writer, ViewDataDictionary viewData = null, ITempDataDictionary tempData = null, object model = null)
        {
            var requestServices = actionContext.HttpContext.RequestServices;

            if (viewData == null)
            {
                var modelMetadataProvider = requestServices.GetRequiredService<IModelMetadataProvider>();

                viewData = new ViewDataDictionary(modelMetadataProvider, actionContext.ModelState);
                if (model != null)
                {
                    viewData.Model = model;
                }
            }
            else if (model != null && viewData.Model != model)
            {
                viewData = new ViewDataDictionary(viewData);
                viewData.Model = model;
            }


            if (tempData == null)
            {
                var tempDataFactory = requestServices.GetRequiredService<ITempDataDictionaryFactory>();

                tempData = tempDataFactory.GetTempData(actionContext.HttpContext);
            }

            if (writer == null)
            {
                //var writerFactory = requestServices.GetRequiredService<IHttpResponseStreamWriterFactory>();
                //writerFactory.CreateWriter(actionContext.HttpContext.Response, )
            }

            var viewOptions = requestServices.GetRequiredService<IOptions<MvcViewOptions>>().Value;

            var viewContext = new ViewContext(
                actionContext,
                view,
                viewData,
                tempData,
                writer ?? TextWriter.Null,
                viewOptions.HtmlHelperOptions);

            return viewContext;
        }

        //public static ViewContext CreateViewContext(ActionContext context, ViewComponentResult result)
        //{
        //    var response = context.HttpContext.Response;
        //    var requestServices = actionContext.HttpContext.RequestServices;
        //    var _modelMetadataProvider = requestServices.GetRequiredService<IModelMetadataProvider>();

        //    var viewData = result.ViewData;
        //    if (viewData == null)
        //    {
        //        viewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);

        //        //viewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
        //        //{
        //        //    Model = model,
        //        //};
        //    }

        //    var tempData = result.TempData;
        //    if (tempData == null)
        //    {
        //        tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);
        //    }

        //    var _writerFactory = context.HttpContext.RequestServices.GetRequiredService<IHttpResponseStreamWriterFactory>();

        //    await using (var writer = _writerFactory.CreateWriter(response.Body, resolvedContentTypeEncoding))
        //    {
        //        var viewContext = new ViewContext(
        //            context,
        //            NullView.Instance,
        //            viewData,
        //            tempData,
        //            writer,
        //            _htmlHelperOptions);

        //        // IViewComponentHelper is stateful, we want to make sure to retrieve it every time we need it.
        //        var viewComponentHelper = context.HttpContext.RequestServices.GetRequiredService<IViewComponentHelper>();
        //        (viewComponentHelper as IViewContextAware)?.Contextualize(viewContext);
        //        var viewComponentResult = await GetViewComponentResult(viewComponentHelper, _logger, result);

        //        if (viewComponentResult is ViewBuffer viewBuffer)
        //        {
        //            // In the ordinary case, DefaultViewComponentHelper will return an instance of ViewBuffer. We can simply
        //            // invoke WriteToAsync on it.
        //            await viewBuffer.WriteToAsync(writer, _htmlEncoder);
        //            await writer.FlushAsync();
        //        }
        //        else
        //        {
        //            await using var bufferingStream = new FileBufferingWriteStream();
        //            await using (var intermediateWriter = _writerFactory.CreateWriter(bufferingStream, resolvedContentTypeEncoding))
        //            {
        //                viewComponentResult.WriteTo(intermediateWriter, _htmlEncoder);
        //            }

        //            await bufferingStream.DrainBufferAsync(response.Body);
        //        }
        //    }
        //}

        public static HtmlHelper CreateHtmlHelper(ViewContext viewContext, ViewDataDictionary viewData)
        {
            var newHelper = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlHelper>();

            if (newHelper is IViewContextAware contextable)
            {
                var newViewContext = new ViewContext(viewContext, viewContext.View, viewData, viewContext.Writer);
                contextable.Contextualize(newViewContext);
            }

            return newHelper as HtmlHelper;
        }
#endif
        public static HtmlHelper CreateHtmlHelper(this ControllerContext controllerContext, object model = null)
        {

            return CreateHtmlHelper(controllerContext.Controller, model);
        }

        public static HtmlHelper CreateHtmlHelper(this ControllerBase controller, object model = null)
        {

            var viewContext = new ViewContext(controller.ControllerContext, new EmpyView(), model == null ? controller.ViewData : new ViewDataDictionary(controller.ViewData) { Model = model }, controller.TempData, TextWriter.Null);
            return new HtmlHelper(viewContext, new ViewPage());

        }


        public static HtmlHelper CreateHtmlHelper<T>(this ControllerContext controllerContext, T model = null)
            where T : class
        {
            return CreateHtmlHelper<T>(controllerContext.Controller, model);
        }

        public static HtmlHelper<T> CreateHtmlHelper<T>(this ControllerBase controller, T model = null)
            where T : class
        {
            return CreateHtmlHelper<T>(controller, model, TextWriter.Null);
        }

        public static HtmlHelper<T> CreateHtmlHelper<T>(this ControllerBase controller, T model, TextWriter writer)
            where T : class
        {
            var viewContext = new ViewContext(controller.ControllerContext, new EmpyView(), model == null ? controller.ViewData : new ViewDataDictionary(controller.ViewData) { Model = model }, controller.TempData, writer);
            return new HtmlHelper<T>(viewContext, new ViewPage());
        }


        public class EmpyView : IView
        {
#if NET5
            public string Path => string.Empty;

            public Task RenderAsync(ViewContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return Task.CompletedTask;
            }
#endif
            public void Render(ViewContext viewContext, System.IO.TextWriter writer)
            {
                throw new InvalidOperationException();
            }

        }

        public static ViewContext CreateViewContext(this HtmlHelper htmlHelper, ViewDataDictionary viewData = null, object model = null, TextWriter writer = null)
        {
            ViewDataDictionary newViewData = null;

            if (model == null)
            {
                if (viewData == null)
                {
                    newViewData = new ViewDataDictionary(htmlHelper.ViewData);
                }
                else
                {
                    newViewData = new ViewDataDictionary(viewData);
                }
            }
            else
            {
                if (viewData == null)
                {
                    newViewData = new ViewDataDictionary(htmlHelper.ViewData) { Model = model };
                }
                else
                {
                    newViewData = new ViewDataDictionary(viewData) { Model = model };
                }
            }

            return new ViewContext(htmlHelper.ViewContext, htmlHelper.ViewContext.View, newViewData, htmlHelper.ViewContext.TempData, writer ?? htmlHelper.ViewContext.Writer);

        }


        public class ExtendedViewContext : ViewContext, IViewDataContainer
        {
            public ExtendedViewContext() { }

            public ExtendedViewContext(ControllerContext controllerContext, IView view, ViewDataDictionary viewData, TempDataDictionary tempData, TextWriter writer)
                : base(controllerContext, view, viewData, tempData, writer)
            {
            }

            //public ComponentLayoutMode? ComponentLayoutMode { get; set; }

            public ItemViewContext ItemViewContext { get; set; }
        }

        public static ItemViewContext GetItemViewContext(this HtmlHelper htmlHelper)
        {

            return htmlHelper.ViewData["itemViewContext"] as ItemViewContext;
        }

        public static void SetItemViewContext(this HtmlHelper htmlHelper, ItemViewContext itemViewContext)
        {

            htmlHelper.ViewData["itemViewContext"] = itemViewContext;
        }

        public static ExtendedViewContext CreateViewContext<T>(this HtmlHelper htmlHelper, ViewDataDictionary viewData = null, T model = null, TextWriter writer = null)
            where T : class
        {
            ViewDataDictionary<T> newViewData = null;

            if (model == null)
            {
                if (viewData == null)
                {
                    if (htmlHelper.ViewData.Model is T)
                    {
                        newViewData = new ViewDataDictionary<T>(htmlHelper.ViewData);
                    }
                    else
                    {
#if NET452
                        newViewData = new ViewDataDictionary<T>();
#endif
                    }
                }
                else
                {
                    if (viewData.Model is T)
                    {
                        newViewData = new ViewDataDictionary<T>(viewData);
                    }
                    else
                    {
#if NET452
                        newViewData = new ViewDataDictionary<T>();
#endif
                    }
                }
            }
            else
            {
                if (viewData == null)
                {
                    newViewData = new ViewDataDictionary<T>(model);

                }
                else
                {
                    if (viewData.Model is T)
                    {
                        newViewData = new ViewDataDictionary<T>(viewData) { Model = model };
                    }
                    else
                    {
                        newViewData = new ViewDataDictionary<T>(model);

                    }
                }
            }
            return new ExtendedViewContext(htmlHelper.ViewContext, htmlHelper.ViewContext.View, newViewData, htmlHelper.ViewContext.TempData, writer ?? htmlHelper.ViewContext.Writer);

        }

#if NET452
        public static ExtendedViewContext CreateViewContext(ControllerContext controllerContext, IView view = null, ViewDataDictionary viewData = null, object model = null, TextWriter writer = null)
        {
            var controller = controllerContext.Controller;

            ViewDataDictionary newViewData = null;

            if (model == null)
            {
                if (viewData == null)
                {
                    newViewData = new ViewDataDictionary(controller.ViewData);
                }
                else
                {
                    newViewData = new ViewDataDictionary(viewData);
                }
            }
            else
            {
                if (viewData == null)
                {
                    newViewData = new ViewDataDictionary(model);
                }
                else
                {
                    newViewData = new ViewDataDictionary(viewData) { Model = model };
                }
            }

            return new ExtendedViewContext(controllerContext, view, newViewData, controller.TempData, writer ?? controllerContext.HttpContext.Response.Output);
        }
#endif

#if NET452
        public static IView GetPartialView(this HtmlHelper htmlHelper, string partialViewName, ViewContext viewContext = null)
        {
            return ViewEngines.Engines.FindPartialView(viewContext ?? ViewHelpers.CreateViewContext(htmlHelper), partialViewName).View;
        }
#endif
        public static void RenderPartialView(this HtmlHelper htmlHelper, IView view, ViewContext viewContext, TextWriter writer = null)
        {
            view.Render(viewContext, writer ?? viewContext.Writer);

        }

        public static void RenderPartialView(this HtmlHelper htmlHelper, IView view, object model = null, ViewDataDictionary viewData = null, TextWriter writer = null)
        {
            ViewHelpers.RenderPartialView(htmlHelper, view, ViewHelpers.CreateViewContext(htmlHelper, viewData, model, writer));
        }

#if NET5
        public static void RenderPartial(this IHtmlHelper htmlHelper, string partialViewName, TextWriter writer, ViewDataDictionary viewData = null, object model = null)
        {
            writer.Write("RenderPartial is not implemented in core5");
        }

        public static void RenderPartial(this IHtmlHelper htmlHelper, string partialViewName, ItemRenderer itemRenderer, object model = null)
        {
            var currentItemRenderer = htmlHelper.ViewData["itemRenderer"] as ItemRenderer;
            htmlHelper.ViewData["itemRenderer"] = itemRenderer;

            htmlHelper.RenderPartial(partialViewName, model);

            if (currentItemRenderer != null)
            {
                htmlHelper.ViewData["itemRenderer"] = currentItemRenderer;
            }
            else
            {
                htmlHelper.ViewData.Remove("itemRenderer");
            }
        }
#else

        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, TextWriter writer, ViewDataDictionary viewData = null, object model = null)
        {
            ViewContext newViewContext = CreateViewContext(htmlHelper, viewData, model, writer);

            IView view = ViewEngines.Engines.FindPartialView(newViewContext, partialViewName).View;

            view.Render(newViewContext, writer);
        }

        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, ItemRenderer itemRenderer, object model = null)
        {
            var currentItemRenderer = htmlHelper.ViewData["itemRenderer"] as ItemRenderer;
            htmlHelper.ViewData["itemRenderer"] = itemRenderer;

            bool writeAlreadySuppressed = itemRenderer.SuppressWrite;
            itemRenderer.SuppressWrite = true;
            //htmlHelper.RenderPartial(partialViewName, model:model);
            if (writeAlreadySuppressed == false)
            {
                itemRenderer.SuppressWrite = false;
            }

            if (currentItemRenderer != null)
            {
                htmlHelper.ViewData["itemRenderer"] = currentItemRenderer;
            }
            else
            {
                htmlHelper.ViewData.Remove("itemRenderer");
            }
        }
#endif

        public static string Partial(this HtmlHelper htmlHelper, string partialViewName, ViewDataDictionary viewData = null, object model = null)
        {
            using (var writer = new StringWriter(CultureInfo.CurrentUICulture))
            {
                ViewHelpers.RenderPartial(htmlHelper, partialViewName, writer, viewData, model);

                return writer.ToString();
            }
        }

#if NET452
        public static void RenderPartial(ControllerContext controllerContext, string partialViewName, TextWriter writer, ViewDataDictionary viewData = null, object model = null)
        {
            ViewEngineResult result = ViewEngines.Engines.FindPartialView(controllerContext, partialViewName);

            IView view = result.View;

            ViewContext newViewContext = CreateViewContext(controllerContext, view, viewData, model, writer);

            view.Render(newViewContext, writer);

            result.ViewEngine.ReleaseView(controllerContext, view);

        }
#else
        public static void RenderPartial(ControllerContext controllerContext, string partialViewName, TextWriter writer, ViewDataDictionary viewData = null, object model = null)
        {
            writer.Write("RenderPartial is not implemented in core5");
        }
#endif

        public static string RenderPartial(ControllerContext controllerContext, string partialViewName, ViewDataDictionary viewData = null, object model = null)
        {
            using (var writer = new StringWriter(CultureInfo.CurrentUICulture))
            {
                ViewHelpers.RenderPartial(controllerContext, partialViewName, writer, viewData, model);
                return writer.ToString();
            }
        }

#if NET5
        public static string RenderPartial(ActionContext context, string partialViewName, object model = null)
        {
            var services = context.HttpContext.RequestServices;
            var executor = services.GetService<IActionResultExecutor<PartialViewResult>>();

            return "RenderPartial is not implemented in core5";
            //using (var writer = new StringWriter(CultureInfo.CurrentUICulture))
            //{
            //    ViewHelpers.RenderPartial(context., partialViewName, writer, viewData, model);
            //    return writer.ToString();
            //}
        }

        public static ViewEngineResult FindPartialView(ActionContext actionContext, string viewName)
        {
            var services = actionContext.HttpContext.RequestServices;
            var viewEngine = services.GetService<ICompositeViewEngine>();
            return FindPartialView(actionContext, viewEngine, viewName);
        }

        public static ViewEngineResult FindPartialView(ActionContext actionContext, IViewEngine viewEngine, string viewName)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var result = viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: false);
            var originalResult = result;
            if (!result.Success)
            {
                result = viewEngine.FindView(actionContext, viewName, isMainPage: false);
            }

            if (!result.Success)
            {
                if (originalResult.SearchedLocations.Any())
                {
                    if (result.SearchedLocations.Any())
                    {
                        // Return a new ViewEngineResult listing all searched locations.
                        var locations = new List<string>(originalResult.SearchedLocations);
                        locations.AddRange(result.SearchedLocations);
                        result = ViewEngineResult.NotFound(viewName, locations);
                    }
                    else
                    {
                        // GetView() searched locations but FindView() did not. Use first ViewEngineResult.
                        result = originalResult;
                    }
                }
            }

            return result;
        }
#endif


#if NET452
        public static MvcHtmlString DisplayFor(this HtmlHelper htmlHelper, object model, string templateName = null, bool useCache = true)
        {
            using (var writer = new StringWriter(CultureInfo.CurrentUICulture))
            {
                ViewHelpers.DisplayFor(htmlHelper, model, writer, templateName, useCache);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        private static IEnumerable<string> GetTemplateNames(object model, string templateName)
        {
            if (templateName != null)
            {
                yield return templateName;
            }

            var type = model != null ? model.GetType() : null;

            while (type != null)
            {
                var uiHintAttributes = type.GetCustomAttributes<UIHintAttribute>(inherit: false, inheritAttributes: true);
                var uiHintAttribute = uiHintAttributes.FirstOrDefault(a => String.Equals(a.PresentationLayer, "MVC", StringComparison.OrdinalIgnoreCase)) ?? uiHintAttributes.FirstOrDefault(a => String.IsNullOrEmpty(a.PresentationLayer));

                if (uiHintAttribute != null && string.IsNullOrEmpty(uiHintAttribute.UIHint) == false)
                {
                    yield return uiHintAttribute.UIHint;
                }

                yield return type.IsGenericType == false ? type.Name : type.Name.Remove(type.Name.IndexOf('`'));

                type = type.BaseType;
            }
        }

        //private static Dictionary<string, IView> cachedDisplayTemplated = new Dictionary<string, IView>();

        public static void DisplayFor(this HtmlHelper htmlHelper, object model, TextWriter writer, string templateName = null, bool useCache = true)
        {
            IView view = null;

            var viewContext = htmlHelper.ViewContext;

            try
            {
                string controllerName = viewContext.RouteData.GetRequiredString("controller");
                //viewContext.Controller.GetType().Name;

                //var cacheKeyList = new List<string>();

                foreach (string viewTemplateName in GetTemplateNames(model, templateName))
                {
                    string fullViewName = string.Concat("DisplayTemplates/", viewTemplateName);

                    //string cacheKey = string.Concat(controllerName, "/", fullViewName);

                    //if (useCache == false || cachedDisplayTemplated.TryGetValue(cacheKey, out view) == false)
                    //{
                    //cacheKeyList.Add(cacheKey);

                    view = ViewEngines.Engines.FindPartialView(viewContext, fullViewName).View;
                    //}

                    if (view != null)
                    {
                        break;
                    }
                }

                //foreach (string cacheKey in cacheKeyList)
                //{
                //    cachedDisplayTemplated[cacheKey] = view;
                //}
            }
            catch (Exception exception)
            {
                Log.WriteLine(System.Diagnostics.TraceEventType.Error, "Could not get display", exception);
            }

            if (view == null)
            {
                throw new Exception(string.Format("Could not find display template {0}", templateName));
            }

            if (writer == null)
            {
                writer = viewContext.Writer;
            }

            var newViewContext = CreateViewContext(htmlHelper, model: model, writer: writer);

            view.Render(newViewContext, writer);
        }
#endif
        //public static bool HasAnyPresentationInformation(IPresentationViewModel presentation, Common.ComponentLayoutMode mode = Common.ComponentLayoutMode.Body, PresentationSettings overrideSettings = null)
        //{
        //    var settings = overrideSettings ?? presentation.Settings;
        //    if ((settings.IdentitySettings.DisplayIn(mode) == true && presentation.HasIdentity == true)
        //        || (settings.PartNumberSettings.DisplayIn(mode) == true && presentation.HasPartNumber == true)
        //        || (settings.NameSettings.DisplayIn(mode) == true && presentation.HasName == true)
        //        || (settings.ToolBarSettings.DisplayIn(mode) == true)
        //        || (settings.DescriptionSettings.DisplayIn(mode) == true && presentation.HasDescription == true)
        //        || (settings.SearchSettings.DisplayIn(mode) == true && presentation.DisplaySearch == true)
        //        || (settings.FootnotesSettings.DisplayIn(mode) == true && presentation.HasFootnotes == true)
        //        || (settings.BulletinsSettings.DisplayIn(mode) == true && presentation.ApplicationRequestContext.Configuration.BulletinsEnabled == true && presentation.HasBulletins == true)
        //        || (settings.OrderDisplaySettings.DisplayIn(mode) == true && presentation.HasPartNumber == true)
        //        || (settings.UsedIn.DisplayIn(mode) == true && presentation.HasUsedIn == true)
        //        || (settings.FilterDisplaySettings.DisplayIn(mode) == true && presentation.HasFilters == true)
        //        || (settings.ReplacementSettings.DisplayIn(mode) == true && presentation.HasReplacements == true && presentation.ApplicationRequestContext.HasPermission(AuthorizePermissions.PartReplacement, defaultPermission: true))
        //        || (settings.CrossSalesDisplaySettings.DisplayIn(mode) == true)
        //        || (settings.Properties.DisplayIn(mode) == true && presentation.HasProperties == true)
        //        || (presentation.HasHighlightedProperties == true && presentation.HighlightedProperties.Any(p => p.DisplayIn(mode, ComponentLayoutMode.Body) == true))
        //        || (presentation.HasImages == true && presentation.Images.Any(i => i.Settings.DisplayIn(mode, ComponentLayoutMode.BodyLeftSide) == true) == true))
        //    {
        //        return true;
        //    }
        //    return false;
        //}



        public class LayoutTextWriter : TextWriter
        {
            public LayoutTextWriter(TextWriter innerWriter)
            {
                InnerWriter = innerWriter;
            }

            public StringBuilder GetStringBuilder()
            {
                var stringWriter = InnerWriter as StringWriter;
                if (stringWriter != null) return stringWriter.GetStringBuilder();
                var layoutWriter = InnerWriter as LayoutTextWriter;
                if (layoutWriter != null) return layoutWriter.GetStringBuilder();

                return null;
            }

            public TextWriter InnerWriter { get; private set; }

            public Action<TextWriter> PreContentWriter { get; set; }

            public bool IgnorePrependingWhiteSpace { get; set; }

            public override Encoding Encoding { get { return InnerWriter.Encoding; } }

            public override void Write(char value)
            {
                if (IgnorePrependingWhiteSpace == true)
                {
                    if (char.IsWhiteSpace(value) == true) return;
                    IgnorePrependingWhiteSpace = false;
                }

                if (PreContentWriter == null)
                {
                    InnerWriter.Write(value);
                }
                else
                {
                    PreContentWriter(InnerWriter);
                    PreContentWriter = null;
                    InnerWriter.Write(value);
                }
            }

            public override void Write(string value)
            {
                if (IgnorePrependingWhiteSpace == true)
                {
                    if (string.IsNullOrWhiteSpace(value) == true) return;
                    IgnorePrependingWhiteSpace = false;
                }

                if (PreContentWriter == null)
                {
                    InnerWriter.Write(value);
                }
                else
                {
                    PreContentWriter(InnerWriter);
                    PreContentWriter = null;
                    InnerWriter.Write(value);
                }
            }

            public override string ToString()
            {
                return InnerWriter.ToString();
            }
        }

        //public static MvcHtmlString ItemsLayout<T>(this HtmlHelper htmlHelper, string partialViewName, IEnumerable<T> items, ComponentLayoutMode displayModes = ComponentLayoutMode.Visible, Action<T, ItemLayoutOptions> options = null)
        //    where T : class
        //{
        //    using (var writer = new StringWriter())
        //    {
        //        ViewHelpers.RenderItemsLayoutInternal(htmlHelper, writer, partialViewName, null, items, displayModes, options);
        //        return MvcHtmlString.Create(writer.ToString());
        //    }
        //}

        /// <summary>
        /// Renders a collection of item layouts. The item content is rendered with default RenderItemView.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="items"></param>
        /// <param name="displayModes"></param>
        /// <param name="options"></param>
        //public static void RenderItemsLayout<T>(this HtmlHelper htmlHelper, IEnumerable<T> items, ComponentLayoutMode displayModes = ComponentLayoutMode.Visible, Action<T, ItemLayoutOptions> options = null)
        //     where T : class, IItemViewModel
        //{
        //    RenderItemsLayout(htmlHelper, htmlHelper.ViewContext.Writer, items, displayModes, options);
        //}

        /// <summary>
        /// Renders a collection of items layouts. The item content is rendered with default RenderItemView.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="writer"></param>
        /// <param name="items"></param>
        /// <param name="displayModes"></param>
        /// <param name="options"></param>
        //        public static void RenderItemsLayout<T>(this HtmlHelper htmlHelper, TextWriter writer, IEnumerable<T> items, ComponentLayoutMode displayModes = ComponentLayoutMode.Visible, Action<T, ItemLayoutOptions> options = null)
        //            where T : class, IItemViewModel
        //        {
        //#if NET452
        //            var applicationRequestContext = htmlHelper.GetApplicationRequestContext();
        //            ViewHelpers.RenderItemsLayoutInternal(htmlHelper, writer, null, (helper, item, context) => new System.Web.WebPages.HelperResult(w =>
        //            {
        //                RenderItemViewInternal(helper, w, item, context.LayoutMode, ComponentsMode.All, context.ItemSettings, context.NavigationUrl, context.ComponentClassNames, applicationRequestContext);
        //            }), items, displayModes, options);
        //#else
        //            writer.Write("RenderItemsLayout not implemented in core5");
        //#endif
        //        }

        /// <summary>
        /// Renders a collection of item layouts. The item content is rendered with specified partial view. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="partialViewName"></param>
        /// <param name="items"></param>
        /// <param name="displayModes"></param>
        /// <param name="options"></param>
        //public static void RenderItemsLayout<T>(this HtmlHelper htmlHelper, string partialViewName, IEnumerable<T> items, ComponentLayoutMode displayModes = ComponentLayoutMode.Visible, Action<T, ItemLayoutOptions> options = null)
        //    where T : class
        //{
        //    ViewHelpers.RenderItemsLayoutInternal(htmlHelper, htmlHelper.ViewContext.Writer, partialViewName, null, items, displayModes, options);
        //}

        /// <summary>
        /// Renders a collection of items layouts. The item content is rendered with specified partial view. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="writer"></param>
        /// <param name="partialViewName"></param>
        /// <param name="items"></param>
        /// <param name="displayModes"></param>
        /// <param name="options"></param>
        //public static void RenderItemsLayout<T>(this HtmlHelper htmlHelper, TextWriter writer, string partialViewName, IEnumerable<T> items, ComponentLayoutMode displayModes = ComponentLayoutMode.Visible, Action<T, ItemLayoutOptions> options = null)
        //    where T : class
        //{
        //    ViewHelpers.RenderItemsLayoutInternal(htmlHelper, writer, partialViewName, null, items, displayModes, options);
        //}

        /// <summary>
        /// Renders a collection of items layouts. The item content is rendered with specified helper result provider. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="writer"></param>
        /// <param name="resultProvider"></param>
        /// <param name="items"></param>
        /// <param name="displayModes"></param>
        /// <param name="options"></param>
        //public static void RenderItemsLayout<T>(this HtmlHelper htmlHelper, TextWriter writer, Func<HtmlHelper<T>, T, ItemViewContext, HelperResult> resultProvider, IEnumerable<T> items, ComponentLayoutMode displayModes = ComponentLayoutMode.Visible, Action<T, ItemLayoutOptions> options = null)
        //    where T : class
        //{
        //    ViewHelpers.RenderItemsLayoutInternal(htmlHelper, writer, null, resultProvider, items, displayModes, options);
        //}

        /// <summary>
        /// Renders a collection of items layouts. The item content is rendered with specified helper result provider. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="writer"></param>
        /// <param name="resultProvider"></param>
        /// <param name="items"></param>
        /// <param name="displayModes"></param>
        /// <param name="options"></param>
        //        public static void RenderItemsLayout<T>(this HtmlHelper htmlHelper, Func<HtmlHelper<T>, T, ItemViewContext, HelperResult> resultProvider, IEnumerable<T> items, ComponentLayoutMode displayModes = ComponentLayoutMode.Visible, Action<T, ItemLayoutOptions> options = null)
        //            where T : class
        //        {
        //            ViewHelpers.RenderItemsLayoutInternal(htmlHelper, htmlHelper.ViewContext.Writer, null, resultProvider, items, displayModes, options);
        //        }

        //        private static void RenderItemsLayoutInternal<T>(HtmlHelper htmlHelper, TextWriter writer, string partialViewName, Func<HtmlHelper<T>, T, ItemViewContext, HelperResult> resultProvider, IEnumerable<T> items, ComponentLayoutMode displayModes, Action<T, ItemLayoutOptions> options)
        //            where T : class
        //        {
        //            var layoutWriter = new LayoutTextWriter(writer) { IgnorePrependingWhiteSpace = true };
        //            var viewContext = CreateViewContext<T>(htmlHelper, viewData: htmlHelper.ViewData, model: (T)null, writer: layoutWriter);
        //#if NET5
        //            var view = partialViewName != null ? FindPartialView(viewContext, partialViewName).View : null;
        //#else
        //            var view = partialViewName != null ? ViewEngine.Default.FindReuseablePartialView(viewContext, partialViewName).View : null;
        //#endif
        //            var context = new RenderItemLayoutContext<T>(viewContext, view, resultProvider, layoutWriter);
        //            foreach (var item in items)
        //            {
        //                var itemAttributes = new AttributeBuilder();
        //                var itemOptions = new ItemLayoutOptions { ItemAttributes = itemAttributes };

        //                //var itemViewModel = item as IItemViewModel;
        //                //if (itemViewModel != null)
        //                //{
        //                //    var itemSettings = itemViewModel.Settings;
        //                //    if (itemSettings != null)
        //                //    {
        //                //        itemOptions.ItemSettings = itemSettings;

        //                //        itemAttributes.Class(itemSettings.ClassName);
        //                //        if (itemSettings.HtmlAttributes != null && itemSettings.HtmlAttributes.Count > 0)
        //                //        {
        //                //            itemSettings.HtmlAttributes.AddToAttributes(itemAttributes, item, context.FormatterContext);
        //                //        }

        //                //        if (itemSettings.NavigationSettings != null && itemSettings.NavigationSettings.Mode == NavigationElementMode.RootElement)
        //                //        {
        //                //            itemOptions.ItemTagName = "a";
        //                //            var url = itemViewModel.GetNavigationUrl();
        //                //            if (url != null)
        //                //            {
        //                //                itemOptions.NavigationUrl = url;
        //                //                url.AddToAttributes(itemAttributes);
        //                //            }
        //                //        }
        //                //    }                              
        //                //}

        //                if (options != null)
        //                {
        //                    options(item, itemOptions);
        //                }
        //                if (viewContext != null)
        //                {
        //                    viewContext.ViewData.Model = item;
        //                }
        //                context.Item = item;
        //                ViewHelpers.RenderItemLayoutInternal<T>(context, displayModes, itemOptions, false);
        //            }
        //        }

        //        public static void RenderItemsListLayout<T>(this HtmlHelper htmlHelper, string partialViewName, IEnumerable<T> items, Action<T, ItemLayoutOptions> options = null,  object htmlAttributes = null)
        //            where T : class
        //        {
        //            ViewHelpers.RenderItemsListLayout<T>(htmlHelper, htmlHelper.ViewContext.Writer, partialViewName, items, options);
        //        }

        //        public static void RenderItemsListLayout<T>(this HtmlHelper htmlHelper, TextWriter writer, string partialViewName, IEnumerable<T> items, ComponentLayoutMode displayModes = ComponentLayoutMode.Visible, Action<T, ItemLayoutOptions> options = null, ListSettings<T> settings = null, object htmlAttributes = null)
        //            where T : class
        //        {
        //            var layoutWriter = new LayoutTextWriter(writer) { IgnorePrependingWhiteSpace = true };
        //            var viewContext = CreateViewContext<T>(htmlHelper, viewData: htmlHelper.ViewData, model: (T)null, writer: layoutWriter);
        //#if NET5
        //            var context = new RenderItemLayoutContext<T>(viewContext, FindPartialView(viewContext, partialViewName).View, null, layoutWriter);
        //#else
        //            var context = new RenderItemLayoutContext<T>(viewContext, ViewEngine.Default.FindReuseablePartialView(viewContext, partialViewName).View, null, layoutWriter);
        //#endif
        //            htmlHelper.List(writer, items, (h, w, item) =>
        //            {
        //                var itemOptions = new ItemLayoutOptions { ItemAttributes = new AttributeBuilder() };
        //                if (options != null) options(item, itemOptions);
        //                viewContext.ViewData.Model = context.Item = item;
        //                ViewHelpers.RenderItemLayoutInternal<T>(context, displayModes, itemOptions, false);
        //            }, settings: settings, htmlAttributes: htmlAttributes);
        //        }

        private class RenderItemLayoutContext<T>
        {
            public RenderItemLayoutContext(ExtendedViewContext viewContext, IView view, Func<HtmlHelper<T>, T, ItemViewContext, HelperResult> resultProvider, LayoutTextWriter layoutWriter)
            {
                ViewContext = viewContext;
                View = view;
                ResultProvider = resultProvider;
                LayoutWriter = layoutWriter;
            }

            public T Item { get; set; }

            private HtmlHelper<T> htmlHelper;
#if NET5
            public HtmlHelper<T> HtmlHelper { get { return htmlHelper; } }
#else
            public HtmlHelper<T> HtmlHelper { get { return htmlHelper ?? (htmlHelper = new HtmlHelper<T>(ViewContext, ViewContext)); } }
#endif

            public ExtendedViewContext ViewContext { get; set; }

            public ItemViewContext ItemViewContext { get; set; }

            public IView View { get; set; }

            public Func<HtmlHelper<T>, T, ItemViewContext, HelperResult> ResultProvider { get; set; }

            public LayoutTextWriter LayoutWriter { get; set; }


        }


        public class RenderElement
        {
            public Action<ItemRenderer> RenderAction { get; set; }

            public float? Order { get; set; }

            //public string Group { get; set; }

            //public IComponentLayout Layout { get; set; }
        }

        public class ItemRenderer
        {
            public ItemRenderer(HtmlHelper htmlHelper, ApplicationRequestContext requestContext) : this(htmlHelper, requestContext, htmlHelper.GetItemViewContext()) { }

            public ItemRenderer(HtmlHelper htmlHelper, ApplicationRequestContext requestContext, TextWriter writer = null)
            {
                Html = htmlHelper;
                RequestContext = requestContext;
                this.writer = writer;
                elements = new List<RenderElement>();
            }

            public ItemRenderer(HtmlHelper htmlHelper, ApplicationRequestContext requestContext, ItemViewContext itemViewContext, TextWriter writer = null)
            {
                Html = htmlHelper;
                RequestContext = requestContext;

                this.writer = writer;
                elements = new List<RenderElement>();
            }

            public ApplicationRequestContext RequestContext { get; private set; }

            public HtmlHelper Html { get; private set; }

            private TextWriter writer;
            public TextWriter Writer { get { return writer ?? (writer = Html.ViewContext.Writer); } }

            public ItemViewContext ItemViewContext { get; private set; }


            private List<RenderElement> elements;

            private List<RenderElementGroup> groupedElements;



            public bool SuppressWrite { get; set; }

            public string Separator { get; set; }

            public void Clear()
            {
                elements.Clear();
                groupedElements = null;
            }

            private class RenderElementGroup
            {
                public RenderElementGroup(string name, float order, bool showTab, string label)
                {
                    Name = name;
                    Elements = new List<RenderElement>();
                    Order = order;
                    ShowTab = showTab;
                    Label = label;
                }

                public string Name { get; private set; }

                public bool ShowTab { get; private set; }

                public string Label { get; private set; }

                public List<RenderElement> Elements { get; private set; }

                public float Order { get; private set; }
            }

            private class RenderElementTab
            {
                public RenderElementTab()
                {
                    Elements = new List<RenderElement>();
                }

                public TabItem Item { get; set; }

                public List<RenderElement> Elements { get; private set; }

                //public static void Add(List<TabRenderer> tabs, ComponentSettings settings, HtmlContent htmlContent)
                //{
                //    string name = settings.DisplayGroup;
                //    var tab = tabs.Where(t => t.Item.Name == name).FirstOrDefault();
                //    if (tab == null)
                //    {
                //        tab = new TabRenderer() { Item = new TabItem { Name = name } };
                //        tabs.Add(tab);
                //    }
                //    if (tab.Item.HeaderLabel == null && settings.Label != null)
                //    {
                //        tab.Item.HeaderLabel = settings.Label;
                //    }

                //    tab.Contents.Add(htmlContent);
                //}
            }

            //public void Render(Action<ItemRenderer> action, IComponentLayout componentLayout)
            //{
            //    if (componentLayout != null)
            //    {
            //        Render(action, componentLayout.DisplayOrder, componentLayout.DisplayGroup, componentLayout.DisplayGroupOrder, componentLayout.EnableTab, componentLayout.DisplayGroupLabel);
            //    }
            //    else
            //    {
            //        Render(action);
            //    }
            //}

            public void Render(Action<ItemRenderer> action, float? order = null, string group = null, float? groupOrder = null, bool showTab = false, string label = null)
            {
                if (group == null)
                {
                    elements.Add(new RenderElement { RenderAction = action, Order = order });
                }
                else /*if (showTab == false)*/
                {
                    RenderElementGroup elementGroup;
                    if (groupedElements == null)
                    {
                        groupedElements = new List<RenderElementGroup>();
                        elementGroup = new RenderElementGroup(group, groupOrder ?? 0, showTab, label);
                        groupedElements.Add(elementGroup);
                    }
                    else
                    {
                        elementGroup = groupedElements.FirstOrDefault(g => g.Name == group);
                        if (elementGroup == null)
                        {
                            elementGroup = new RenderElementGroup(group, groupOrder ?? 0, showTab, label);
                            groupedElements.Add(elementGroup);
                        }
                    }
                    elementGroup.Elements.Add(new RenderElement { RenderAction = action, Order = order });
                }
            }

            public void RenderHtml(Func<object, HelperResult> result)
            {
                //Render(ctx => result(null).WriteTo(ctx.Writer), componentLayout);
                Render(ctx => new HtmlContent(result).WriteContentHtml(ctx.Html, ctx.Writer));
            }

            public void RenderHtml(Func<object, HelperResult> result, float? order = null, string group = null, float? groupOrder = null, bool enableTab = false)
            {
                //Render(ctx => result(null).WriteTo(ctx.Writer), componentLayout);
                Render(ctx => new HtmlContent(result).WriteContentHtml(ctx.Html, ctx.Writer), order, group, groupOrder, enableTab);
            }

            public void RenderHtml(HtmlContent htmlContent)
            {
                Render(ctx => htmlContent.WriteContentHtml(ctx.Html, ctx.Writer));
            }

            public void RenderHtml(HtmlContent htmlContent, float? order = null, string group = null, float? groupOrder = null)
            {
                Render(ctx => htmlContent.WriteContentHtml(ctx.Html, ctx.Writer), order, group, groupOrder);
            }

            //public void Write()
            //{
            //    if (SuppressWrite == true) return;

            //    if (elements.Count > 0)
            //    {
            //        if (elements.Count > 1)
            //        {
            //           /* elements.InsertionSort(new RenderElementComparer())*/;
            //            elements[0].RenderAction(this);
            //            for (int i = 1; i < elements.Count; i++)
            //            {
            //                if (Separator != null)
            //                {
            //                    Writer.Write(Separator);
            //                }
            //                elements[i].RenderAction(this);
            //            }
            //        }
            //        else
            //        {
            //            elements[0].RenderAction(this);
            //        }
            //    }

            //    if (groupedElements != null)
            //    {
            //        var tabs = new List<TabItem>();
            //        foreach (var elementGroup in groupedElements.OrderBy(g => g.Order))
            //        {
            //            if (elementGroup.ShowTab == true) // TAB
            //            {
            //                //var tabItem = new TabItem { Name = elementGroup.Name, HeaderLabel = this.RequestContext.GetApplicationTextTranslation(elementGroup.Label) };
            //                tabItem.Content = new HtmlContent((HtmlHelper h, TextWriter w) =>
            //                {
            //                    var currentWriter = Writer;
            //                    writer = w;

            //                    var groupElements = elementGroup.Elements;
            //                    if (groupElements.Count > 1)
            //                    {
            //                        //groupElements.InsertionSort(new RenderElementComparer());
            //                        groupElements[0].RenderAction(this);
            //                        for (int i = 1; i < groupElements.Count; i++)
            //                        {
            //                            if (Separator != null)
            //                            {
            //                                w.Write(Separator);
            //                            }
            //                            groupElements[i].RenderAction(this);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        groupElements[0].RenderAction(this);
            //                    }

            //                    writer = currentWriter;
            //                });
            //                tabs.Add(tabItem);
            //            }
            //            else
            //            {
            //                Writer.WriteLine("<div class=\"group ");
            //                Writer.WriteLine(elementGroup.Name);
            //                Writer.WriteLine("\">");

            //                if (!string.IsNullOrEmpty(elementGroup.Label) == true)
            //                {
            //                    Writer.WriteLine("<label class=\"group-label\">" + this.RequestContext.GetApplicationTextTranslation(elementGroup.Label) + "</label>");
            //                }

            //                var groupElements = elementGroup.Elements;
            //                if (groupElements.Count > 1)
            //                {
            //                    groupElements.InsertionSort(new RenderElementComparer());
            //                    groupElements[0].RenderAction(this);
            //                    for (int i = 1; i < groupElements.Count; i++)
            //                    {
            //                        if (Separator != null)
            //                        {
            //                            Writer.Write(Separator);
            //                        }
            //                        groupElements[i].RenderAction(this);
            //                    }
            //                }
            //                else
            //                {
            //                    groupElements[0].RenderAction(this);
            //                }
            //                Writer.WriteLine("</div>");
            //            }
            //        }

            //        if (tabs.Count > 0)
            //        {
            //            Html.Tab(Writer, tabs, settings: new TabSettings() { Hottrack = false, IsPresentationTab = true }, htmlAttributes: new { @class = "presentation-tab", data_tab_item_count = tabs.Count() });
            //        }
            //    }
            //}

            public class RenderElementComparer : IComparer<RenderElement>
            {
                public int Compare(RenderElement x, RenderElement y)
                {
                    var a = x.Order;
                    var b = y.Order;
                    if (a.HasValue == false)
                    {
                        if (b.HasValue == false) return 0;
                        if (b.Value > 0) return -1;
                        return 1;
                    }

                    if (b.HasValue == false)
                    {
                        if (a.Value >= 0) return 1;
                        return -1;
                    }

                    if (a.Value < b.Value) return -1;
                    if (a.Value > b.Value) return 1;

                    return 0;
                }
            }
        }

        public static ItemRenderer GetItemRenderer(this HtmlHelper htmlHelper, ApplicationRequestContext requestContext)
        {
            var itemRenderer = htmlHelper.ViewData["itemRenderer"] as ItemRenderer;
            return itemRenderer ?? new ItemRenderer(htmlHelper, requestContext);
        }

        public class ImageCollectionOptions
        {
            //public Func<IImageViewModel, ImageSettings> ImageSettingsProvider { get; set; }

            public string DefaultControl { get; set; }

            public string OverrideControl { get; set; }

            public Func<ImageViewModel, AttributeBuilder> ImageHtmlAttributeProvider { get; set; }

            //public Func<IImageViewModel, NavigationUrl> ImageUrlProvider { get; set; }

            public int? MaxImageCount { get; set; }

            public static ImageCollectionOptions Default = new ImageCollectionOptions();
        }

        public static void RenderImages(this HtmlHelper htmlHelper, IEnumerable<ImageViewModel> images, ImageCollectionOptions options = null, AttributeBuilder htmlAttributes = null, ApplicationRequestContext requestContext = null)
        {
            ViewHelpers.RenderImages(htmlHelper, htmlHelper.ViewContext.Writer, images, options, htmlAttributes, requestContext);
        }

        public static void RenderImages(this HtmlHelper htmlHelper, TextWriter writer, IEnumerable<ImageViewModel> images, ImageCollectionOptions options = null, AttributeBuilder htmlAttributes = null, ApplicationRequestContext requestContext = null)
        {
            if (images != null)
            {
                if (options == null) options = ImageCollectionOptions.Default;

                var stringComparer = StringComparer.OrdinalIgnoreCase;
                var carouselSettings = new CarouselSettings();

                var groupHtmlAttributes = new AttributeBuilder(htmlAttributes) // Image Collection Attributes
                    .Attr("data-group", ""); // Group id

                var slides = images.Select(image => new CarouselSlide
                {
                    //Content = Slide(i, imageGroupSettings.CarouselSettings),
                    Content = new HtmlContent((HtmlHelper h, TextWriter w) =>
                    {
                        var itemHtmlAttributes = new AttributeBuilder() // Image Settings Attributes
                        .Attr(options.ImageHtmlAttributeProvider != null ? options.ImageHtmlAttributeProvider(image) : null); // Image Provider Attributes

                        //var url = null;
                        //if (url != null)
                        //{
                        //    url.WriteStartTag(writer, itemHtmlAttributes.Class("image-container"));
                        //    ViewHelpers.RenderImage(htmlHelper, writer, image, htmlAttributes: "image");
                        //    writer.Write("</a>");
                        //}
                        //else
                        //{
                        ViewHelpers.RenderImage(htmlHelper, writer, image, htmlAttributes: itemHtmlAttributes.Class("image"));
                        //}
                    }),
                    ////Attributes = i.Settings != null ? i.Settings.GetAttributes(Html, obj: i, formatterContext: formatterContext) : null, // Image Settings Attributes
                    ThumbnailContent = new HtmlContent((HtmlHelper h, TextWriter w) =>
                    {
                        ViewHelpers.RenderImageThumbnail(htmlHelper, w, image);
                    })
                });

                htmlHelper.RenderCarousel(writer, slides, carouselSettings, groupHtmlAttributes, requestContext);
                //foreach (var imageGroup in imageGroups)
                //{
                //    Type groupType = imageGroup.Key.Item1;
                //    string groupId = imageGroup.Key.Item2;
                //    bool customTypeImage = groupType != typeof(ImageViewModel);

                //    var imageGroupSettings = imageGroup.Where(i => i.Settings != null).Select(i => i.Settings).FirstOrDefault();

                //    var groupImages = imageGroup.OrderBy(id => id.Settings != null ? id.Settings.DisplayOrder : 0).ToArray();

                //    string viewType = null;
                //    if (imageGroupSettings != null)
                //    {
                //        viewType = string.IsNullOrEmpty(imageGroupSettings.ViewType) == true || stringComparer.Equals(imageGroupSettings.ViewType, ImageSettings.ViewTypeAuto) ? null : imageGroupSettings.ViewType;
                //    }

                //    // Custom View
                //    if (imageGroupSettings != null && imageGroupSettings.ViewName != null)
                //    {
                //        htmlHelper.RenderPartial(imageGroupSettings.ViewName, new ImageCollectionViewModel(groupImages, htmlAttributes: htmlAttributes));
                //    }
                //    // Carousel
                //    else if (
                //        ((viewType == null && imageGroupSettings != null && imageGroupSettings.CarouselSettings != null)
                //        || (imageGroupSettings != null && stringComparer.Equals(imageGroupSettings.ViewType, ImageSettings.ViewTypeCarousel))) && groupImages.Length > 1
                //        ) // Or check properties like PopUp / mouse over zoom
                //    {

                //    }

                //    // Image View (zoom pan etc.)
                //    else if (imageGroupSettings != null && customTypeImage == false && ((viewType == null && imageGroupSettings.ImageViewSettings != null) || stringComparer.Equals(imageGroupSettings.ViewType, ImageSettings.ViewTypeControl) || stringComparer.Equals(imageGroupSettings.ViewType, ImageSettings.ViewTypeIllustration)))
                //    {
                //        var isIllustrationView = imageGroupSettings.ViewType != null && stringComparer.Equals(imageGroupSettings.ViewType, ImageSettings.ViewTypeIllustration) ? true : false;
                //        var thumbnailSize = imageGroupSettings.ImageViewSettings.ThumbnailSize ?? new ImageSize { Width = 80, Height = 80, SuperSampling = 6 };
                //        var imageDefinitions = groupImages.Select(i => new ImageViewDefinition
                //        {
                //            LoadMode = ImageViewLoadMode.Delayed,
                //            LoadUrl = UrlUtility.Action(requestContext, (isIllustrationView ? "GetIllustrationImageView" : "GetImageView"), "Illustration", routeValues: new RouteValueDictionary { { "id", i.Id } }),
                //            ThumbCaption = i.Caption,
                //            ThumbUrl = UrlUtility.Action(requestContext, "ImageThumbnail", "Content", new RouteValueDictionary { { "id", i.Id }, { "width", thumbnailSize.Width ?? thumbnailSize.Height ?? 80 }, { "height", thumbnailSize.Height ?? thumbnailSize.Width ?? 80 }, { "sampling", thumbnailSize.SuperSampling ?? 6 } }),
                //        });

                //        writer.Write(htmlHelper.ImagesViewer(imageDefinitions, htmlAttributes: htmlAttributes)); // TODO!!!
                //    }

                //    // Simple images
                //    else if (groupImages.Length > 1)
                //    {
                //        var groupHtmlAttributes = new AttributeBuilder(htmlAttributes) // Image Collection Attributes
                //            .Attr("data-group", groupId); // Group id
                //        writer.Write("<section ");
                //        groupHtmlAttributes.WriteTo(writer, groupHtmlAttributes);
                //        writer.Write('>');

                //        foreach (var image in groupImages)
                //        {
                //            var settings = image.Settings;
                //            var itemHtmlAttributes = (image.Settings != null ? image.Settings.GetAttributes(obj: image, formatterContext: formatterContext) : new AttributeBuilder()) // Image Settings Attributes
                //                .Attr(options.ImageHtmlAttributeProvider != null ? options.ImageHtmlAttributeProvider(image) : null);  // Image Provider Attributes
                //            var url = options.ImageUrlProvider == null ? null : options.ImageUrlProvider(image);
                //            if (url != null)
                //            {
                //                url.WriteStartTag(writer, itemHtmlAttributes.Class("image-container"));
                //                ViewHelpers.RenderImage(htmlHelper, writer, image, htmlAttributes: "image");
                //                writer.Write("</a>");
                //            }
                //            else
                //            {
                //                ViewHelpers.RenderImage(htmlHelper, writer, image, htmlAttributes: itemHtmlAttributes.Class("image"));
                //            }
                //        }

                //        writer.Write("</section>");
                //    }

                //    // Simple image
                //    else
                //    {
                //        var image = groupImages[0];

                //        var imageSettings = image.Settings;

                //        var itemHtmlAttributes = (imageSettings != null ? imageSettings.GetAttributes(obj: image, formatterContext: formatterContext) : new AttributeBuilder()) // Image Settings Attributes
                //            .Attr(htmlAttributes) // Image Collection Attributes
                //            .Attr("data-group", groupId); // Group id

                //        if (options.ImageHtmlAttributeProvider != null)
                //        {
                //            var perImageAttributes = options.ImageHtmlAttributeProvider(image);
                //            if (perImageAttributes.Count > 0)
                //            {
                //                itemHtmlAttributes.Attr(perImageAttributes);
                //            }
                //        }

                //        var arId = imageSettings != null && imageSettings.Size != null ? imageSettings.Size.AspectRatioId : null;
                //        if (arId.HasValue == true)
                //        {
                //            itemHtmlAttributes.Class("ar ar" + arId.Value);
                //        }

                //        var url = options.ImageUrlProvider == null ? null : options.ImageUrlProvider(image);
                //        if (url != null)
                //        {
                //            url.WriteStartTag(writer, itemHtmlAttributes.Class("image-container"));
                //            ViewHelpers.RenderImage(htmlHelper, writer, image, htmlAttributes: "image");
                //            writer.Write("</a>");
                //        }
                //        else
                //        {
                //            if (arId.HasValue == true && (imageSettings == null || imageSettings.AsBackground != true))
                //            {
                //                writer.Write("<div ");
                //                itemHtmlAttributes.Class("image-container").WriteTo(writer);
                //                writer.Write('>');
                //                ViewHelpers.RenderImage(htmlHelper, writer, image, htmlAttributes: "image");
                //                writer.Write("</div>");
                //            }
                //            else
                //            {
                //                ViewHelpers.RenderImage(htmlHelper, writer, image, htmlAttributes: itemHtmlAttributes.Class("image"));
                //            }
                //        }
                //    }
                //}
            }
        }

        public static MvcHtmlString Image(this HtmlHelper htmlHelper, ImageViewModel image, object htmlAttributes = null)
        {
            if (image == null) return MvcHtmlString.Empty;

            using (var writer = new StringWriter())
            {
                RenderImage(htmlHelper, writer, image, htmlAttributes: htmlAttributes);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        public static MvcHtmlString Image(this HtmlHelper htmlHelper, ImageViewModel image, int? width, int? height, int? sampling = null, bool? cover = null, bool? asBackground = null, object htmlAttributes = null)
        {
            if (image == null) return MvcHtmlString.Empty;

            using (var writer = new StringWriter())
            {
                ImageSize size = width.HasValue || height.HasValue ? new ImageSize() { Height = height, Width = width } : null;
                RenderImage(htmlHelper, writer, image, imageSize: size, htmlAttributes: htmlAttributes);
                return MvcHtmlString.Create(writer.ToString());
            }
        }



        public static void RenderImage(this HtmlHelper htmlHelper, TextWriter writer, ImageViewModel image,ImageSize imageSize = null, object htmlAttributes = null)
        {
            // if we have a size and that size is unnessesary big, ask for a thumbnail of the image instead
            if (image != null)
            {
                        var attributes = new AttributeBuilder(htmlAttributes);
                           attributes.Class("cover");
                        var url = imageSize == null? image.GetUrl() : image.GetThumbnailUrl(width: imageSize.Width, height: imageSize.Height);
                        writer.Write("<div class=\"image-container\"> ");
                        writer.Write("<img ");
                        attributes.WriteTo(writer);
                        writer.Write(" src=\"");
                        writer.Write(url);
                        writer.Write("\"/>");
                        writer.Write("</div>");


            }
        }

        public static MvcHtmlString ImageThumbnail(this HtmlHelper htmlHelper, ImageViewModel image, object htmlAttributes = null)
        {
            if (image == null) return MvcHtmlString.Empty;

            using (var writer = new StringWriter())
            {
                RenderImageThumbnail(htmlHelper, writer, image, htmlAttributes: htmlAttributes);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        public static MvcHtmlString ImageThumbnail(this HtmlHelper htmlHelper, ImageViewModel image, int? width, int? height, int? sampling = null, bool? cover = null, bool? asBackground = null, object htmlAttributes = null)
        {
            if (image == null) return MvcHtmlString.Empty;

            using (var writer = new StringWriter())
            {
                RenderImageThumbnail(htmlHelper, writer, image, htmlAttributes/*width, height, sampling, cover, htmlAttributes: htmlAttributes, requestContext: image.ApplicationRequestContext*/);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        public static void RenderImageThumbnail(this HtmlHelper htmlHelper, TextWriter writer, ImageViewModel image, object htmlAttributes = null)
        {
            string url = image.GetThumbnailUrl();

            var attributes = new AttributeBuilder(htmlAttributes);

            writer.Write("<div ");
            attributes.WriteTo(writer);
            writer.Write(" style=\"background-image:url(");
            writer.Write(url);
            writer.Write(")\"></div>");
        }

        public static MvcHtmlString ComponentContainer(this HtmlHelper htmlHelper, string tagName, TagRenderMode tagRenderMode = TagRenderMode.Normal, AttributeBuilder htmlAttributes = null)
        {
            using (var writer = new StringWriter())
            {
                RenderComponentContainer(htmlHelper, writer, tagName, tagRenderMode, htmlAttributes);
                return new MvcHtmlString(writer.ToString());
            }
        }

        public static void RenderComponentContainer(this HtmlHelper htmlHelper, string tagName, TagRenderMode tagRenderMode = TagRenderMode.Normal, AttributeBuilder htmlAttributes = null)
        {
            RenderComponentContainer(htmlHelper, htmlHelper.ViewContext.Writer, tagName, tagRenderMode, htmlAttributes);
        }

        public static void RenderComponentContainer(this HtmlHelper htmlHelper, TextWriter writer, string tagName, TagRenderMode tagRenderMode = TagRenderMode.Normal, AttributeBuilder htmlAttributes = null)
        {
           
                tagName = "div";

            if (tagRenderMode == TagRenderMode.EndTag)
            {
                writer.Write("</");
                writer.Write(tagName);
                writer.Write('>');
            }
            else
            {
                writer.Write('<');
                writer.Write(tagName);
                htmlAttributes = htmlAttributes != null ? new AttributeBuilder(htmlAttributes) : new AttributeBuilder();
                
                if (htmlAttributes.Count > 0)
                {
                    writer.Write(' ');
                    htmlAttributes.WriteTo(writer);
                }
                if (tagRenderMode == TagRenderMode.SelfClosing)
                {
                    writer.Write("/>");
                }
                else
                {
                    writer.Write('>');
                }
            }

        }
    }
    public class ImageSize{
       public int? Height { get; set; }
     public   int? Width { get; set; }
    }

    public enum ComponentsMode { None = 0x0, Image = 0x1, Identity = 0x2, PartNumber = 0x4, Name = 0x8, Description = 0x10, Properties = 0x20, HighlightedProperties = 0x40, References = 0x80, Bulletins = 0x100, Footnotes = 0x200, Icon = 0x400, Toolbar = 0x800, Widgets = 0x1000, All = 0xFFFF }
}

