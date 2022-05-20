using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Manager;
using MultivendorWebViewer.Server.Models;
using MultivendorWebViewer.ViewModels;

namespace MultivendorWebViewer.Controllers
{
    public class UserController : BaseController
    {
        //private ApplicationUserManager userManager;
        //public ApplicationUserManager UserManager
        //{
        //    get { return userManager ?? new ApplicationUserManager(ApplicationRequestContext, new ApplicationUserStore(ApplicationRequestContext)); }
        //    private set { userManager = value; }
        //}
        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private ApplicationSignInManager signInManager;
        public ApplicationSignInManager SignInManager
        {
            get { return signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { signInManager = value; }
        }
        [HttpGet]
        public ActionResult Login() {
            
            return View("Login");
        }
        [HttpGet]
        public ActionResult Unauthenticated()
        {

            return View("Login");
        }
        [HttpGet]
        public ActionResult Unauthorized()
        {
            return View("Login");
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            try
            {
                // Get current user
                var user = ApplicationRequestContext.User;

                // Get navigation adress for the page displayed when sign out is selected
                string referrer = null;
                if (ApplicationRequestContext.HttpRequest.UrlReferrer != null)
                {
                    referrer = ApplicationRequestContext.HttpRequest.UrlReferrer.AbsolutePath;
                }

                // Remove users order from session
                //ApplicationRequestContext.OrderManager.RemoveCurrentOrderFromSession(ApplicationRequestContext);

                // Check if a SAML2 logout is required
               
                // Do a OWin sign out
                var allAuthenticationTypes = AuthenticationManager.GetAuthenticationTypes().Select(i => i.AuthenticationType).ToArray();
                AuthenticationManager.SignOut(allAuthenticationTypes);

                // And do a forms sign out to
                FormsAuthentication.SignOut();

                // Release all session data for user
                Session.Abandon();

                // clear authenticated cookie by setting expire to last year
                HttpCookie authenticatedCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                authenticatedCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(authenticatedCookie);

                // clear session cookie 
                SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
                HttpCookie sessionCookie = new HttpCookie(sessionStateSection.CookieName, "");
                sessionCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(sessionCookie);

                // Drop cache data
                //ApplicationRequestContext.UserManager.InvalidateUserCache(user);
                int? StartCategory = ApplicationRequestContext.Configuration != null ? ApplicationRequestContext.Configuration.SiteProfile.StartCatalogueId : null;
                if (StartCategory == null)
                    return RedirectToAction("NotFound", "Security");
                else
                {
                    return RedirectToAction("Index", "Category", new { id = StartCategory });
                }
            }
            catch (Exception exception)
            {
                throw;
            }
           
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                var usermanager =ApplicationRequestContext.UserManager;

                if (usermanager != null)
                {
                    ApplicationUser user = null;
                    try
                    {
                            user = await usermanager.FindAsync(model.UserName, model.Password);
                    }
                    catch (AuthenticationException aex)
                    {
                        ModelState.AddModelError("", aex.Message);
                        return View(model);
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e.Message);
                        return View(model);
                    }


                    if (user != null)
                    {
                        await SignInAsync(user, model.RememberMe);

                        if (string.IsNullOrEmpty(returnUrl) == false)
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else {
                        ModelState.AddModelError("", "Login failed for the user");
                    }

                }
                else
                {
                    if (ServerConfigurationProvider.Default.Configuration.ServerDatabase != null && ServerConfigurationProvider.Default.Configuration.ServerDatabase.Enabled == false)
                    {
                    }

                    ModelState.AddModelError("", "User login not permitted at this moment. Please contact administrator.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            try
            {
                var allAuthenticationTypes = AuthenticationManager.GetAuthenticationTypes().Select(i => i.AuthenticationType).ToArray();
                AuthenticationManager.SignOut(allAuthenticationTypes);


                System.Security.Claims.ClaimsIdentity[] identities = new System.Security.Claims.ClaimsIdentity[] { await user.GenerateUserIdentityAsync(ApplicationRequestContext.UserManager) };
                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identities);
            }
            catch (Exception exception)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isvalid = true;
                var isEmailAddressValid = EmailValidation.Default.IsEmailVaild(model.UserName);
                if (!isEmailAddressValid) {
                    isvalid = false;
                    ModelState.AddModelError("", "Email address format not valid");
                }
                if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword) || model.Password != model.ConfirmPassword) {
                    isvalid = false;
                    ModelState.AddModelError("", "Invalid password");

                }
                if (!isvalid) { 
                    return View(model);
                }
                var customerRole = AuthorizePermissions.Customer;
                model.Email = model.UserName;
                model.UserRole = customerRole;
                var usermanager = ApplicationRequestContext.UserManager;
                User newUser = new User() { UserName= model.UserName};
                var user = new ApplicationUser(newUser) { UserName = model.Email, Email = model.Email };

                
                var result = usermanager.RegisterUser(model);
                if (result != null)
                {
                    await SignInAsync(user, true);

                    //string code = await usermanager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "User",
                    //   new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await usermanager.SendEmailAsync(user.Id,
                    //   "Confirm your account", "Please confirm your account by clicking <a href=\""
                    //   + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                //AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }

}