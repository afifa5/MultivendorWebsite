using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.ViewModels;
using System;
using System.Diagnostics;
using System.Web.Helpers;
using Claims = System.Security.Claims;
using MultivendorWebViewer.Manager;
using System.Web;
using MultivendorWebViewer.Server.Models;

namespace MultivendorWebViewer
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
           

            if (ServerConfigurationProvider.Default.Configuration.ServerDatabase != null && ServerConfigurationProvider.Default.Configuration.ServerDatabase.Enabled == true)
            {

                app.CreatePerOwinContext(() => ServerModelDatabaseContextManager.Default.Context());
                app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieSecure = CookieSecureOption.SameAsRequest,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                
            //LoginPath = new PathString("/Account/Login"),
            //ReturnUrlParameter = "returnUrl",
            Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>
                    (
                        validateInterval: TimeSpan.FromMinutes(240),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)
                    ),
                    OnResponseSignIn = context =>
                    {
                        context.Properties.AllowRefresh = true;
                        context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(300);
                    }
            }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ApplicationCookie);

            // Declare claims name to use as unique identifier
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Claims.ClaimTypes.NameIdentifier;

        }

       
    }

    public class TraceLogger 
    {
        public void WriteInformation(string message)
        {
            Trace.WriteLine("SAML INFORMATION: " + message);
        }

        public void WriteError(string message, Exception ex)
        {
            Trace.WriteLine("SAML ERROR: " + message + Text(ex));
        }

        public void WriteVerbose(string message)
        {
            Trace.WriteLine("SAML VERBOSE: " + message);
        }

        public string Text(Exception ex)
        {
            if (ex == null) return String.Empty;

            string result = String.Empty; ;
            while (ex != null)
            {
                result += " " + ex.Message;
                ex = ex.InnerException;
            }
            return result.Trim();
        }
    }
}