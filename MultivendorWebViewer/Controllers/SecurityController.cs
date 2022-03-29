using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Manager;
using MultivendorWebViewer.ViewModels;

namespace MultivendorWebViewer.Controllers
{
    public class SecurityController : BaseController
    {
        // GET: Category
        [HttpGet]
        public  ActionResult Index(int? id)
        {
            return View();
        }
        [HttpGet]
        public ActionResult NotFound()
        {
            return View("_NotFound");
        }


    }
}