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
            //options.PageSizes = new PaginationPageSize[] { 25, 50, 100 };
            //options.DefaultPageSize = 100;
            //options.DefaultSortSelector = "Name";
            options.Tools = tools;
            options.DisplayFilter = false;
            options.DownloadToolAvailable = false;
            options.DisplayItemCountDescription = true;
           
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