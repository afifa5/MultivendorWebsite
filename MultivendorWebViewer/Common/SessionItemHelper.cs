using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.Common
{
    public static class SessionItemHelper
    {
        private static object sessionDataLock = new object();

        public static T GetItem<T>(HttpContextBase context, string name, Func<T> setter = null)
        {
            if (context != null)
            {
                var session = context.Session;
                if (session != null)
                {
                    object value = context.Session[name];
                    if (value is T)
                    {
                        return (T)value;
                    }
                    else if (setter != null)
                    {
                        if (value != null)
                        {
                            // Conflict in usage of keys
                        }

                        lock (sessionDataLock)
                        {
                            value = context.Session[name];
                            if (value is T)
                            {
                                return (T)value;
                            }
                            else
                            {
                                var addValue = setter();
                                if (addValue != null)
                                {
                                    try
                                    {
                                        context.Session[name] = addValue;
                                        return addValue;
                                    }
                                    catch
                                    {
                                        return (T)context.Session[name];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return default(T);
        }

        public static void RemoveItem(HttpContextBase context, string name)
        {
            if (context != null)
            {
                var session = context.Session;
                if (session != null)
                {
                    lock (sessionDataLock)
                    {
                        context.Session.Remove(name);
                    }
                }
            }
        }
    }
}