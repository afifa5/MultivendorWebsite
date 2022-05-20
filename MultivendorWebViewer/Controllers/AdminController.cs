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
    [PermissionAuthorize(AuthorizePermissions.Administration, AuthorizePermissions.Vendor, AlwaysRequire = true)]
    public class AdminController : BaseController
    {
        // GET: Category
        [HttpGet]
        public override ActionResult Index()
        {
          return View("Index");
        }
    }
}