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
            int StartCategory = 1;
            var category= ApplicationRequestContext.CategoryManager.GetCategory(StartCategory, ApplicationRequestContext);
            var categoryViewModel = new CategoryViewModel(category, ApplicationRequestContext);
           // var categoryViewModel = ApplicationRequestContext._CategoryManager.GetCategory(StartCategory, ApplicationRequestContext);
            return View(categoryViewModel);
        }
    }
}