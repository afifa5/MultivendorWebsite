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
        [HttpGet]
        public  ActionResult OrderCostView()
        {
            var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
            if (order != null && order.OrderLines.Any())
            {
                var orderViewModel = new OrderViewModel(order, ApplicationRequestContext);
                return PartialView("OrderCartCostViewContainer",orderViewModel);
            }
            return PartialView("OrderCartCostViewContainer",new OrderViewModel(null, ApplicationRequestContext));

        }
        [HttpGet]
        public ActionResult OrderCartView()
        {
            var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
            if (order != null && order.OrderLines.Any())
            {
                var orderViewModel = new OrderViewModel(order, ApplicationRequestContext);
                return PartialView("OrderCart", orderViewModel);
            }
            return PartialView("OrderCart", new OrderViewModel(null, ApplicationRequestContext));

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
                if ((order.OrderLines.Any()))
                {
                    var existProduct = order.OrderLines.Where(p => p.ProductId == productId).FirstOrDefault();
                    if (quantity >= 0)
                    {
                        if (existProduct != null)
                        {
                            //if(availableQuantity > existProduct.Quantity + quantity)
                            existProduct.Quantity = quantity;
                        }
                        else
                            order.OrderLines.Add(orderLine);
                    }

                }
                else
                {
                    order.OrderLines.Add(orderLine);
                }
                if (quantity <= 0 && order.OrderLines!=null && order.OrderLines.Any()) {
                    order.OrderLines.RemoveAll(p => p.ProductId == productId);
                }
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
        
        [HttpPost]
        public ActionResult DeleteAllOrder()
        {
            //Check availability
             ApplicationRequestContext.OrderManager.SetCurrentOrder(ApplicationRequestContext,null);
            return Json(new { status = true });
        }

    }

}