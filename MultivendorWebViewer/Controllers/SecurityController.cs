using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Components;
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

        [AllowAnonymous]
        public ActionResult SessionTimeout(string iconImage = null, string header = null, string message = null)
        {
            if (iconImage == null && header == null && message == null)
            {
                return new EmptyResult();
            }
            string heading = header != null ? ApplicationRequestContext.GetApplicationTextTranslation(header) : string.Empty;
            string Messaging = message != null ? ApplicationRequestContext.GetApplicationTextTranslation(message) : string.Empty;
            string iconImages = iconImage ?? Icons.Info;
            string[] modalvalue = { iconImages, heading, Messaging };
            return PartialView("_SessionTimeout", modalvalue);
        }
    }
}