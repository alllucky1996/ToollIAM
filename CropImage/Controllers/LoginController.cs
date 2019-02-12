using CropImage.Areas;
using CropImage.Commons;
using CropImage.Models.SysTem;
using CropImage.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CropImage.Controllers
{
    public class LoginController : BaseController
    {
        // GET: Login
        public ActionResult Login()
        {
            //return View();
            return RedirectToAction("Index");
        }
        public ActionResult Register()
        {
            return View();
        }
        
        public ActionResult Index(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
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
                if(user!= null)
                {
                    Error = "Đã tồn tại userName: '"+ model.UserName + "'";
                    ViewBag.Error = Error;
                    return View(model);
                }
                db.Accounts.Add( new Account() {
                    Code = model.Code,
                    Email = model.Email,
                    FullName = model.FullName,
                    UserName = model.UserName, 
                    PassWord = StringHelper.stringToSHA512("123456")
                });
                var result = await db.SaveChangesAsync();
                if (result>0)
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
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string pass = Commons.StringHelper.stringToSHA512(model.PassWord);// StringHelper.stringToSHA512(model.PassWord.Trim());
            var user = await db.Accounts.Where(o=>o.UserName== model.UserName).FirstOrDefaultAsync();
            
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
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction("WriteWord", "ImageCropeds", new { area = "Core" });
                else
                    return Redirect(returnUrl);
            }
            ViewBag.Error = "Sai thông tin";
            return View(model);
            
        }
    }
}