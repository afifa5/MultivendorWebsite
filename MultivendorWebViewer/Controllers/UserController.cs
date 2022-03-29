using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.ViewModels;

namespace MultivendorWebViewer.Controllers
{
    public class UserController : BaseController
    {
        [HttpGet]
        public ActionResult Login() {
            return View("Login");
        }
    }
}