using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Manager;
using MultivendorWebViewer.ViewModels;

namespace MultivendorWebViewer.Controllers
{
    public class CategoryController : BaseController
    {
        // GET: Category
        [HttpGet]
        public override ActionResult Index()
        {
            int? StartCategory = ApplicationRequestContext.Configuration!=null ? ApplicationRequestContext.Configuration.SiteProfile.StartCatalogueId : null;
            var category= ApplicationRequestContext.CategoryManager.GetCategory(StartCategory.Value, ApplicationRequestContext);
            var categoryViewModel = new CategoryViewModel(category, ApplicationRequestContext);
            return View(categoryViewModel);
        }
        [HttpGet]
        public  ActionResult Node(int nodeId)
        {
            int? StartCategory = ApplicationRequestContext.Configuration != null ? ApplicationRequestContext.Configuration.SiteProfile.StartCatalogueId : null;
            var category = ApplicationRequestContext.CategoryManager.GetCategory(StartCategory.Value, ApplicationRequestContext);
            var categoryViewModel = new CategoryViewModel(category, ApplicationRequestContext);
            return View("Index",categoryViewModel);
        }
    }
}