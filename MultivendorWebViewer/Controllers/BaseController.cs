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
        //public ApplicationRequestContext ApplicationRequestContext { get { return new ApplicationRequestContext(HttpContext.cu);} }
        // GET: Base
        public virtual ActionResult Index()
        {            
            return View();
        }

    }
}