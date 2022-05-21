using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Helpers;
using Newtonsoft.Json;


namespace MultivendorWebViewer.Controllers
{
    public class BaseController : Controller
    {
        public ApplicationRequestContext ApplicationRequestContext { get { return ApplicationRequestContext.GetContext(HttpContext); } }
        // GET: Base
        public virtual ActionResult Index()
        {            
            return View();
        }
        protected virtual HtmlAsyncResult AsyncHtml(string partialView, object model, string selector = null, HtmlInsertMethod insertMethod = HtmlInsertMethod.Replace, bool notify = true)
        {
            return new HtmlAsyncResult(partialView, model, selector, insertMethod, notify);
        }

    }
    public class HtmlAsyncResult : ActionResult
    {
        public HtmlAsyncResult(string partialView, object model, string selector = null, HtmlInsertMethod insertMethod = HtmlInsertMethod.Replace, bool? notify = null)
        {
            this.partialView = partialView;
            this.model = model;
            this.selector = selector;
            this.insertMethod = insertMethod;
            this.notify = notify;
        }

        public HtmlAsyncResult(string html, string selector = null, HtmlInsertMethod insertMethod = HtmlInsertMethod.Replace, bool? notify = null)
        {
            this.html = html ?? "";
            this.selector = selector;
            this.insertMethod = insertMethod;
            this.notify = notify;
        }

        public HtmlAsyncResult(params AsyncHtmlResult[] htmlResults)
        {
            this.results = htmlResults;
        }

        private AsyncHtmlResult[] results;

        private string html;
        private string partialView;
        private object model;
        private HtmlInsertMethod insertMethod;
        private string selector;
        private bool? notify;


        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";

            if (this.results != null)
            {
                var serializedObject = JsonConvert.SerializeObject(new { AsyncResults = this.results }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                response.Write(serializedObject);
            }
            else
            {
                var htmlResults = new List<AsyncHtmlResult>();

                if (html != null)
                {
                    htmlResults.Add(new AsyncHtmlResult()
                    {
                        Html = html,
                        Selector = selector,
                        InsertMethod = insertMethod,
                        Notify = notify
                    });
                }
                else if (partialView != null)
                {
                    string viewHtml = ViewHelpers.RenderPartial(context, partialView, model: model);

                    htmlResults.Add(new AsyncHtmlResult()
                    {
                        Html = viewHtml,
                        Selector = selector,
                        InsertMethod = insertMethod,
                        Notify = notify
                    });
                }

                var serializedObject = JsonConvert.SerializeObject(new { AsyncResults = htmlResults }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                response.Write(serializedObject);
            }
        }

    }
}