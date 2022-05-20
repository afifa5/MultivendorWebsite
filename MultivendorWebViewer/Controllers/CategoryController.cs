using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Manager;
using MultivendorWebViewer.ViewModels;
using MultivendorWebViewer.Common;

namespace MultivendorWebViewer.Controllers
{
    //[PermissionAuthorize(AuthorizePermissions.Administration, AuthorizePermissions.Vendor, AlwaysRequire = true)]
    public class CategoryController : BaseController
    {
        // GET: Category
        [HttpGet]
        public  ActionResult Index(int? id)
        {
            var category= ApplicationRequestContext.CategoryManager.GetCategory(id.Value, ApplicationRequestContext);
            if (category == null) {
                return RedirectToAction("NotFound", "Security");
            }
            var categoryViewModel = new CategoryViewModel(category, ApplicationRequestContext);
            return View(categoryViewModel);
        }

        [HttpGet]
        public  ActionResult Node(int nodeId)
        {
            var node = ApplicationRequestContext.CategoryManager.GetNodeById(nodeId);
            var nodeViewModel = new NodeViewModel(node, ApplicationRequestContext);
            return View("NodeView", nodeViewModel);
        }
    }
}