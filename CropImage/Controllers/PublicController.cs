using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CropImage.Controllers
{
    public class PublicController : BaseController
    {
        // GET: Public
        public ActionResult Index()
        {
            return RedirectToAction("GennerError");
           // return View();
        }
        [Route("~/quan-ly/loi-he-thong", Name = "ManagementGeneralError")]
        public ActionResult GennerError()
        {
            ViewBag.Exception = TempData["Exception"];
            return View();
        }
    }
}