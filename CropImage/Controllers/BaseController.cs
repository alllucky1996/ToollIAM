using CropImage.Areas;
using CropImage.Models;
using CropImage.Models.SysTem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CropImage.Controllers
{
    public class BaseController : Controller
    {
        protected long accountId;
        protected DataContext db = new DataContext();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session[SessionEnum.Email] == null || Session[SessionEnum.Email].ToString() == "")
            {
                Redirect("/Login");
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            ViewBag.Error = TempData["Error"];
            ViewBag.Success = TempData["Success"];
        }
    }
}