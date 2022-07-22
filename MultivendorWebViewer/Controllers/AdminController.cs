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
    public class AdminController : BaseController
    {
        // GET: Category
        [HttpGet]
        public  ActionResult AdminView()
        {
          return View("Index");
        }

        [PermissionAuthorize(AuthorizePermissions.Administration, AlwaysRequire = true)]
        [HttpGet]
        public ActionResult UserList()
        {
            return View("UserListView");
        }

        //[HttpPost]
        //public ActionResult AdminView()
        //{
        //    return View("Index");
        //}

        [PermissionAuthorize(AuthorizePermissions.Administration, AlwaysRequire = true)]
        [HttpGet]
        public ActionResult GetAllUserList(DataViewRequest request)
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
        #region DatabaseMigrations
        [HttpGet]
        public ActionResult DatabaseMigrationView()
        {
            ServerDatabaseMigrationDetail migrationdetail = new ServerDatabaseMigrationDetail();

            var dbconnection = ServerModelDatabaseContextManager.Default.GetConnection();
            var connectionstring = dbconnection.ConnectionString;

            ServerDatabaseMigrator serverdatabasemigratior = new ServerDatabaseMigrator(connectionstring);

            var configuration = serverdatabasemigratior.Configuration;

            var migrator = new DbMigrator(configuration);

            var appliedmigration = migrator.GetDatabaseMigrations();

            DefaultInitialCreate initialcreate = new DefaultInitialCreate();

            var default_initialcreate = initialcreate.MigrationId.Substring(2);

            var latestinitialcreate = default_initialcreate.Substring(0, default_initialcreate.Length - 1);

            var validdatabasewithmigration = appliedmigration.Contains(latestinitialcreate);

            migrationdetail.EnableMigarationForDatabase = validdatabasewithmigration == true ? true : false;

            foreach (var item in appliedmigration.Reverse())
            {
                migrationdetail.AppliedMigrationNames.Add(item);
            }

            var pendingmigraiton = migrator.GetPendingMigrations();

            foreach (var item in pendingmigraiton)
            {
                migrationdetail.PendingMigraitonNames.Add(item);
                migrationdetail.PendingMigrationDetails.Add(PendingMigrationInformation(item));
            }

            var comptiable = serverdatabasemigratior.IsCurrentServerDatabaseCompatibleWithModel(dbconnection);

            migrationdetail.CurrentServerDatabaseCompatibleWithCurrentModel = comptiable;

            return View("_DatabaseMigrationView", migrationdetail);
        }

        [HttpPost]
        public ActionResult UpdateDatabaseMigration()
        {
            var dbconnection = ServerModelDatabaseContextManager.Default.GetConnection();
            var connectionstring = dbconnection.ConnectionString;

            ServerDatabaseMigrator serverdatabasemigratior = new ServerDatabaseMigrator(connectionstring);

            var configuration = serverdatabasemigratior.Configuration;

            var migrator = new DbMigrator(configuration);

            var pendingmigraiton = migrator.GetPendingMigrations();

            string runningmigration = string.Empty;

            try
            {
                foreach (var item in pendingmigraiton)
                {
                    runningmigration = item;
                    migrator.Update(item);
                }
                return Json(new { result = "true", message = string.Format("Migration done.") });
            }
            catch (Exception e)
            {
                return Json(new { result = "false", message = string.Format("Migration failed at {0}. <br/> Exception Message : {1} <br/> InnerException : {2}", runningmigration, e.Message, e.InnerException) });
            }

        }
        public PendingMigratioDetail PendingMigrationInformation(string migrationname)
        {
            PendingMigratioDetail detail = new PendingMigratioDetail();

            switch (migrationname)
            {
                case "202007211138512_mig_refactoring2_column_tables_32":

                    List<string> information = new List<string>();

                    information.Add("Users attached to non-existing price group will have their price group set to NULL.");
                    information.Add("Organizations attached to non-existing price group will have their price group set to NULL.");

                    detail.PendingMigrationInformation = information;
                    detail.PendingMigrationName = migrationname;

                    break;

            }


            return detail;

        }
        public bool IsServerDatabaseValid(string serverDatabaseName)
        {
            try
            {
                using (var context = new ServerModelContext(ServerModelDatabaseContextManager.Default.GetConnectionString()))
                {
                    var user = context.Users.FirstOrDefault();
                    if (user == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

    }
}