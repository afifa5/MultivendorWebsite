#if NET5
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
#endif

using MultivendorWebViewer.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Routing;
#if NET452
using System.Web.Routing;
#endif

namespace MultivendorWebViewer.Components
{
    public class State
    {
      

        private CultureInfo uiCulture;
        public CultureInfo UICulture
        {
            get { return uiCulture; }
            set
            {
                uiCulture = value;
                cacheRouteValues = null;
            }
        }

       

        //private IFilterSelection filter;
        //public IFilterSelection Filter
        //{
        //    get { return filter; }
        //    set
        //    {
        //        filter = value;
        //        cacheRouteValues = null;
        //    }
        //}



        public virtual State Copy()
        {
            return (State)MemberwiseClone();
        }

        public override int GetHashCode()
        {
            // Make state immutable?

            // Cache!
            return HashHelper.GetHashCode<CultureInfo>( new[] { UICulture });
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj) == true)
            {
                return true;
            }

            var other = obj as State;

            return other != null &&
                object.Equals(other.UICulture, UICulture) == true;
              
        }

        private RouteValueDictionary cacheRouteValues = null;

        public RouteValueDictionary ToRouteValues(ApplicationRequestContext requestContext, ValueSerializationSettings settings = null, bool includeQuery = true, object additionalValues = null)
        {
            RouteValueDictionary values = new RouteValueDictionary();
            //if (settings == null && includeQuery == true)
            //{
            //    if (cacheRouteValues == null)
            //    {
            //        cacheRouteValues = StateRouteProvider.Default.GetRouteValues(requestContext, this, null, true);
            //    }

            //    values = new RouteValueDictionary(cacheRouteValues);

            //    if (additionalValues != null)
            //    {
            //        values.Merge(additionalValues as IDictionary<string, object> ?? new RouteValueDictionary(additionalValues));
            //    }
            //}
            //else
            //{
            //    values = StateRouteProvider.Default.GetRouteValues(requestContext, this, settings, includeQuery);

            //    if (additionalValues != null)
            //    {
            //        values.Merge(additionalValues as IDictionary<string, object> ?? new RouteValueDictionary(additionalValues));
            //    }
            //}

            return values;
        }

        public RouteValueDictionary ToRouteValues(ApplicationRequestContext requestContext, object additionalValues)
        {
            return ToRouteValues(requestContext, null, true, additionalValues);
        }

        //public IDictionary<string, object> ToQuery(ApplicationRequestContext requestContext, ValueSerializationSettings settings = null)
        //{
        //    return StateRouteProvider.Default.GetQueryValues(requestContext, this, settings);
        //}

        //public string ToQueryString(ApplicationRequestContext requestContext, ValueSerializationSettings settings = null)
        //{
        //    return StateRouteProvider.Default.ToQueryString(requestContext, this, settings);
        //}

        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(ToRouteValues(null, includeQuery: true), new Newtonsoft.Json.Converters.KeyValuePairConverter());
        }

        public override string ToString()
        {
            return ToJson();
        }

        public bool IsEmpty = false;

        static public State Empty { get { return new State { IsEmpty = true }; } }
    }

    public class StateProvider : SingletonBase<StateProvider>
    {
#if NET5
        public virtual State GetState(HttpContext context)
        {
            throw new NotImplementedException("NET5:StateProvider.GetState not implemented");
            //return RequestItemHelper.GetItem<State>(context, "State", () => StateRouteProvider.Default.GetState(context.Request.GetApplicationRequestContext()));
        }
#else
        //public virtual State GetState(HttpContextBase context)
        //{
        //    return RequestItemHelper.GetItem<State>(context, "State", () => StateRouteProvider.Default.GetState(context.Request.RequestContext.GetApplicationRequestContext()));
        //}
#endif

#if NET5
        public virtual void SetState(HttpContext context, State state)
#else
        public virtual void SetState(HttpContextBase context, State state)
#endif
        {
            RequestItemHelper.SetItem(context, "State", state);
        }
    }
}