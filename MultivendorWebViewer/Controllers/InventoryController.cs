using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Manager;
using MultivendorWebViewer.ViewModels;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Helpers;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Components;
using MultivendorWebViewer.DbMigrations;
using System.Data.Entity.Migrations;

namespace MultivendorWebViewer.Controllers
{
    [PermissionAuthorize(AuthorizePermissions.Administration, AuthorizePermissions.Vendor, AlwaysRequire = true)]
    public class InventoryController : BaseController
    {
        // GET: Category
        [HttpGet]
        public  ActionResult InventoryView()
        {
          return View("Index");
        }

        [PermissionAuthorize(AuthorizePermissions.Administration, AlwaysRequire = true)]
        #region Catalogue
        [HttpGet]
        public ActionResult CategoryView()
        {
            return View("CatalogueListView");
        }

        
        [PermissionAuthorize(AuthorizePermissions.Administration, AlwaysRequire = true)]
        [HttpGet]
        public ActionResult GetCategoryList(DataViewRequest request)
        {
            var allUser = ApplicationRequestContext.UserDBManager.GetAllUsers().SelectMany(p=> p);
            var allUsersViewModels = new List<UserViewModel>();
            if (allUser != null && allUser.Any())
            {
                allUsersViewModels = allUser.Select(p => new UserViewModel(p, ApplicationRequestContext)).ToList();
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
            options.PageSizes = new PaginationPageSize[] { 50,100 };
            options.DefaultPageSize = 50;
            options.DefaultSortSelector = "UserName";
            //options.DefaultFilterSelection = new DataViewFilterSelection() { Properties = new List<DataViewFilterSelectionProperty>() { new DataViewFilterSelectionProperty() { Id = "UserName", Property= "UserName" } } };
            options.Tools = tools;
            options.DisplayFilter = true;
            options.DownloadToolAvailable = false;
            options.DisplayItemCountDescription = true;
            options.SortSelectors = new DataViewSelectorCollection<DataViewSortSelector>
                          {
                              new DataViewSortSelector { Id = "UserName", Label = CustomStrings.UserName, Sort = new Sort("UserName") },
                              //new DataViewSortSelector { Id = "UserRole", Label = CustomStrings.UserRole, Sort = new Sort("UserRole") },
                              new DataViewSortSelector { Id = "Customer.FirstName", Label = CustomStrings.FirstName, Sort = new Sort("Customer.FirstName") },
                              new DataViewSortSelector { Id = "Customer.PhoneNumber", Label = CustomStrings.Phone, Sort = new Sort("Customer.PhoneNumber") },
                              new DataViewSortSelector { Id = "CompanyName", Label = CustomStrings.UserCompany, Sort = new Sort("CompanyName") },
                            };

            options.FilterSelectors = new DataViewSelectorCollection<DataViewFilterSelector>
                          {
                              new DataViewFilterSelector { Id = "UserName", Label = CustomStrings.UserName, Property = "UserName" },
                              //new DataViewFilterSelector { Id = "UserRole", Label = CustomStrings.UserRole, Property = "UserRole" },
                              new DataViewFilterSelector { Id = "Customer.FirstName", Label = CustomStrings.FirstName, Property = "Customer.FirstName" },
                              new DataViewFilterSelector { Id = "Customer.PhoneNumber", Label = CustomStrings.Phone, Property = "Customer.PhoneNumber" },
                              new DataViewFilterSelector { Id = "CompanyName", Label = CustomStrings.UserCompany, Property = "CompanyName" },

                          };
            var result = new DataViewResult(request, ApplicationRequestContext)
            {

                DataViewOptions = options,
                Content = state =>
                {
                    var dataViewModel = new DataViewString<UserViewModel>(ApplicationRequestContext, allUsersViewModels, state: state);
                    return new DataViewContentResult("_UserTable", dataViewModel);
                }
            };
            return result;
        }
        #endregion
        [HttpGet]
        public ActionResult OrderList()
        {
            //ViewBag.DataListUrl = UrlUtility.Action(ApplicationRequestContext, "GetOrderList", "Admin");

            return View("OrderListView");
        }
        [HttpGet]
        public ActionResult GetOrderList(DataViewRequest request)
        {
            var orderList = new List<OrderViewModel>();
            if (ApplicationRequestContext.User.UserRole == AuthorizePermissions.Administration)
            {
                var allOrder = ApplicationRequestContext.OrderManager.GetAllOrders();
                if (allOrder != null && allOrder.Any())
                {
                    orderList = allOrder.Select(p => new OrderViewModel(p, ApplicationRequestContext) { SelectedDeliveryOption = p.DeliveryMethodName }).OrderByDescending(p => p.OrderReference).ToList();
                }
            }
            else if (ApplicationRequestContext.User.UserRole == AuthorizePermissions.Vendor)
            {
                var allOrder = ApplicationRequestContext.OrderManager.GetUserOrders(ApplicationRequestContext.User.Id);
                if (allOrder != null && allOrder.Any())
                {
                    orderList = allOrder.Select(p => new OrderViewModel(p, ApplicationRequestContext) { SelectedDeliveryOption = p.DeliveryMethodName }).OrderByDescending(p => p.OrderReference).ToList();
                }
            }
            //else {
            //    var allOrder = ApplicationRequestContext.OrderManager.GetCustomerOrders(ApplicationRequestContext.User.Id);
            //    if (allOrder != null && allOrder.Any())
            //    {
            //        orderList = allOrder.Select(p => new OrderViewModel(p, ApplicationRequestContext) { SelectedDeliveryOption = p.DeliveryMethodName }).OrderByDescending(p => p.OrderReference).ToList();
            //    }
            //}
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
            var result = new DataViewResult(request, ApplicationRequestContext)
            {

                DataViewOptions = options,
                Content = state =>
                {
                    var dataViewModel = new DataViewString<OrderViewModel>(ApplicationRequestContext, orderList, state: state);
                    return new DataViewContentResult("_OrdersTable", dataViewModel);
                }
            };
            return result;
        }


    }
}