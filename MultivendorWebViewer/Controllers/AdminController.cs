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
        [HttpGet]
        public ActionResult GetAllUserList(DataViewRequest request,string searchText)
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

    }
}