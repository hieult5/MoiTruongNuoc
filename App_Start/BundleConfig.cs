﻿using System.Web.Optimization;

namespace stask
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.bundle.min.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.css",
                      "~/Scripts/toastr.min.css",
                      "~/Content/animate.css",
                      "~/Content/site.css",
                      "~/Content/Skin/Fontawesome/css/all.css"));
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                    "~/Scripts/jquery.jOrgChart.js",
                      "~/Scripts/moment.js",
                      "~/Scripts/angular.min.js",
                      "~/Scripts/angular-lazy-img.min.js",
                      "~/Scripts/ng-file-upload.min.js",
                      "~/Scripts/angularTreeview.min.js",
                      "~/Scripts/tree-grid-directive.js",
                      "~/Scripts/i18n/angular-locale_vi-vn.js",
                      "~/Scripts/angular-sanitize.min.js",
                      "~/Scripts/angular-route.min.js",
                      "~/Scripts/angular-ckeditor.min.js",
                      "~/Scripts/angular-resource.min.js",
                      "~/Scripts/saveHtmlToPdf.min.js",
                      "~/Scripts/angular-vs-repeat.min.js",
                      "~/Scripts/angular-animate.min.js",
                      "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                      "~/Scripts/angular-filter.min.js",
                      //"~/Scripts/angular-dragdrop.min.js",
                      "~/Scripts/angular-drag-and-drop-lists.js",
                      "~/Scripts/jsTag.min.js",
                      "~/Scripts/angucomplete.js",
                      "~/Scripts/datetimepicker.js",
                      "~/Scripts/cssCurrencyInput.js",
                      "~/Scripts/dateTimeInput.js",
                      //"~/Scripts/datetimepicker.js",
                      //"~/Scripts/datetimepicker.templates.js",
                      "~/Scripts/dialogs.min.js",
                      "~/Scripts/toastr.min.js",
                      "~/Scripts/angularjs-typeahead-dropdown.min.js",
                      "~/Scripts/draganddrop.min.js",
                      "~/Scripts/ngtimeago.js",
                      "~/Scripts/angular-ui-switch.min.js",
                      "~/Scripts/smart-table.min.js",
                      "~/Scripts/angular-ui-router.min.js",
                      "~/Scripts/FileSaver.min.js",
                      //"~/Scripts/sortable.js",
                      "~/Scripts/ocLazyLoad.min.js"
                      ));
            bundles.Add(new StyleBundle("~/Content/Skin/app-assets/cssSkin").Include(
                      "~/Content/Skin/app-assets/css/bootstrap.css",
                      "~/Content/Skin/app-assets/css/app.css",
                      "~/Content/Skin/app-assets/css/core/menu/menu-types/vertical-compact-menu.css",
                      "~/Content/Skin/app-assets/fonts/simple-line-icons/style.css",
                      "~/Content/Skin/assets/css/style.css",
                      "~/Content/Skin/app-assets/css/Style.css",
                      "~/Content/Skin/Login.css"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/jsSkin").Include(
                          "~/App/FireBase/firebase-app.js",
                          "~/App/FireBase/firebase-messaging.js",
                          "~/App/FireBase/firebase-database.js",
                          "~/App/FireBase/initFireBase.js"
                          ));
            BundleTable.EnableOptimizations = true;
        }
    }
}
