using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Server.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            if (order != null && !string.IsNullOrEmpty(Model.DeliveryMethodName)) {
                SelectedDeliveryOption = Model.DeliveryMethodName;
            }
        }
        private Order Model { get; set; }
        public string OrderReference { get; set; }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public List<OrderLineViewModel>  OrderLines { get { return GetOrderLines(); } }
        public CustomerViewModel Customer { get { return GetOrderCustomer(); } }
        public string CreatedDate => Model.CreatedDate != null ? Model.CreatedDate.Value.ToString("yyyy-MM-dd", new CultureInfo(ApplicationRequestContext.SelectedCulture)) : string.Empty;
        public string ModificationDate => Model.ModificationDate != null ? Model.ModificationDate.Value.ToString("yyyy-MM-dd", new CultureInfo(ApplicationRequestContext.SelectedCulture)) : string.Empty;
        public string ModifiedBy => Model.ModifiedBy;
        public int CustomerId => Model.CustomerId;
        public string SelectedDeliveryOption { get; set; }
        private List<OrderLineViewModel> GetOrderLines()
        {
            return Model!=null && Model.OrderLines.Any() ? Model.OrderLines.Select(p => new OrderLineViewModel(p, ApplicationRequestContext)).ToList() : new List<OrderLineViewModel>();
        }
        private CustomerViewModel GetOrderCustomer()
        {
            return Model != null && Model.Customer!=null ?new CustomerViewModel(Model.Customer, ApplicationRequestContext) : new CustomerViewModel(new Customer(),ApplicationRequestContext);
        }
    }
}