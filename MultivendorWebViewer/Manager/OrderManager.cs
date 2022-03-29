using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultivendorWebViewer.ViewModels;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Common;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Threading.Tasks;
using System.IO;

namespace MultivendorWebViewer.Manager
{
    public class OrderManager
    {

        public virtual Order GetCurrentOrder(ApplicationRequestContext applicationRequestContext)
        {
 
            if (applicationRequestContext.SessionData == null) return null;
            var sessionOrders = applicationRequestContext.SessionData.Orders;
            var siteOrder = sessionOrders.GetOrAdd("MultivendorWeb", order =>
            {
                return new Order();
            });

            return siteOrder;
        }
        public virtual void SetCurrentOrder(ApplicationRequestContext applicationRequestContext, Order order)
        {
            var sessionOrders = applicationRequestContext.SessionData.Orders;
            if (order != null)
            {
                sessionOrders["MultivendorWeb"] = order;
            }
            else
            {
                ((System.Collections.IDictionary)sessionOrders).Remove("MultivendorWeb");
            }
        }

    }
}