using AjaxFileUpload.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjaxFileUpload.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.FileId = Guid.NewGuid();
            return View();
        }

        public PartialViewResult UploadFile(Guid id)
        {
            ViewBag.FileId = id;
            return PartialView();
        }

        [AjaxNoCache]
        public ActionResult Status(Guid id)
        {
            return Json(new { isok = true, message = FileUpload.CheckProgress(id) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string SaveFile()
        {
            
            return String.Format("UploadComplete {0}", Request.Files[0].FileName);
        }
    }
}
