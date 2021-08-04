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
        public ApplicationRequestContext ApplicationRequestContext { get { return new ApplicationRequestContext(/*URLUtility.CreateHttpContext().Request.RequestContext*/);} }
        // GET: Base
        public virtual ActionResult Index()
        {            
            return View();
        }

    }
}