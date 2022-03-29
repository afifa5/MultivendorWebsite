using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.ViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel(Order order, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = order;
            if (order!=null && !string.IsNullOrEmpty(order.OrderReference)) {
                OrderReference = order.OrderReference;
            }
        }
        private Order Model { get; set; }
        public string OrderReference { get; set; }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public List<OrderLineViewModel>  OrderLines { get { return GetOrderLines(); } }
        private List<OrderLineViewModel> GetOrderLines()
        {
            return Model!=null && Model.OrderLines.Any() ? Model.OrderLines.Select(p => new OrderLineViewModel(p, ApplicationRequestContext)).ToList() : new List<OrderLineViewModel>();
        }
    }
}