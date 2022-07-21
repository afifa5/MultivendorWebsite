using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Components;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Helpers;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Server.Models;
using MultivendorWebViewer.ViewModels;

namespace MultivendorWebViewer.Controllers
{
    public class OrderController : BaseController
    {
        [HttpGet]
        public override ActionResult Index()
        {
            return View();

        }
        [HttpGet]
        public  ActionResult OrderCostView()
        {
            var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
            if (order != null && order.OrderLines != null && order.OrderLines.Any())
            {
                var orderViewModel = new OrderViewModel(order, ApplicationRequestContext);
                return PartialView("OrderCartCostViewContainer",orderViewModel);
            }
            return PartialView("OrderCartCostViewContainer",new OrderViewModel(null, ApplicationRequestContext));

        }
        [HttpGet]
        public ActionResult OrderPaymentView()
        {
            var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
            if (order == null || order.OrderLines == null || !order.OrderLines.Any()) {
                return new EmptyResult();
            }
            return PartialView("PaymentView");

        }
        [HttpPost]
        public ActionResult SaveAddress(string selectedDeliveryMethod, Customer information)
        {
            var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
            if (order != null && order.OrderLines != null && order.OrderLines.Any())
            {
                order.DeliveryMethodName = selectedDeliveryMethod;
                if (order.Customer != null) {
                    if (!string.IsNullOrEmpty(information.FirstName)) order.Customer.FirstName = information.FirstName;
                    if (!string.IsNullOrEmpty(information.LastName)) order.Customer.LastName = information.LastName;
                    if (!string.IsNullOrEmpty(information.Email)) order.Customer.Email = information.Email;
                    if (!string.IsNullOrEmpty(information.PhoneNumber)) order.Customer.PhoneNumber = information.PhoneNumber;
                    if (!string.IsNullOrEmpty(information.Address)) order.Customer.Address = information.Address;
                    if (!string.IsNullOrEmpty(information.PostCode)) order.Customer.PostCode = information.PostCode;
                    if (!string.IsNullOrEmpty(information.City)) order.Customer.City = information.City;
                    if (!string.IsNullOrEmpty(information.CareOf)) order.Customer.CareOf = information.CareOf;
                    if (!string.IsNullOrEmpty(information.Country)) order.Customer.Country = information.Country;
                }
                else
                order.Customer = information;
            }
            ApplicationRequestContext.OrderManager.SetCurrentOrder(ApplicationRequestContext, order);
            return Json(new { status = false });

        }
        [HttpPost]
        public ActionResult PlaceOrder()
        {
          var orderReference =   ApplicationRequestContext.OrderManager.PlaceOrder(ApplicationRequestContext);
            return Json(new { status = true,orderReference = orderReference });

        }
        [HttpGet]
        public ActionResult OrderCartView()
        {
            var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
            if (order != null && order.OrderLines != null && order.OrderLines.Any())
            {
                var orderViewModel = new OrderViewModel(order, ApplicationRequestContext);
                return PartialView("OrderCart", orderViewModel);
            }
            return PartialView("OrderCart", new OrderViewModel(null, ApplicationRequestContext));

        }
        [HttpGet]
        public ActionResult SuccessOrderView(string orderReference)
        {
            var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
            ApplicationRequestContext.OrderManager.SetCurrentOrder(ApplicationRequestContext, null);

            if (order != null && order.OrderLines != null && order.OrderLines.Any())
            {
                var orderViewModel = new OrderViewModel(order, ApplicationRequestContext);
                orderViewModel.OrderReference = orderReference;
                return View("PlaceOrder", orderViewModel);
            }
            return View("PlaceOrder", new OrderViewModel(null, ApplicationRequestContext));

        }
        public ActionResult OrderCustomerView()
        {
            var order = ApplicationRequestContext.OrderManager.GetCurrentOrder(ApplicationRequestContext);
            if (order.Customer == null && ApplicationRequestContext.User != null && ApplicationRequestContext.User.CustomerId.HasValue) {
                order.Customer = ApplicationRequestContext.UserDBManager.GetCustomerById(ApplicationRequestContext.User.CustomerId.Value);
                ApplicationRequestContext.OrderManager.SetCurrentOrder(ApplicationRequestContext, order);
            }
            if (order != null && order.OrderLines!=null && order.OrderLines.Any())
            {
                var orderViewModel = new OrderViewModel(order, ApplicationRequestContext);
                return PartialView("CustomerView", orderViewModel);
            }
            return PartialView("CustomerView", new OrderViewModel(null, ApplicationRequestContext));

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
                if (order.OrderLines != null && (order.OrderLines.Any()))
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
            if (order != null && order.OrderLines !=null&& order.OrderLines.Any()) {
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
       
        [HttpGet]
        public ActionResult OrderList(string orderReference = null)
        {
            ViewBag.DataListUrl = UrlUtility.Action(ApplicationRequestContext, "GetOrderList", "Order", new { orderReference = orderReference });
            return View("~/Views/Admin/OrderListView.cshtml");
        }
        [HttpGet]
        public ActionResult GetOrderList(DataViewRequest request, string orderReference = null)
        {
            var orderList = new List<OrderViewModel>();
              if (!string.IsNullOrEmpty(orderReference)) {
            var order =ApplicationRequestContext.OrderManager.GetOrderByReference(orderReference);
                var orderViewModel = new OrderViewModel(order, ApplicationRequestContext) { SelectedDeliveryOption = order.DeliveryMethodName };
                orderList.Add(orderViewModel);
            }
            else if (ApplicationRequestContext.User != null) {
                var allOrder = ApplicationRequestContext.OrderManager.GetCustomerOrders(ApplicationRequestContext.User.Id);
                if (allOrder != null && allOrder.Any())
                {
                    orderList = allOrder.Select(p => new OrderViewModel(p, ApplicationRequestContext) { SelectedDeliveryOption = p.DeliveryMethodName }).OrderByDescending(p => p.OrderReference).ToList();
                }
            }
           
             

            var tools = new List<ToolBarItem>();



            //tools.Insert(0, new ToolBarItem
            //{
            //    Label = "New address",
            //    Icon = ApplicationRequestContext.GetIcon(Icons.Plus),
            //    Id = "CreateCompanyAddress",
            //    ClassNames = "create-new-company-address",
            //    TextAlignment = ToolBarAlignment.Left,
            //    Location = DataViewOptions.ToolLocations.AfterSortSelectors,
            //});


            var options = request.State.CreateOptions();
            options.PageSizes = new PaginationPageSize[] { 50, 100 };
            options.DefaultPageSize = 50;
            //options.DefaultSortSelector = "OrderReference";
            //options.DefaultFilterSelection = new DataViewFilterSelection() { Properties = new List<DataViewFilterSelectionProperty>() { new DataViewFilterSelectionProperty() { Id = "UserName", Property= "UserName" } } };
            options.Tools = tools;
            options.DisplayFilter = true;
            options.DownloadToolAvailable = false;
            options.DisplayItemCountDescription = true;
            options.SortSelectors = new DataViewSelectorCollection<DataViewSortSelector>
                          {
                              new DataViewSortSelector { Id = "OrderReference", Label = CustomStrings.Reference, Sort = new Sort("OrderReference") },
                              new DataViewSortSelector { Id = "CreatedDate", Label = CustomStrings.CreatedDate, Sort = new Sort("CreatedDate") },
                              new DataViewSortSelector { Id = "ModificationDate", Label = CustomStrings.ModifiedDate, Sort = new Sort("ModificationDate") },
                              new DataViewSortSelector { Id = "ModifiedBy", Label = CustomStrings.ModifiedBy, Sort = new Sort("ModifiedBy") },
                            };

            options.FilterSelectors = new DataViewSelectorCollection<DataViewFilterSelector>
                          {
                              new DataViewFilterSelector { Id = "OrderReference", Label = CustomStrings.Reference, Property = "OrderReference" },
                              new DataViewFilterSelector { Id = "CreatedDate", Label = CustomStrings.CreatedDate, Property = "CreatedDate" },
                              new DataViewFilterSelector { Id = "ModificationDate", Label = CustomStrings.ModifiedDate, Property = "ModificationDate" },
                              new DataViewFilterSelector { Id = "ModifiedBy", Label = CustomStrings.ModifiedBy, Property = "ModifiedBy" },

                          };
            //ViewBag.CustomerRequest = "true";

            var result = new DataViewResult(request, ApplicationRequestContext)
            {

                DataViewOptions = options,
                Content = state =>
                {
                    var dataViewModel = new DataViewString<OrderViewModel>(ApplicationRequestContext, orderList, state: state);
                    return new DataViewContentResult("~/Views/Admin/_OrdersTable.cshtml", dataViewModel);
                }
            };
            return result;
        }


        [HttpGet]
        public ActionResult OrderItemView(string orderReference)
        {
            var order = ApplicationRequestContext.OrderManager.GetOrderByReference(orderReference);
            if (order != null && order.OrderLines != null && order.OrderLines.Any())
            {
                var orderViewModel = new OrderViewModel(order, ApplicationRequestContext);
                return PartialView("~/Views/Admin/OrderDetail.cshtml", orderViewModel);
            }
            return PartialView("~/Views/Admin/OrderDetail.cshtml", new OrderViewModel(null, ApplicationRequestContext));

        }

    }

}