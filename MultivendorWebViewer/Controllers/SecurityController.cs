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

        [HttpGet]
        public ActionResult GetLanguagePopUp()
        {
            List<CultureInfo> cultureInfo = new List<CultureInfo>();
            var availableLanguage = ApplicationRequestContext.Configuration.SiteProfile.AvailableLanguage.Split(',').ToList();
            if (availableLanguage.Count() > 0) {
                availableLanguage.ForEach(i => {
                    cultureInfo.Add(new CultureInfo(i));
                });
            }
            return PartialView("_AvailableLanguage", cultureInfo);
        }
        [HttpPost]
        public ActionResult SetLanguage(string languageCode)
        {
            var userSetting = ApplicationRequestContext.UserSettingProvider.Load(ApplicationRequestContext);
            userSetting.UICulture = languageCode;
            ApplicationRequestContext.UserSettingProvider.Store(ApplicationRequestContext, userSetting);
            return Json(new { status = true });
        }
    }
}