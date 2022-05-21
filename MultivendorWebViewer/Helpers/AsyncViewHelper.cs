
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;


namespace MultivendorWebViewer.Helpers
{
    public class AsyncHtmlOptions
    {
        public AsyncHtmlOptions()
        {
            BatchLoad = AsyncHtmlOptions.DefaultBatchLoad;

            BatchLoadUrl = AsyncHtmlOptions.DefaultBatchLoadUrl;
        }

        public string Id { get; set; }

        public string HtmlUrl { get; set; }

        public bool BatchLoad { get; set; }

        public string BatchLoadUrl { get; set; }

        public int? Order { get; set; }

        public virtual IDictionary<string, object> ToHtmlAttributes()
        {
            var attributes = new Dictionary<string, object>();

            attributes.Add("data-async-html-options", JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            return attributes;
        }

        [JsonIgnore]
        public virtual string ClassString
        {
            get { return "multivendor-async"; }
        }

        [JsonIgnore]
        public virtual string AttributeString
        {
            get { return string.Concat("data-async-html-options=", JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), ""); }
        }

        public static bool DefaultBatchLoad { get; set; }

        public static string DefaultBatchLoadUrl { get; set; }
    }

    public class AsyncHtmlResult
    {
        //public HtmlContent Content { get; set; }

        public string ItemId { get; set; }

        public string Selector { get; set; }

        public string Html { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public HtmlInsertMethod InsertMethod { get; set; }

        public bool? Notify { get; set; }
    }

    public enum HtmlInsertMethod { Html, Replace, Append, Prepend, Before, After };

    public class Behaviours
    {
        public HtmlHelper Helper { get; set; }

        public virtual MvcHtmlString AsyncHtmlRequest(AsyncHtmlOptions options)
        {
            string optionsString = Newtonsoft.Json.JsonConvert.SerializeObject(options, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            //string attributeString = string.Concat("data-async-html=\"", htmlLoadUrl, "\" data-html.load-id=\"", id, "\"")
            return MvcHtmlString.Create(string.Concat("multivendor-async data-async-html=\"", optionsString, "\""));
        }
    }

    public class AsyncItem
    {
        public string BatchId { get; set; }
            
        public object Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public HtmlInsertMethod? InsertMethod { get; set; }

        public string Selector { get; set; }
    }

    public class AsyncItem<T> : AsyncItem
    {
        public T Data { get; set; }
    }

#if NET5
    public class Async : IHtmlContent
#else
    public class Async : IHtmlString
#endif
    {
        public string BatchId { get; set; }

        public string Url { get; set; }

        public object Data { get; set; } 

        public HtmlInsertMethod? InsertMethod { get; set; }

        public string Selector { get; set; }

        public int? LoadDelay { get; set; }

        public AttributeBuilder ItemAttributes(object id, HtmlInsertMethod? insertMethod = null, string selector = null)
        {
            return GetItemAttributes(id, BatchId, insertMethod, selector);
        }

#if NET5
        public IHtmlContent ItemContainer(object id, HtmlInsertMethod? insertMethod = null, string selector = null, string tagName = "div")
        {
            return null;
        }
#else
        public IHtmlString ItemContainer(object id, HtmlInsertMethod? insertMethod = null, string selector = null, string tagName = "div")
        {
            return null;
        }
#endif

        public void WriteTo(AttributeBuilder attributeBuilder)
        {
            attributeBuilder.Attr("data-async", Newtonsoft.Json.JsonConvert.SerializeObject(new { Url, Id = BatchId, Data, InsertMethod = InsertMethod.HasValue == true ? InsertMethod.Value.ToString() : null, Selector, LoadDelay }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

#if NET5
        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            writer.Write(string.Concat("data-async='", Newtonsoft.Json.JsonConvert.SerializeObject(new { Url, Id = BatchId, Data, InsertMethod = InsertMethod.HasValue == true ? InsertMethod.Value.ToString() : null, Selector, LoadDelay }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "'"));
        }
#else
        string IHtmlString.ToHtmlString()
        {
            return string.Concat("data-async='", Newtonsoft.Json.JsonConvert.SerializeObject(new { Url, Id = BatchId, Data, InsertMethod = InsertMethod.HasValue == true ? InsertMethod.Value.ToString() : null, Selector, LoadDelay }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "'");
        }
#endif

        public static AttributeBuilder GetItemAttributes(object id, string ownerBatchId = null, HtmlInsertMethod? insertMethod = null, string selector = null)
        {
            var asyncItem = new AsyncItem { Id = id, BatchId = ownerBatchId, InsertMethod = insertMethod, Selector = selector };
            return new AttributeBuilder("multivendor-async-item").Attr("data-async-item=", Newtonsoft.Json.JsonConvert.SerializeObject(asyncItem, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }

    public static class AsyncClassDecorators
    {
#if NET5
        public static IHtmlContent Async = new HtmlString("multivendor-async");

        public static IHtmlContent AsyncItem = new HtmlString("multivendor-async-item");
#else
        public static IHtmlString Async = new HtmlString("multivendor-async");

        public static IHtmlString AsyncItem = new HtmlString("multivendor-async-item");
#endif
    }

    public static class AsyncHtmlViewHelpers
    {
        public static MvcHtmlString AsyncView(this HtmlHelper htmlHelper, string url, object data = null, HtmlContent content = null, string batchId = null, HtmlInsertMethod? insertMethod = null, string selector = null, int? loadDelay = null, object htmlAttributes = null)
        {
            using (var htmlWriter = new StringWriter(CultureInfo.CurrentUICulture))
            {
                AsyncHtmlViewHelpers.AsyncView(htmlHelper, htmlWriter, url, data, content, batchId, insertMethod, selector, loadDelay, htmlAttributes);

                return MvcHtmlString.Create(htmlWriter.ToString());
            }
        }

        public static void AsyncView(this HtmlHelper htmlHelper, TextWriter writer, string url, object data = null, HtmlContent content = null, string batchId = null, HtmlInsertMethod? insertMethod = null, string selector = null, int? loadDelay = null, object htmlAttributes = null)
        {
            var tag = new TagBuilder("div");

            var attributeBuilder = new AttributeBuilder(htmlAttributes).Attr("data-async", Newtonsoft.Json.JsonConvert.SerializeObject(new { Url = url, Id = batchId, InsertMethod = insertMethod.HasValue == true ? insertMethod.Value.ToString() : null, Selector = selector, Data = data, LoadDelay = loadDelay }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            tag.MergeAttributes(attributeBuilder);

            tag.AddCssClass("multivendor-async");

            if (content == null)
            {
                tag.Write(writer, TagRenderMode.Normal);
            }
            else
            {
                tag.Write(writer, TagRenderMode.StartTag);

                content.WriteContentHtml(htmlHelper, writer);

                tag.Write(writer, TagRenderMode.EndTag);
            }
        }

        /// <summary>
        /// Write HTML attribute for specify loading of one or more async views. Needs to be complemented
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="url"></param>
        /// <param name="batchId"></param>
        /// <param name="insertMethod"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
#if NET5
        public static IHtmlContent Async(this HtmlHelper htmlHelper, string url, string batchId = null, object data = null, HtmlInsertMethod? insertMethod = null, string selector = null)
#else
        public static IHtmlString Async(this HtmlHelper htmlHelper, string url, string batchId = null, object data = null, HtmlInsertMethod? insertMethod = null, string selector = null)
#endif
        {
            return new HtmlString(string.Concat("data-async='", Newtonsoft.Json.JsonConvert.SerializeObject(new { Url = url, Id = batchId, Data = data, InsertMethod = insertMethod.HasValue == true ? insertMethod.Value.ToString() : null, Selector = selector }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "'"));
        }

        public static MvcHtmlString AsyncItemView(this HtmlHelper htmlHelper, object id, object data = null, HtmlContent content = null, string ownerBatchId = null, HtmlInsertMethod insertMethod = HtmlInsertMethod.Html, string selector = null, object htmlAttributes = null)
        {
            using (var htmlWriter = new StringWriter(CultureInfo.CurrentUICulture))
            {
                AsyncHtmlViewHelpers.AsyncItemView(htmlHelper, htmlWriter, id, data, content, ownerBatchId, insertMethod, selector, htmlAttributes);

                return MvcHtmlString.Create(htmlWriter.ToString());
            }
        }


        public static void AsyncItemView(this HtmlHelper htmlHelper, TextWriter writer, object id, object data = null, HtmlContent content = null, string ownerBatchId = null, HtmlInsertMethod? insertMethod = null, string selector = null, object htmlAttributes = null)
        {
            var tag = new TagBuilder("div");

            tag.MergeAttributes(htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            var asyncItem = new AsyncItem<object> { Id = id, BatchId = ownerBatchId, InsertMethod = insertMethod, Selector = selector, Data = data };

            tag.MergeAttribute("data-async-item", Newtonsoft.Json.JsonConvert.SerializeObject(asyncItem, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            tag.AddCssClass("multivendor-async-item");

            if (content == null)
            {
                tag.Write(writer, TagRenderMode.Normal);
            }
            else
            {
                tag.Write(writer, TagRenderMode.StartTag);

                content.WriteContentHtml(htmlHelper, writer);

                tag.Write(writer, TagRenderMode.EndTag);
            }
        }


        /// <summary>
        /// Write HTML attribute for specificy an async target
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="id"></param>
        /// <param name="ownerBatchId"></param>
        /// <param name="insertMethod"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
#if NET5
        public static IHtmlContent AsyncItem(this HtmlHelper htmlHelper, object id, string ownerBatchId = null, HtmlInsertMethod? insertMethod = null, string selector = null)
#else
        public static IHtmlString AsyncItem(this HtmlHelper htmlHelper, object id, string ownerBatchId = null, HtmlInsertMethod? insertMethod = null, string selector = null)
#endif
        {
            var asyncItem = new AsyncItem { Id = id, BatchId = ownerBatchId, InsertMethod = insertMethod, Selector = selector };
            return new HtmlString(string.Concat("data-async-item='", Newtonsoft.Json.JsonConvert.SerializeObject(asyncItem, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "'"));
        }
#if NET5
        public static IHtmlContent AsyncItem<T>(this HtmlHelper htmlHelper, object id, T data, string ownerBatchId = null, HtmlInsertMethod? insertMethod = null, string selector = null)
#else
        public static IHtmlString AsyncItem<T>(this HtmlHelper htmlHelper, object id, T data, string ownerBatchId = null, HtmlInsertMethod? insertMethod = null, string selector = null)
#endif
        {
            var asyncItem = new AsyncItem<T> { Id = id, BatchId = ownerBatchId, InsertMethod = insertMethod, Selector = selector, Data = data };
            return new HtmlString(string.Concat("data-async-item='", Newtonsoft.Json.JsonConvert.SerializeObject(asyncItem, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "'"));
        }
    }

}
