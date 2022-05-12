using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using System.Collections.Concurrent;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Server.Models;
#if NET5
using Microsoft.AspNetCore.Http;
#endif

namespace MultivendorWebViewer.Common
{
    [Serializable]
    public class SessionData
    {
        public SessionData()
        {
            orders = new ConcurrentDictionary<string, Order>(StringComparer.OrdinalIgnoreCase);
        }
       
        [NonSerialized]
        private ConcurrentDictionary<string, Order> orders;
        [XmlIgnore]
        public ConcurrentDictionary<string, Order> Orders { get { return orders; } set { orders = value; } }
        [NonSerialized]
        private User user;
        [XmlIgnore]
        [Obsolete("Use ApplicationRequestContext.User")]
        public User User
        {
            get
            {
                return user;
            }
            set { user = value; }
        }
        public static SessionData GetSessionData(HttpContextBase context)
        {
            return SessionItemHelper.GetItem(context, "MultivendorWebSessionData", () => new SessionData());
        }


    }
}
