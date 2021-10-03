#if NET5
using Microsoft.AspNetCore.Http;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.Common
{
    public static class RequestItemHelper
    {
//#if NET452
        public static T GetItem<T>(HttpContextBase context, object key, Func<T> setter = null)
        {
            if (context == null) return default(T);

            object value = context.Items[key];//.ApplicationInstance.Context.Items[key];
            if (value is T)
            {
                return (T)value;
            }
            else if (setter != null)
            {
                var addValue = setter();
                if (addValue != null)
                {
                    context.Items[key] = addValue;
                    return addValue;
                }
            }

            return default(T);
        }
        //#endif
        public static T GetItem<T>(HttpContext context, object key, Func<T> setter = null)
        {
            if (context == null) return default(T);

            object value = context.Items[key];
            if (value is T)
            {
                return (T)value;
            }
            else if (setter != null)
            {
                var addValue = setter();
                if (addValue != null)
                {
                    context.Items[key] = addValue;
                    return addValue;
                }
            }

            return default(T);
        }

        public static void SetItem(HttpContext context, object key, object item)
        {
            if (context != null)
            {
                context.Items[key] = item;
            }
        }

        //#if NET452
        public static void SetItem(HttpContextBase context, object key, object item)
        {
            if (context != null)
            {
                context.Items[key] = item;
            }
        }

        public static void RemoveItem(HttpContextBase context, object key)
        {
            if (context != null)
            {
                context.Items.Remove(key);
                //context.ApplicationInstance.Context.Items.Remove(key);
            }
        }
//#endif

        public static void RemoveItem(HttpContext context, object key)
        {
            if (context != null)
            {
                context.Items.Remove(key);
            }
        }

    }
}