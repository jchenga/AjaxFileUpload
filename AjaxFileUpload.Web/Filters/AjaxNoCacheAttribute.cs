using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjaxFileUpload.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AjaxNoCacheAttribute : ActionFilterAttribute
    {
        
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
                cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                cache.SetCacheability(HttpCacheability.NoCache);
                cache.SetNoStore();
            }
        }
    }
}