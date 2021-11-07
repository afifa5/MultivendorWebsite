using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
#if NET5
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing.Template;
#else
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
#endif
using Newtonsoft.Json.Serialization;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using MultivendorWebViewer.Common;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;

namespace MultivendorWebViewer
{
    public class ValueSerializationSettings
    {
        public ValueSerializationSettings(bool ignoreNull = true, params string[] ignoredValues)
        {
            HtmlEncode = true;
            IgnoreNull = ignoreNull;
            IgnoredValues = ignoredValues != null && ignoredValues.Length > 0 ? ignoredValues : null;
        }

        public bool HtmlEncode { get; set; }

        public bool IgnoreNull { get; set; }

        public IEnumerable<string> IgnoredValues { get; set; }

        public bool IsValueIgnored(string key)
        {
            return IgnoredValues != null && IgnoredValues.Contains(key);
        }

        internal static ValueSerializationSettings Default = new ValueSerializationSettings();

        internal static ValueSerializationSettings Empty = new ValueSerializationSettings();
    }

    public static class UrlUtility
    {
        //public static string ToQueryString(IDictionary<string, object> values, ValueSerializationSettings settings = null, string prefix = "?")
        //{
        //    if (values == null || values.Count == 0)
        //    {
        //        return null;
        //    }

        //    if (settings == null)
        //    {
        //        settings = ValueSerializationSettings.Default;
        //    }

        //    var builder = new StringBuilder();
        //    foreach (var entry in values)
        //    {
        //        if ((settings.IgnoreNull == false || entry.Value != null) && (settings.IgnoredValues == null || settings.IgnoredValues.Contains(entry.Key) == false))
        //        {
        //            builder.Append(builder.Length == 0 ? prefix : "&");
        //            builder.Append(entry.Key);
        //            builder.Append("=");
        //            if (settings.HtmlEncode == true)
        //            {
        //                builder.Append(entry.Value != null ? HttpUtility.UrlEncode(entry.Value.ToString()) : null);
        //            }
        //            else
        //            {
        //                builder.Append(entry.Value);
        //            }
        //        }
        //    }

        //    return builder.Length > 0 ? builder.ToString() : null;
        //}

       
        //public static string ToQueryString(object values, ValueSerializationSettings settings = null, string prefix = "?")
        //{
        //    if (values == null)
        //    {
        //        return null;
        //    }

        //    var dictionary = values as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(values);

        //    return UrlUtility.ToQueryString(dictionary, settings, prefix);
        //}

#if NET452
        public static string StripQuery(Uri path, out string query)
        {
            return StripQuery(path.ToString(), out query);
        }

        public static string StripQuery(string path, out string query)
        {
            int queryIndex = path.IndexOf('?');
            if (queryIndex >= 0)
            {
                query = path.Substring(queryIndex);
                return path.Substring(0, queryIndex);
            }
            else
            {
                query = null;
                return path;
            }
        }
#endif
        public static string RemoveQueryParameter(string uri, string paramater)
        {
            if (uri == null) return null;

            int index = uri.IndexOf('?');
            if (index != -1)
            {
                string noneQueryPart = uri.Substring(0, index);

                // If the '?' is the last char in string, the query string is empty
                if (index == uri.Length - 1) return noneQueryPart;

                string queryString = uri.Substring(index + 1);
                var query = HttpUtility.ParseQueryString(queryString);
                query.Remove(paramater);

                return query.Count > 0 ? string.Concat(noneQueryPart, "?", query.ToString()) : noneQueryPart;
            }

            return uri;
        }

#if NET452
        public static string RouteUrl(RequestContext context, RouteValueDictionary routeValues, string query = null)
        {
            return new UrlHelper(context).RouteUrl(routeValues, query);
        }

        //public static string RouteUrl(RequestContext requestContext, RouteValueDictionary routeValues, NameValueCollection query = null)
        //{
        //    return new UrlHelper(requestContext).RouteUrl(routeValues, query);
        //}

        public static string RouteUrl(RequestContext context, RouteValueDictionary routeValues, IDictionary<string, object> query = null)
        {
            return new UrlHelper(context).RouteUrl(routeValues, query);
        }

        public static string RouteUrl(RequestContext context, RouteValueDictionary routeValues, object query)
        {
            return new UrlHelper(context).RouteUrl(routeValues, HtmlHelper.AnonymousObjectToHtmlAttributes(query));
        }

        [Obsolete("Use Action method with a request context or application request context")]
        public static string Action(string actionName = null, string controllerName = null, object routeValues = null, State state = null)
        {
            var requestContext = HttpContext.Current.Request.RequestContext;
            return Action(requestContext.GetApplicationRequestContext(), actionName, controllerName, routeValues, state);
        }
#endif

        //public static string Action(RequestContext requestContext, string actionName = null, string controllerName = null, object routeValues = null, State state = null)
        //{
        //    return Action(requestContext.GetApplicationRequestContext(), actionName, controllerName, routeValues, state);
        //}
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> target, IDictionary<TKey, TValue> dictionary)
        {
            foreach (var entry in dictionary)
            {
                target[entry.Key] = entry.Value;
            }

            return target;
        }
        public static string Action(ApplicationRequestContext requestContext, string actionName = null, string controllerName = null, object routeValues = null)
        {
            if (controllerName == null)
            {
#if NET5
                controllerName = requestContext.ActionContext.RouteData.Values["controller"] as string;
#else
                controllerName = requestContext.RequestContext.RouteData.GetRequiredString("controller");
#endif
            }

            //string query = null;
            RouteValueDictionary routeValueDictionary = null;
            routeValueDictionary = routeValues as RouteValueDictionary ?? new RouteValueDictionary(routeValues);
#if NET5
            string url = new UrlHelper(requestContext.ActionContext).Action(actionName, controllerName, routeValueDictionary);
#else
            string url = UrlHelper.GenerateUrl(null, actionName, controllerName, routeValueDictionary, RouteTable.Routes, requestContext.RequestContext, true);
#endif
            return url;
        }

#if NET452
        public static string Action(RequestContext requestContext, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues = null, State state = null)
        {
            return Action(requestContext.GetApplicationRequestContext(), actionName, controllerName, protocol, hostName, fragment, routeValues, state);
        }

        public static string Action(ApplicationRequestContext requestContext, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues = null, State state = null)
        {
            if (controllerName == null)
            {
                controllerName = requestContext.RequestContext.RouteData.GetRequiredString("controller");
            }

            //string query = null;
            RouteValueDictionary routeValueDictionary = null;
            if (state != null)
            {
                routeValueDictionary = state.ToRouteValues(requestContext);
                if (routeValues != null)
                {
                    routeValueDictionary.Merge(routeValues as IDictionary<string, object> ?? new RouteValueDictionary(routeValues));
                }

                //query = state.ToQueryString(new ValueSerializationSettings { IgnoreNull = false });
            }
            else if (routeValues != null)
            {
                routeValueDictionary = routeValues as RouteValueDictionary ?? new RouteValueDictionary(routeValues);
            }

            string url = UrlHelper.GenerateUrl(null, actionName, controllerName, protocol, hostName, fragment, routeValueDictionary, RouteTable.Routes, requestContext.RequestContext, true);

            //if (string.IsNullOrEmpty(query) == false)
            //{
            //    return query[0] == '?' ? string.Concat(url, query) : string.Concat(url, "?", query);
            //}

            return url;
            //return string.IsNullOrEmpty(request.Url.Query) ? url : string.Concat(url, request.Url.Query);
        }
#endif

#if NET5
        public static RouteValueDictionary GetRouteValuesFromUrl(string url, HttpContext httpContext, string routeName)
        {
            var route = httpContext.GetRouteData().Routers.OfType<Route>().FirstOrDefault(p => p.Name == routeName);         
            if (route == null) throw new ApplicationException($"Could not find route with name {routeName}");
            return UrlUtility.GetRouteValuesFromUrl(url, route);
        }
        public static RouteValueDictionary GetRouteValuesFromUrl(string url, Route route)
        {
            var template = route.ParsedTemplate;
            var matcher = new TemplateMatcher(template, route.Defaults);
            var routeValues = new RouteValueDictionary();
            var localPath = (new Uri(url)).LocalPath;
            if (!matcher.TryMatch(localPath, routeValues)) return routeValues;
                //throw new Exception("Could not identity controller and action");
            return routeValues;
        } 
#endif
        public static string GetBaseUrl(ApplicationRequestContext requestContext)
        {
#if NET5
            // TODO NET5
            return requestContext.HttpRequest.PathBase;
#else
            string baseUrl = "";

            RouteValueDictionary routeValueDictionary = null;
            //if (state != null)
            //{
            //    routeValueDictionary = state.ToRouteValues(requestContext);
            //    baseUrl = UrlHelper.GenerateUrl("", "", "", routeValueDictionary, RouteTable.Routes, requestContext.RequestContext, true);
            //}
            //else
            //{
                baseUrl = Action(requestContext, "", "", new { area = "" });
            //}

            if (!string.IsNullOrEmpty(baseUrl))
            {
                var charIndex = baseUrl.IndexOf("?");
                if (charIndex > -1)
                {
                    baseUrl = baseUrl.Substring(0, charIndex);
                }
            }

            return baseUrl;
#endif
        }
#if NET452
        public static HttpContextBase CreateHttpContext(Uri url, RouteData routeData = null)
        {
            //return new RouteHttpContext(url);

            var context = new HttpContextWrapper(new HttpContext(new RouteHttpWorkerRequest(url)));
            context.Request.RequestContext.RouteData = routeData ?? RouteTable.Routes.GetRouteData(context);
            return context;
        }
#endif

#region Helper Classes

#if NET452
        internal class RouteHttpContext : HttpContextBase
        {
            public RouteHttpContext(Uri uri)
            {
                request = new RouteHttpRequest(uri);
            }

            private RouteHttpRequest request;
            public override HttpRequestBase Request
            {
                get { return request; }
            }

            private IDictionary items = new Dictionary<object, object>();
            public override IDictionary Items
            {
                get { return items; }
            }

            protected class RouteHttpRequest : HttpRequestBase
            {
                public RouteHttpRequest(Uri uri)
                {
                    this.uri = uri;
                }

                private Uri uri;

                public override string Path
                {
                    get { return uri.AbsolutePath; }
                }

                private NameValueCollection headers = new NameValueCollection();
                public override NameValueCollection Headers
                {
                    get { return headers; }
                }

                public override NameValueCollection QueryString
                {
                    get { return uri.GetQueryCollection(); }
                }

                public override string ApplicationPath
                {
                    get { return HttpRuntime.AppDomainAppVirtualPath; }
                }

                public override string AppRelativeCurrentExecutionFilePath
                {
                    get { return string.Concat("~", uri.LocalPath); }
                }

                public override string HttpMethod
                {
                    get { return "GET"; }
                }

                public override string RawUrl
                {
                    get { return uri.PathAndQuery; }
                }

                public override Uri Url
                {
                    get { return uri; }
                }

                public override Uri UrlReferrer
                {
                    get { return null; }
                }

                public override string PathInfo { get { return string.Empty; } }
            }
        }

        public class RouteHttpWorkerRequest : HttpWorkerRequest
        {
            public RouteHttpWorkerRequest(Uri uri)
            {
                this.uri = uri;
            }

            private Uri uri;

            public override void EndOfRequest() { }

            public override void FlushResponse(bool finalFlush) { }

            public override string GetHttpVerbName()
            {
                return "GET";
            }

            public override string GetHttpVersion()
            {
                return "HTTP/1.0";
            }

            public override string GetLocalAddress()
            {
                return uri.Host;
            }

            public override int GetLocalPort()
            {
                return uri.Port;
            }

            public override string GetQueryString()
            {
                return string.IsNullOrEmpty(uri.Query) == false ? uri.Query.Substring(1) : null;
            }

            public override string GetRawUrl()
            {
                return uri.PathAndQuery;
            }

            public override string GetRemoteAddress()
            {
                return "127.0.0.1";
            }

            public override int GetRemotePort()
            {
                return 0;
            }

            public override string GetUriPath()
            {
                return uri.AbsolutePath; // LocalPath?
            }

            public override void SendKnownResponseHeader(int index, string value) { }

            public override void SendResponseFromFile(IntPtr handle, long offset, long length) { }

            public override void SendResponseFromFile(string filename, long offset, long length) { }

            public override void SendResponseFromMemory(byte[] data, int length) { }

            public override void SendStatus(int statusCode, string statusDescription) { }

            public override void SendUnknownResponseHeader(string name, string value) { }
        }

#endif

#endregion
    }

    public static class Utility
    {
#if NET452
        public static string MapPath(string virtualPath, HttpContext httpContext)
        {
            return Utility.MapPath(virtualPath, httpContext != null && httpContext.Handler != null ? httpContext.Request : null);
        }

        public static string MapPath(string virtualPath, HttpContextBase httpContext)
        {
            return Utility.MapPath(virtualPath, httpContext != null && httpContext.Handler != null ? httpContext.Request : null);
        }

        public static string MapPath(string virtualPath, HttpRequest request)
        {
            if (request != null) return request.MapPath(virtualPath);
            if (virtualPath == null) return null;
            return System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
        }

        public static string MapPath(string virtualPath, HttpRequestBase request)
        {
            if (request != null) return request.MapPath(virtualPath);
            if (virtualPath == null) return null;
            return System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
        }

        public static string MapPath(string virtualPath)
        {
            if (virtualPath == null) return null;
            return System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
        }
#endif
        /// <summary>
        /// Creates a set of strings from a separated string. Difference from CreateCodeSet is that the set is optimized for read depending on size and might be readonly.
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
     

        /// <summary>
        /// Creates a set of strings from a separated string.
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
       

        public static string CodeSetToString(IEnumerable<string> codeSet, string separator = ",")
        {
            return codeSet != null ? string.Join(separator, codeSet) : null;
        }

        public static IEnumerable<int> ParseIntArray(string array, params string[] separators)
        {
            return array != null ? array.Split(separators != null && separators.Length > 0 ? separators : new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s.Trim())) : Enumerable.Empty<int>();
        }

        public static IEnumerable<string> ParseStringArray(string array, params string[] separators)
        {
            return array != null ? array.Split(separators != null && separators.Length > 0 ? separators : new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()) : Enumerable.Empty<string>();
        }

        public static void SqlDataReader(/*AssertSite site,*/string connectionString, string sql, Action<System.Data.SqlClient.SqlDataReader> action)
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();

                var command = new System.Data.SqlClient.SqlCommand(sql, connection);

                var reader = command.ExecuteReader();

                action(reader);
            }
        }

      

        public static string JsonEncode(object value, bool ignoreNullValues = true, params string[] ignoredProperties)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = ignoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include };
            if (ignoredProperties != null && ignoredProperties.Length > 0)
            {
                settings.ContractResolver = new IgnoredPropertiesContractResolver { IgnoredProperties = ignoredProperties };
            }

            return JsonConvert.SerializeObject(value, settings);
        }

        private class IgnoredPropertiesContractResolver : DefaultContractResolver
        {
            public IEnumerable<string> IgnoredProperties { get; set; }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                return base.CreateProperties(type, memberSerialization).Where(p => IgnoredProperties.Contains(p.PropertyName) == false).ToList();
            }
        }


#region Base 64 operations
        public static string FromBase64(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
#endregion

#region Mime Type

        /* Old 32-bit only definition
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(
             System.UInt32 pBC,
             [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
             [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
             System.UInt32 cbSize,
             [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
             System.UInt32 dwMimeFlags,
             out System.UInt32 ppwzMimeOut,
             System.UInt32 dwReserved);
        */

        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        static extern int FindMimeFromData(IntPtr pBC,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] 
            byte[] pBuffer,
            int cbSize,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
            int dwMimeFlags,
            out IntPtr ppwzMimeOut,
            int dwReserved);

        private static System.Collections.Generic.Dictionary<string, string> MIME_Mappings = null;

        public static string CheckType(string filePath)
        {
            byte[] buffer = null;

            if (File.Exists(filePath) == true)
            {
                try
                {
                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        int readLength = Math.Min(1024, (int)fileStream.Length);
                        buffer = new byte[readLength];
                        fileStream.Read(buffer, 0, readLength);
                    }
                }
                catch
                {
                    // Do nothing
                }
            }

            string extension = Path.GetExtension(filePath).TrimStart('.');
            return CheckType(buffer, extension);
        }

        public static string CheckType(byte[] buffer, string extension)
        {
            string mimeTypeString = null;

            if (MIME_Mappings == null)
            {
                MIME_Mappings = new System.Collections.Generic.Dictionary<string, string>();

                MIME_Mappings.Add("pdf", "application/pdf");
                MIME_Mappings.Add("xps", "application/vnd.ms-xpsdocument");
                MIME_Mappings.Add("doc", "application/msword");
                MIME_Mappings.Add("dot", "application/msword");

                MIME_Mappings.Add("mht", "message/rfc822");
                MIME_Mappings.Add("mhtml", "message/rfc822");

                MIME_Mappings.Add("oxt", "application/vnd.openofficeorg.extension");
                MIME_Mappings.Add("pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
                MIME_Mappings.Add("sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide");
                MIME_Mappings.Add("ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
                MIME_Mappings.Add("potx", "application/vnd.openxmlformats-officedocument.presentationml.template");
                MIME_Mappings.Add("xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                MIME_Mappings.Add("xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template");
                MIME_Mappings.Add("docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                MIME_Mappings.Add("dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template");

                MIME_Mappings.Add("xla", "application/vnd.ms-excel");
                MIME_Mappings.Add("xlc", "application/vnd.ms-excel");
                MIME_Mappings.Add("xlm", "application/vnd.ms-excel");
                MIME_Mappings.Add("xls", "application/vnd.ms-excel");
                MIME_Mappings.Add("xlt", "application/vnd.ms-excel");
                MIME_Mappings.Add("xlw", "application/vnd.ms-excel");
                MIME_Mappings.Add("msg", "application/vnd.ms-outlook");
                MIME_Mappings.Add("sst", "application/vnd.ms-pkicertstore");
                MIME_Mappings.Add("cat", "application/vnd.ms-pkiseccat");
                MIME_Mappings.Add("stl", "application/vnd.ms-pkistl");
                MIME_Mappings.Add("pot", "application/vnd.ms-powerpoint");
                MIME_Mappings.Add("pps", "application/vnd.ms-powerpoint");
                MIME_Mappings.Add("ppt", "application/vnd.ms-powerpoint");
                MIME_Mappings.Add("mpp", "application/vnd.ms-project");
                MIME_Mappings.Add("wcm", "application/vnd.ms-works");
                MIME_Mappings.Add("wdb", "application/vnd.ms-works");
                MIME_Mappings.Add("wks", "application/vnd.ms-works");
                MIME_Mappings.Add("wps", "application/vnd.ms-works");

                MIME_Mappings.Add("ai", "application/postscript");
                MIME_Mappings.Add("eps", "application/postscript");
                MIME_Mappings.Add("ps", "application/postscript");
                MIME_Mappings.Add("rtf", "application/rtf");

                MIME_Mappings.Add("png", "image/png");
                MIME_Mappings.Add("bmp", "image/bmp");
                MIME_Mappings.Add("cod", "image/cis-cod");
                MIME_Mappings.Add("gif", "image/gif");
                MIME_Mappings.Add("ief", "image/ief");
                MIME_Mappings.Add("jpe", "image/jpeg");
                MIME_Mappings.Add("jpeg", "image/jpeg");
                MIME_Mappings.Add("jpg", "image/jpeg");
                MIME_Mappings.Add("jfif", "image/pipeg");
                MIME_Mappings.Add("svg", "image/svg+xml");
                MIME_Mappings.Add("tif", "image/tiff");
                MIME_Mappings.Add("tiff", "image/tiff");
                MIME_Mappings.Add("ras", "image/x-cmu-raster");
                MIME_Mappings.Add("cmx", "image/x-cmx ");
                MIME_Mappings.Add("ico", "image/x-icon");
                MIME_Mappings.Add("pnm", "image/x-portable-anymap");
                MIME_Mappings.Add("pbm", "image/x-portable-bitmap");
                MIME_Mappings.Add("pgm", "image/x-portable-graymap");
                MIME_Mappings.Add("ppm ", "image/x-portable-pixmap");
                MIME_Mappings.Add("rgb", "image/x-rgb");
                MIME_Mappings.Add("xbm", "image/x-xbitmap");
                MIME_Mappings.Add("xpm", "image/x-xpixmap");
                MIME_Mappings.Add("xwd", "image/x-xwindowdump");

                MIME_Mappings.Add("wmv", "application/x-mplayer2");
            }

            string proposedMimeType = (string.IsNullOrEmpty(extension) == false && MIME_Mappings.ContainsKey(extension) == true) ? MIME_Mappings[extension] : null;
            if (proposedMimeType != null)
            {
                return proposedMimeType;
            }

            if (buffer != null)
            {
                try
                {
                    IntPtr mimeType;
                    int returnValue = FindMimeFromData(new IntPtr(0), null, buffer, (int)buffer.Length, null, 0, out mimeType, 0);
                    mimeTypeString = Marshal.PtrToStringUni(mimeType);
                }
                catch
                {
                    // Do nothing
                }
            }

            if (string.IsNullOrEmpty(mimeTypeString) == true || mimeTypeString == "application/octet-stream")
            {
                if (proposedMimeType != null)
                {
                    mimeTypeString = proposedMimeType;
                }

                if (string.IsNullOrEmpty(mimeTypeString) == true)
                {
                    mimeTypeString = "text/plain";
                }
            }

            return mimeTypeString;
        }

        public static string GetImageMimeType(ImageFormat format)
        {
            if (format.Guid == ImageFormat.Jpeg.Guid)
            {
                return "image/jpeg";
            }
            else if (format.Guid == ImageFormat.Png.Guid)
            {
                return "image/png";
            }
            else if (format.Guid == ImageFormat.Gif.Guid)
            {
                return "image/gif";
            }
            else if (format.Guid == ImageFormat.Bmp.Guid)
            {
                return "image/bmp";
            }
            else if (format.Guid == ImageFormat.Tiff.Guid)
            {
                return "image/tiff";
            }

            return "image";
        }

        public static string GetImageMimeType(Image image)
        {
            if (image == null)
            {
                return "image";
            }

            return GetImageMimeType(image.RawFormat);
        }

#endregion

        public static T ParseEnum<T>(string value, bool ignoreCase = true, T fallbackValue = default(T))
            where T : struct
        {
            T result;
            if (Enum.TryParse<T>(value, ignoreCase, out result) == true)
            {
                return result;
            }

            if (value != null)
            {
                string cleaned = value.Replace("-", "");
                if (Enum.TryParse<T>(cleaned, ignoreCase, out result) == true)
                {
                    return result;
                }
            }

            return fallbackValue;
        }

        public static Nullable<T> ParseNullableEnum<T>(string value, bool ignoreCase = true)
            where T : struct
        {
            T result;
            if (Enum.TryParse<T>(value, ignoreCase, out result) == true)
            {
                return result;
            }

            if (value != null)
            {
                string cleaned = value.Replace("-", "");
                if (Enum.TryParse<T>(cleaned, ignoreCase, out result) == true)
                {
                    return result;
                }
            }

            return null;
        }

#if NET452
        public class MetaTag
        {
            public MetaTag(string str)
            {
                Raw = str;
            }

            public MetaTag(string name, string content)
            {
                this.name = name;
                this.content = content;
            }

            private string name;
            public string Name { get { return name ?? (name = GetAttr(Raw, "name")); } }

            private string property;
            public string Property { get { return property ?? (property = GetAttr(Raw, "property")); } }

            private string itemProp;
            public string ItemProp { get { return itemProp ?? (itemProp = GetAttr(Raw, "itemprop")); } }

            private string content;
            public string Content { get { return content ?? (content = GetAttr(Raw, "content")); } }

            public string Raw { get; private set; }

            public string GetAttr(string name)
            {
                return GetAttr(Raw, name);
            }

            private string GetAttr(string str, string name)
            {
                if (str == null || string.IsNullOrWhiteSpace(name) == true) return string.Empty;
                int index = str.IndexOf(name + "=", StringComparison.OrdinalIgnoreCase);
                if (index > 0)
                {
                    int findIndex = index + name.Length + 1;
                    if (findIndex < str.Length)
                    {
                        int startIndex = str.IndexOf('"', findIndex);
                        if (startIndex > 0 && startIndex < str.Length)
                        {
                            int endIndex = str.IndexOf('"', startIndex + 1);
                            if (endIndex > 0)
                            {
                                return str.Substring(startIndex + 1, endIndex - startIndex - 1);
                            }
                        }
                    }
                }
                return string.Empty;
            }
        }

        public static List<MetaTag> GetHtmlMetaTags(string url)
        {
            var metaTags = new List<MetaTag>();
            HttpWebRequest request = null;
            var headerBuilder = new StringBuilder();
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var receiveStream = response.GetResponseStream();
                        using (var readStream = string.IsNullOrWhiteSpace(response.CharacterSet) ? new StreamReader(receiveStream) : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet)))
                        {
                            string line = null;
                            string partialMetaTag = null;
                            int lineCount = 0;
                            int index = 0;
                            do
                            {
                                //line = await readStream.ReadLineAsync();
                                line = readStream.ReadLine();
                                index = 0;
                                bool continueSearch = true;
                                do
                                {
                                    if (partialMetaTag == null)
                                    {
                                        int metaTagIndex = line.IndexOf("<meta ", index, StringComparison.OrdinalIgnoreCase);
                                        if (metaTagIndex >= 0)
                                        {
                                            int endTagIndex = line.IndexOf('>', metaTagIndex + 6);
                                            if (endTagIndex >= 0)
                                            {
                                                string metaTag = line.Substring(metaTagIndex, endTagIndex - metaTagIndex + 1);
                                                metaTags.Add(new MetaTag(metaTag));
                                                index = endTagIndex + 1;
                                            }
                                            else
                                            {
                                                partialMetaTag = string.Concat(partialMetaTag, line.Substring(metaTagIndex));
                                                continueSearch = false;
                                            }
                                        }
                                        else
                                        {
                                            continueSearch = false;
                                        }
                                    }
                                    else
                                    {
                                        int endTagIndex = line.IndexOf('>', index);
                                        if (endTagIndex >= 0)
                                        {
                                            string metaTag = string.Concat(partialMetaTag, line.Substring(0, endTagIndex + 1));
                                            metaTags.Add(new MetaTag(metaTag));
                                            partialMetaTag = null;
                                            index = endTagIndex + 1;
                                            continueSearch = true;
                                        }
                                        else
                                        {
                                            partialMetaTag = string.Concat(partialMetaTag, line);
                                        }
                                    }
                                } while (continueSearch == true);

                                int titleIndex = line.IndexOf("<title>", StringComparison.OrdinalIgnoreCase);
                                if (titleIndex >= 0)
                                {
                                    int endTitleIndex = line.IndexOf("</title>", titleIndex + 7, StringComparison.OrdinalIgnoreCase);
                                    if (endTitleIndex > 0)
                                    {
                                        string title = line.Substring(titleIndex + 7, endTitleIndex - (titleIndex + 7));
                                        metaTags.Add(new MetaTag("titletag", title));
                                    }
                                    else
                                    {
                                        string title = line.Substring(titleIndex + 7);
                                        metaTags.Add(new MetaTag("titletag", title));
                                    }
                                }

                                headerBuilder.AppendLine(line);
                            } while (++lineCount <= 200 && line.Contains("</head>", StringComparison.OrdinalIgnoreCase) == false && line.Contains("<body", StringComparison.OrdinalIgnoreCase) == false);
                        }
                    }
                }
            }
            catch
            {
                return metaTags;
            }

            return metaTags;
        }
#endif
    }
    public class UrlParametersBuilder
    {
        protected StringBuilder builder;

        public UrlParametersBuilder Append(string parameter)
        {
            if (parameter != null)
            {
                if (builder == null)
                {
                    builder = new StringBuilder();
                    if (parameter.StartsWith("&") == false)
                    {
                        builder.Append(parameter);
                    }
                    else
                    {
                        builder.Append(parameter.Substring(1));
                    }
                }
                else
                {
                    if (parameter.StartsWith("&") == false)
                    {
                        builder.Append("&");
                    }
                    builder.Append(parameter);
                }
            }
            return this;
        }

        public UrlParametersBuilder Append(string name, string value)
        {
            if (value != null)
            {
                if (builder == null)
                {
                    builder = new StringBuilder();
                }
                else
                {
                    builder.Append("&");
                }
                builder.Append(name);
                builder.Append("=");
                builder.Append(value);
            }
            return this;
        }

        public UrlParametersBuilder Append(string name, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                if (value != null)
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                    }
                    else
                    {
                        builder.Append("&");
                    }
                    builder.Append(name);
                    builder.Append("=");
                    builder.Append(value);
                }
            }
            return this;
        }

        public UrlParametersBuilder Append<T>(string name, IEnumerable<T> values, Func<T, string> selector)
        {
            foreach (var value in values)
            {
                if (value != null)
                {
                    string stringValue = selector(value);
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                    }
                    else
                    {
                        builder.Append("&");
                    }
                    builder.Append(name);
                    builder.Append("=");
                    builder.Append(stringValue);
                }
            }
            return this;
        }

        public override string ToString()
        {
            return builder != null ? builder.ToString() : string.Empty;
        }

        public string AppendTo(string baseUrl)
        {
            if (builder == null) return baseUrl;
            int i = baseUrl.IndexOf('?');
            if (i == -1)
            {
                return baseUrl + "?" + builder.ToString();
            }
            else
            {
                return baseUrl.EndsWith("&") == true ? baseUrl + builder.ToString() : baseUrl + "&" + builder.ToString();
            }
        }
    }
    public static class Extensions
    {

        public static Stream GetContentAsStream(this FileResult result)
        {
            var fileStreamResult = result as FileStreamResult;
            if (fileStreamResult != null)
            {
                return fileStreamResult.FileStream;
            }

            var fileContentResult = result as FileContentResult;
            if (fileContentResult != null)
            {
                return new MemoryStream(fileContentResult.FileContents);
            }
#if NET452
            var filePathResult = result as FilePathResult;
            if (filePathResult != null)
            {
                return File.OpenRead(filePathResult.FileName);
            }
#endif
            return null;
        }

        public static async Task<byte[]> GetContentAsBufferAsync(this FileResult result)
        {
            var fileStreamResult = result as FileStreamResult;
            if (fileStreamResult != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    long currentPos = fileStreamResult.FileStream.Position;
                    fileStreamResult.FileStream.Seek(0, SeekOrigin.Begin);
                    await fileStreamResult.FileStream.CopyToAsync(memoryStream);
                    fileStreamResult.FileStream.Seek(currentPos, SeekOrigin.Begin);

                    return memoryStream.ToArray();
                }
            }

            var fileContentResult = result as FileContentResult;
            if (fileContentResult != null)
            {
                return fileContentResult.FileContents;
            }
#if NET452
            var filePathResult = result as FilePathResult;
            if (filePathResult != null)
            {
                return File.ReadAllBytes(filePathResult.FileName);
            }
#endif
            return null;
        }


        public static IDictionary<string, object> ToDictionary(this NameValueCollection collection)
        {
            if (collection == null)
            {
                return null;
            }

            IDictionary<string, object> dictionary = new Dictionary<string, object>(collection.Count);
#if NET5
            foreach (string name in collection.AllKeys)
            {
                dictionary[name] = collection[name];
            }
#else
            collection.CopyTo(dictionary);
#endif
            return dictionary;
        }

        public static void RemoveRange(this NameValueCollection collection, IEnumerable<string> keys)
        {
            foreach (string key in keys)
            {
                collection.Remove(key);
            }
        }

#if NET5
        public static IDictionary<string, object> GetQuery(this HttpRequest request)
        {
            var queryString = request.Query;
#else
        public static IDictionary<string, object> GetQuery(this HttpRequestBase request)
        {
            var queryString = request.QueryString;
            if (queryString.Count == 1 && queryString.Keys[0] == null)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(queryString[0]);
            }
            else
#endif
            {
                var dictionary = new Dictionary<string, object>(queryString.Count + 1);
                foreach (string key in queryString.Keys)
                {
                    if (key == null)
                    {
                        string values = queryString[null];
                        if (values != null)
                        {
                            var keys = values.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            keys.ForEach(k => dictionary[k] = null);
                        }
                    }
                    else if (key == string.Empty)
                    {
                        string values = queryString[string.Empty];
                        if (values != null)
                        {
                            var keys = values.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            keys.ForEach(k => dictionary[k] = string.Empty);
                        }
                    }
                    else
                    {
                        dictionary[key] = queryString[key];
                    }
                }

                return dictionary;
            }
        }


        public static T GetValue<T>(this ModelBindingContext bindingContext, string key)
        {
            var result = bindingContext.ValueProvider.GetValue(key);
            return result != null ? (T)result.ConvertTo(typeof(T)) : default(T);

        }

        /// <summary>
        /// Replace all invalid file name characters by an other letter
        /// </summary>
        /// <param name="source">Filename in string</param>
        /// <param name="replaceWith">Default to an underscore '_' character</param>
        /// <returns>Filename with replaced letters</returns>
        public static string GetSafeFilename(this string source, string replaceWith = "_")
        {
            return string.Join(replaceWith, source.Split(Path.GetInvalidFileNameChars()));
        }

        /// <summary>
        /// Replace html characters: \n \t \r
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ReplaceHtmlSpecialCharacters(this string source, string replacement = " ")
        {
            var newstring = System.Text.RegularExpressions.Regex.Replace(source, @"\t|\n|\r", replacement);
            return newstring;
        }


        public static IOrderedQueryable<TSource> OrderByDirection<TSource, TKey>(this IQueryable<TSource> source, System.Linq.Expressions.Expression<Func<TSource, TKey>> keySelector, SortDirection direction)
        {
            return direction == SortDirection.Ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
        }

        public static IOrderedEnumerable<TSource> OrderByDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection direction)
        {
            return direction == SortDirection.Ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
        }

        public static IOrderedEnumerable<TSource> OrderByDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection direction, IComparer<TKey> comparer)
        {
            return direction == SortDirection.Ascending ? source.OrderBy(keySelector, comparer) : source.OrderByDescending(keySelector, comparer);
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection direction)
        {
            return direction == SortDirection.Ascending ? source.ThenBy(keySelector) : source.ThenByDescending(keySelector);
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection direction, IComparer<TKey> comparer)
        {
            return direction == SortDirection.Ascending ? source.ThenBy(keySelector, comparer) : source.ThenByDescending(keySelector, comparer);
        }

    }

}