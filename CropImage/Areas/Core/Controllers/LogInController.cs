using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CropImage.Models;
using System.IO;
using CropImage.Models.ViewModels;
using CropImage.Controllers;
using CropImage.Models.SysTem;
using CropImage.Commons;

namespace CropImage.Areas.Core.Controllers
{
    public class LogInController : BaseController
    {
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Index()
        {
            ViewBag.Error = TempData["Error"];
            ViewBag.Message = TempData["Message"];
            ViewBag.Success = TempData["Success"];
            ViewBag.Forget = TempData["Forget"];
            Session.Abandon();
            return View();
        }
        public ActionResult Logout()
        {
            ViewBag.Error = TempData["Error"];
            ViewBag.Message = TempData["Message"];
            ViewBag.Success = TempData["Success"];
            ViewBag.Forget = TempData["Forget"];
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            string Error = string.Empty;
            if (ModelState.IsValid)
            {
                var user = await db.Accounts.Where(o => o.UserName == model.UserName).FirstOrDefaultAsync();
                if (user != null)
                {
                    Error = "Đã tồn tại userName: '" + model.UserName + "'";
                    ViewBag.Error = Error;
                    return View(model);
                }
                db.Accounts.Add(new Account()
                {
                    Code = model.Code,
                    Email = model.Email,
                    FullName = model.FullName,
                    UserName = model.UserName,
                    PassWord = StringHelper.stringToSHA512("123456")
                });
                var result = await db.SaveChangesAsync();
                if (result > 0)
                {
                    return RedirectToAction("Index");
                }
                Error = "Đã sảy ra lỗi";
            }
            Error = "Hãy nhập đầy đủ thông tin";
            ViewBag.Error = Error;
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string pass = StringHelper.stringToSHA512(model.PassWord.Trim());
            var user = await db.Accounts.Where(o => o.UserName == model.UserName).FirstOrDefaultAsync();

            if (user != null && user.PassWord == pass)
            {
                //if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                //{
                //    ViewBag.errorMessage = "You must have a confirmed email to log on.";
                //    return View("Error");
                //}
                Session[SessionEnum.Email] = user.Email;
                Session[SessionEnum.AccountId] = user.Id;
                Session[SessionEnum.FullName] = user.FullName;
                accountId = user.Id;
                return RedirectToAction("Index", "CoreHome", new { area = "Core" });
            }
            ViewBag.Error = "Sai thông tin";
            return View(model);

        }
    }
}