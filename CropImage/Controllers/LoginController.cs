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
        private LogHelper<Account> _log;
        private async Task<int> CreateLogAsync(string value, string Mota = null)
        {
            return await _log.CreateAsync(accountId, value, Mota);
        }
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
        [HttpGet]
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
        [HttpPost]
        public ActionResult Index()
        {
            ViewBag.returnUrl = "";
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
            if (accountId == -1) return Redirect("/Login/Index"); 
            var user = await db.Accounts.Where(o => o.UserName == model.UserName).FirstOrDefaultAsync();
            string Error = string.Empty;
            if (ModelState.IsValid)
            {
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
                    PassWord = StringHelper.stringToSHA512("abc@123")
                });
                var result = await db.SaveChangesAsync();
                if (result > 0)
                {
                    await CreateLogAsync("Create account" + model.UserName);
                    return RedirectToAction("Index");
                }
                Error = "Đã sảy ra lỗi";
                return View(model);
            }
            Error = "Hãy nhập đầy đủ thông tin";
            ViewBag.Error = Error;
            return View(model);
        }

        [HttpPost]
      //  [AllowAnonymous]
      //  [ValidateAntiForgeryToken]
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
                if (await db.Images.Where(o=>o.AccountId == accountId).CountAsync()<1) return Redirect("/Core/Images/Create?key=1");
                if (string.IsNullOrEmpty(returnUrl))
                    //return RedirectToAction("WriteWord", "ImageCropeds", new { area = "Core" });
                    return RedirectToAction("Index", "CoreHome", new { area = "Core" });
                    
                else
                    return Redirect(returnUrl);
            }
            ViewBag.Error = "Sai thông tin";
            return View(model);
            
        }
        [HttpGet]
        public async Task<ActionResult> InitRole(string key)
        {
            if(key == "phongcongnghe")
            {
               // db.Accounts.Add(new Account() {FullName="Phạm thu hà", Email="hapt@fsivietnam.com.vn",PassWord= StringHelper.stringToSHA512("12356"),UserName="HaPT" });
                // tạo role
                List<Role> roles = new List<Role>();
                roles.Add(new Role() { Code = "QuanTri", Name = "Quản trị" });
                roles.Add(new Role() { Code = "NguoiCat", Name = "Người cắt", DesCription = "Người cắt hình" });
                roles.Add(new Role() { Code = "NguoiThamQuan", Name = "Thăm quan" });
                roles.ForEach(i => db.Roles.Add(i));
                await db.SaveChangesAsync();
                // tạo phân quyền 
                List<AccountRole> acr = new List<AccountRole>();
                var Ha = await db.Accounts.Where(o => o.UserName.ToLower() == "hapt").FirstOrDefaultAsync();
                if(Ha== null) db.Accounts.Add(new Account() { FullName = "Phạm thu hà", Email = "hapt@fsivietnam.com.vn", PassWord = StringHelper.stringToSHA512("12356"), UserName = "HaPT" });
                var d = await db.Accounts.Where(o => o.UserName.ToLower() == "alllucky").FirstOrDefaultAsync();
                acr.Add(new AccountRole() { AccountId = Ha.Id, CoreRole = "QuanTri", CreateDate = DateTime.Now });
                acr.Add(new AccountRole() { AccountId = d.Id, CoreRole = "QuanTri", CreateDate = DateTime.Now });
                acr.ForEach(o => db.AccountRoles.Add(o));
                var listAc = await db.Accounts.Where(o => o.Id != Ha.Id && o.Id != d.Id).ToListAsync();
                foreach (var item in listAc)
                {
                    db.AccountRoles.Add(new AccountRole() { AccountId = item.Id, CoreRole = "NguoiCat" });
                }
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}