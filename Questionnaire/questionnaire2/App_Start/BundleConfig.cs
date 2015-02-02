using System.Web;
using System.Web.Optimization;

namespace Questionnaire2
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        // "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-1.11.1.js",
                        "~/Scripts/jquery.selectBox.js",
                        "~/Scripts/jquery.scrollTo-1.4.3.1-min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                //"~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery-ui-{version}.custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/tinymce").Include(
                        "~/Scripts/tinymce/jquery.tinymce.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/site.css"
                //, "~/Content/jquery.selectBox.css"
                        ));

            bundles.Add(new StyleBundle("~/Content/themes/start/css").Include(
                        "~/Content/themes/start/jquery.ui.core.css",
                        "~/Content/themes/start/jquery.ui.resizable.css",
                        "~/Content/themes/start/jquery.ui.selectable.css",
                        "~/Content/themes/start/jquery.ui.accordion.css",
                        "~/Content/themes/start/jquery.ui.autocomplete.css",
                        "~/Content/themes/start/jquery.ui.button.css",
                        "~/Content/themes/start/jquery.ui.dialog.css",
                        "~/Content/themes/start/jquery.ui.slider.css",
                        "~/Content/themes/start/jquery.ui.tabs.css",
                        "~/Content/themes/start/jquery.ui.datepicker.css",
                        "~/Content/themes/start/jquery.ui.progressbar.css",
                        "~/Content/themes/start/jquery.ui.theme.css"));
        }
    }
}