using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Configuration;
//using Signifikant.Assert.Model.Static.Server.Repository;
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

                app.CreatePerOwinContext(() => new ServerModelContext(ServerModelDatabaseContextManager.Default.GetConnectionString()));
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
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ApplicationCookie);

            // Declare claims name to use as unique identifier
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Claims.ClaimTypes.NameIdentifier;

            //app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();
            //}
        }

        //private static Saml2AuthenticationOptions CreateSaml2Options()
        //{
        //    var spOptions = CreateSPOptions();
        //    var Saml2Options = new Saml2AuthenticationOptions(false)
        //    {
        //        SPOptions = spOptions
        //    };

        //    var idp = new IdentityProvider(new EntityId("https://sts.windows.net/14a6c996-717b-4c62-887f-d147e19e80ae/"), spOptions)
        //    {
        //        AllowUnsolicitedAuthnResponse = false,
        //        Binding = Saml2BindingType.HttpRedirect,
        //        MetadataLocation = "https://login.microsoftonline.com/14a6c996-717b-4c62-887f-d147e19e80ae/federationmetadata/2007-06/federationmetadata.xml?appid=81c7aa90-2c0d-4034-a66f-4d47f4dc1b43",
        //    };

        //    // TODO använd kund-cerifikat
        //    //idp.SigningKeys.AddConfiguredKey(new X509Certificate2(HostingEnvironment.MapPath("~/App_Data/stubidp.sustainsys.com.cer")));

        //    Saml2Options.IdentityProviders.Add(idp);

        //    // It's enough to just create the federation and associate it
        //    // with the options. The federation will load the metadata and
        //    // update the options with any identity providers found.
        //    //new Federation("https://login.microsoftonline.com/14a6c996-717b-4c62-887f-d147e19e80ae/federationmetadata/2007-06/federationmetadata.xml?appid=81c7aa90-2c0d-4034-a66f-4d47f4dc1b43", true, Saml2Options);

        //    return Saml2Options;
        //}

        //private static SPOptions CreateSPOptions()
        //{
        //    var swedish = CultureInfo.GetCultureInfo("sv-se");

        //    var organization = new Organization();
        //    organization.Names.Add(new LocalizedName("Sigifikant", swedish));
        //    organization.DisplayNames.Add(new LocalizedName("Signifikant", swedish));
        //    organization.Urls.Add(new LocalizedUri(new Uri("http://www.signifikant.se"), swedish));

        //    var spOptions = new SPOptions
        //    {
        //        EntityId = new EntityId("http://VaderstadPartsCatalogue_Test"),
        //        ReturnUrl = new Uri("https://testsrv25.vv.local/AssertWeb/en-GB/Vaderstad/Account/SamlRedirect"),
        //        //DiscoveryServiceUrl = new Uri("https://testsrv25.vv.local/AssertWeb/en-GB/Vaderstad/DiscoveryService"),
        //        Organization = organization,
        //        Logger = new TraceLogger(),
        //    };

        //    var techContact = new ContactPerson
        //    {
        //        Type = ContactType.Technical
        //    };
        //    techContact.EmailAddresses.Add("test@signifikant.se");
        //    spOptions.Contacts.Add(techContact);

        //    var supportContact = new ContactPerson
        //    {
        //        Type = ContactType.Support
        //    };
        //    supportContact.EmailAddresses.Add("test@signifikant.se");
        //    spOptions.Contacts.Add(supportContact);

        //    var attributeConsumingService = new AttributeConsumingService("Saml2")
        //    {
        //        IsDefault = true,
        //    };

        //    attributeConsumingService.RequestedAttributes.AddRange(new RequestedAttribute[] 
        //    {
        //        new RequestedAttribute("urn:someName") { FriendlyName = "Some Name", IsRequired = true, NameFormat =  RequestedAttribute.AttributeNameFormatUri, },
        //        new RequestedAttribute("Minimal")
        //    });

        //    spOptions.AttributeConsumingServices.Add(attributeConsumingService);

        //    // TODO använd kund-cerifikat
        //    //spOptions.ServiceCertificates.Add(new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/App_Data/Sustainsys.Saml2.Tests.pfx"));

        //    return spOptions;
        //}
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