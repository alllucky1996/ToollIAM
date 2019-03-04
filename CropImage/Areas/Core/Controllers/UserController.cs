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
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace CropImage.Areas.Core.Controllers
{
    public class UserController : BaseController
    {
        private LogHelper<Account> _log;
        private async Task<int> CreateLogAsync(Account model, string Mota = null)
        {
            string value = JsonConvert.SerializeObject(model);
            return await _log.CreateAsync(accountId, value, Mota);
        }
        private async Task<int> CreateLogAsync(Account model, string action, string Mota = null)
        { 
            string value = JsonConvert.SerializeObject(model);
            return await _log.CreateAsync(accountId, value, action, Mota);
        }
        public string CName = "user";
        public string CText = "Người dùng";
        public string CRoute = "/Core/User/";
        void BaseView()
        {
            ViewBag.CName = CName;
            ViewBag.Ctext = CText;
            ViewBag.CRoute = CRoute;
        }
        public UserController()
        {
           
            BaseView();
            _log = new LogHelper<Account>(db);
            
        }
        // GET: Images
        public ActionResult Index()
        {
            //return View(await db.Accounts.ToListAsync());
            if (accountId == -1) return Redirect("/Login/Index"); 
            return View();
        }
        public async Task<ActionResult> Table()
        {
            //   if (accountId == -1) return Redirect("/Login/Index"); 
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));  
            var model = await db.Accounts.ToListAsync();
            return View(model);
        }

        #region create
        // GET: Images/Create
        public ActionResult Create()
        {
            if (accountId == -1)
                return Redirect("/Login/Index?returnUrl=/Core/User/Create");
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterModel model)
        {
            try
            {
                if (accountId == -1)
                {
                    return Redirect("/Login/Index?returnUrl=/Core/User/Create");
                }
                if (ModelState.IsValid)
                {
                    var ni = new Account();
                    db.Accounts.Add(new Account()
                    {
                        Code = model.Code.Trim(),
                        Email = model.Email.Trim(),
                        FullName = model.FullName.Trim(),
                        UserName = model.UserName.Trim(),
                        PassWord = StringHelper.stringToSHA512("abc@123"),
                    });
                    var result = await db.SaveChangesAsync();
                    await CreateLogAsync(ni);
                    if (result > 0)
                        return RedirectToAction("Index");
                    ViewBag.Error = "Có lỗi sảy ra trong quá trình thêm mới";
                    return View(model);
                }
                ViewBag.Error = "Hãy nhập đủ thông tin";
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }
        #endregion

        // GET: Images/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath)); 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = await db.Images.FindAsync(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            var dsTrangThai = await db.TrangThais.ToListAsync();
            ViewBag.MaTrangThai = new SelectList(dsTrangThai, "Code", "Name", image.MaTrangThai);
            return View(image);
        }

        // POST: Images/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,code,Name,Description,Uri,TrangThai")] Image image)
        public async Task<ActionResult> Edit(Account model)
        {
            // chưa làm 
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));  
            if (ModelState.IsValid)
            {
                var oldImg = await db.Accounts.FindAsync(model.Id);
                oldImg.FullName = model.FullName;
                /// còn nhiều cái khác cho sửa 
                db.Entry(oldImg).State = EntityState.Modified;
                await db.SaveChangesAsync();
                await CreateLogAsync( oldImg , "Edit","Update accounts");
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Images/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            ////  var acId = Session[SessionEnum.AccountId];
            //if (accountId == -1) return Json(new ExecuteResult() { Isok = true, Message = "Không thực hiện được thao tác do bạn chưa đăng nhập đúng cách.", Data = null });
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //var item = await db.Accounts.FindAsync(id);
            //if (item == null)
            //{
            //    return HttpNotFound();
            //}
            //try
            //{
            //    db.Images.RemoveRange(item.ListImg);
            //    db.Accounts.Remove(item);
            //    var result = await db.SaveChangesAsync();
            //    // remove file sau
            //   // await FileHelper.DeleteFileAsync(item.Uri);
            //    if (result > 0)
            //    {
            //        return Json(new ExecuteResult() { Isok = true, Data = item.FullName });
            //    }
            //    return Json(new ExecuteResult() { Isok = true, Message = "Không thực hiện được thao tác", Data = null });
            //}
            //catch (Exception ex)
            //{
            //    return Json(new ExecuteResult() { Isok = true, Message = ex.Message, Data = null });
            //}
            return Json(new ExecuteResult() { Isok = false, Message = "Thực hiện sai thao tác", Data = null });
        }


        public async Task<ActionResult> DeleteRef(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (accountId == -1) return Json(new ExecuteResult() { Isok = true, Message = "Không thực hiện được thao tác do bạn chưa đăng nhập đúng cách.", Data = null });
            var item = await db.Accounts.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            if (item.Code != null && item.Code.Contains("admin"))
            {
                return HttpNotFound();
            }
            try
            {
                foreach (var i in item.ListImg)
                {
                    db.ImageCropeds.RemoveRange(i.ListCroped);
                }
                db.Images.RemoveRange(item.ListImg);
                db.Accounts.Remove(item);
                var result = await db.SaveChangesAsync();
                if (result > 0 )
                {
                    return Json(new ExecuteResult() { Isok = true, Data = item.FullName });
                }
                return Json(new ExecuteResult() { Isok = true, Message = "Không thực hiện được thao tác", Data = null });
            }
            catch (Exception ex)
            {
                return Json(new ExecuteResult() { Isok = false, Message = ex.Message, Data = null });
            }
        }

    }
}