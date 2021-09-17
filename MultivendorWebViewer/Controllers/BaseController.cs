using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;

namespace MultivendorWebViewer.Controllers
{
    public class BaseController : Controller
    {
        public ApplicationRequestContext ApplicationRequestContext { get { return ApplicationRequestContext.GetContext(HttpContext); } }
        // GET: Base
        public virtual ActionResult Index()
        {            
            return View();
        }

    }
}