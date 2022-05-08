using System.Web;
using System.Web.Optimization;

namespace MultivendorWebViewer
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/helpers").Include(
                       "~/Scripts/DHM.pop-up.js",
                        "~/Scripts/multivendor.helpers.js"
                       ));
            bundles.Add(new ScriptBundle("~/bundles/main").Include(
           "~/Scripts/Main.js",
           "~/Scripts/multivendor.carousel.js"
           ));
            bundles.Add(new ScriptBundle("~/bundles/order").Include(
                    "~/Scripts/Orders.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.css",
                      "~/Content/Font-awesome.css",
                      "~/Content/Controls.css",
                      "~/Content/site.css",
                       "~/Content/HelperComponent.css",
                        "~/Content/Search.css"


                      ));
            bundles.Add(new StyleBundle("~/Content/Presentation").Include(
                 "~/Content/Presentation.css"));
            bundles.Add(new StyleBundle("~/Content/Order").Include(
                    "~/Content/Order.css"));
            bundles.Add(new StyleBundle("~/Content/Media").Include(
                "~/Content/Media.css"));
        }
   
        
}
}
