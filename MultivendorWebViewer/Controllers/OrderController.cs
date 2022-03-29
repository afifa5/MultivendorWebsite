using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.ViewModels;

namespace MultivendorWebViewer.Controllers
{
    public class OrderController : BaseController
    {
        [HttpGet]
        public override ActionResult Index()
        {
            var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
            if (order != null && order.OrderLines.Any()) {
                var orderViewModel = new OrderViewModel(order, ApplicationRequestContext);
                return View(orderViewModel);
            }
            return View(new OrderViewModel(null,ApplicationRequestContext));

        }
        [HttpPost]
        public ActionResult OrderItem(int productId, decimal quantity = 1) {
            //Check availability
            var availability = ApplicationRequestContext.ProductManager.GetpriceByproductId(productId);
            var availableQuantity = availability.FirstOrDefault().Quantity;
            if (availableQuantity > quantity) {
                var orderLine = new OrderLine()
                {
                    ProductId = productId,
                    Quantity = quantity
                };
                var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
                if (order == null)
                {
                    order = new Order();
                }
                if ((order.OrderLines.Any())) {
                    var existProduct = order.OrderLines.Where(p => p.ProductId == productId).FirstOrDefault();
                    if (existProduct != null) {
                        if(availableQuantity > existProduct.Quantity + quantity)
                            existProduct.Quantity += quantity;
                    }
                    else
                        order.OrderLines.Add(orderLine);
                }
                else
                order.OrderLines.Add(orderLine);
                ApplicationRequestContext.OrderManager.SetCurrentOrder(ApplicationRequestContext,order);
                return Json(new { status = true });
            }
            return Json(new { status = false });
        }
        [HttpPost]
        public ActionResult GetTotalQuantity()
        {
            //Check availability
            var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
            if (order != null && order.OrderLines.Any()) {
                var totalCount = order.OrderLines.Sum(p => p.Quantity);
                var countString = Math.Round(totalCount, 0).ToString(new CultureInfo(ApplicationRequestContext.SelectedCulture));
                return Json(new { status = true,totalCount = countString });
            }
            return Json(new { status = false });
        }

    }

}