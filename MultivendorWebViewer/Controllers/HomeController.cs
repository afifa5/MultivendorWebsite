﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
namespace MultivendorWebViewer.Controllers
{
    public class HomeController : BaseController
    {
        public override ActionResult Index()
        {
            int? StartCategory = ApplicationRequestContext.Configuration != null ? ApplicationRequestContext.Configuration.SiteProfile.StartCatalogueId : null;

            return RedirectToAction("Index", "Category", new { id = StartCategory });
        }

    }
}