using System.Web;
using System.Web.Optimization;

namespace FreeWheeling.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;
            //BundleTable.EnableOptimizations = true; //Un comment this to use CDN in debug.

            var jquery = new ScriptBundle("~/bundles/jquery", "//ajax.aspnetcdn.com/ajax/jquery/jquery-1.10.2.min.js").Include(
           "~/Scripts/jquery-{version}.js");
            jquery.CdnFallbackExpression = "window.jQuery";
            bundles.Add(jquery);

            bundles.Add(new ScriptBundle("~/bundles/jquerytime").Include(
                        "~/Scripts/jquery-date*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            var modernizr = new ScriptBundle("~/bundles/modernizr", "http://ajax.aspnetcdn.com/ajax/modernizr/modernizr-2.7.1.js").Include(
                        "~/Scripts/modernizr-*");
            modernizr.CdnFallbackExpression = "window.jQuery";
            bundles.Add(modernizr);

            var bootstrap = new ScriptBundle("~/bundles/bootstrap", "//netdna.bootstrapcdn.com/bootstrap/3.1.0/js/bootstrap.min.js").Include(
                      "~/Scripts/bootstrap.js");
            bootstrap.CdnFallbackExpression = "window.jQuery";
            bundles.Add(bootstrap);

            bundles.Add(new ScriptBundle("~/bundles/respond").Include(
                       "~/Scripts/respond.js"));

            //Custom javaScript
            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                   "~/Scripts/custom.js",
                   "~/Scripts/underscore.js",
                   "~/Scripts/app/extendDataService.js",
                    "~/Scripts/moment.js"));

            //Custom css
            bundles.Add(new StyleBundle("~/Content/custom").Include(
                   "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/css", "//netdna.bootstrapcdn.com/bootstrap/3.1.0/css/bootstrap.min.css").Include(
                      "~/Content/bootstrap.css"));
        }
    }
}
