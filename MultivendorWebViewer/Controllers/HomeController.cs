using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
namespace MultivendorWebViewer.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public override ActionResult Index()
        {

            int? StartCategory = ApplicationRequestContext.Configuration != null ? ApplicationRequestContext.Configuration.SiteProfile.StartCatalogueId : null;
            if (StartCategory == null)
                return RedirectToAction("NotFound", "Security");
            else {
                return RedirectToAction("Index", "Category", new { id = StartCategory });
            }
          
        }
        [HttpGet]
        public ActionResult GetLanguagePopUp()
        {
            List<CultureInfo> cultureInfo = new List<CultureInfo>();
            var availableLanguage = ApplicationRequestContext.Configuration.SiteProfile.AvailableLanguage.Split(',').ToList();
            if (availableLanguage.Count() > 0)
            {
                availableLanguage.ForEach(i => {
                    cultureInfo.Add(new CultureInfo(i));
                });
            }
            return PartialView("_AvailableLanguage", cultureInfo);
        }
        [HttpGet]
        public ActionResult GetUserMenuPopUp()
        {

            return PartialView("UserMenu");
        }
        [HttpPost]
        public ActionResult SetLanguage(string languageCode)
        {
            var userSetting = ApplicationRequestContext.UserSettingProvider.Load(ApplicationRequestContext) ?? new UserSettings();
            userSetting.UICulture = languageCode;
            ApplicationRequestContext.UserSettingProvider.Store(ApplicationRequestContext, userSetting);
            return Json(new { status = true });
        }
    }
}