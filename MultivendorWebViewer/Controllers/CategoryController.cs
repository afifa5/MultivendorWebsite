using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Manager;


namespace MultivendorWebViewer.Controllers
{
    public class CategoryController : BaseController
    {
        // GET: Category
        [HttpGet]
        public override ActionResult Index()
        {
            int StartCategory = 4;
            var categoryViewModel = new CategoryManager().GetCategory(StartCategory, null);
           // var categoryViewModel = ApplicationRequestContext._CategoryManager.GetCategory(StartCategory, ApplicationRequestContext);
            return View(categoryViewModel);
        }
    }
}