using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AjaxFileUpload.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            string contentType = Request.Headers["Content-Type"];
            string contentLength = Request.Headers["Content-Length"];

            if (!string.IsNullOrWhiteSpace(contentType) && contentType.StartsWith("multipart/form-data") && !string.IsNullOrWhiteSpace(contentLength))
            {
                FileUpload.RegisterHandler(Request, @"D:\TempFiles");
            }
        }
    }
}