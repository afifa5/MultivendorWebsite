using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.ViewModels
{
    public class OrderLineViewModel
    {
        public OrderLineViewModel(OrderLine orderLine, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = orderLine;
        }
        private OrderLine Model { get; set; }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public decimal Quantity { get { return Model.Quantity; } }
        public int? UserId => Model.UserId;
        public decimal PriceInclTax => Model.PriceInclTax;
        public decimal Discount => Model.Discount;
        public decimal SubTotal => Model.SubTotal;
        public ProductViewModel Product { get { return GetProductViewModel(); } }
        public PriceAvailailityViewModel PriceAvailability { get { return GetPriceAvailability(); } }
        private ProductViewModel GetProductViewModel() {
            var productId = Model.ProductId;
            var product = ApplicationRequestContext.ProductManager.GetProductById(productId);
            return new ProductViewModel(product, ApplicationRequestContext);
        }
        private PriceAvailailityViewModel GetPriceAvailability() {
            var prices = ApplicationRequestContext.ProductManager.GetpriceByproductId(Model.ProductId);
            if (prices != null)
            {
                return prices.Select(p => new PriceAvailailityViewModel(p, ApplicationRequestContext)).FirstOrDefault();
            }
            else return null;
        }
    }
}