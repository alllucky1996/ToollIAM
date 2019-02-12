﻿using System;
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
    public class ImagesController : BaseController
    {
        //// GET: Core/Images
        //public ActionResult Index()
        //{
        //    return Json("", JsonRequestBehavior.AllowGet);// View();
        //}

        private LogHelper<Image> _log;


        private async Task<int> CreateLogAsync(string value, string Mota = null)
        {
            var ac = Session[SessionEnum.AccountId] == null ? accountId : Session[SessionEnum.AccountId];

            return await _log.CreateAsync((long)ac, value, Mota);
        }
        public string CName = "Iamges";
        public string CText = "Hình ảnh";
        public string CRoute = "/Core/Images/";
        void BaseView()
        {
            ViewBag.CName = CName;
            ViewBag.Ctext = CText;
            ViewBag.CRoute = CRoute;
        }
        public ImagesController()
        {
            BaseView();
            _log = new LogHelper<Image>(db);
        }
        // private DataContext db = new DataContext();


        // GET: Images
        public async Task<ActionResult> Index()
        {
            return View(await db.Images.ToListAsync());
        }
        public async Task<ActionResult> Table()
        {
            var model = await db.Images.ToListAsync();
            return View(model);
        }

        // GET: Images/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = await db.Images.FindAsync(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // GET: Images/Create
        public ActionResult Create()
        {
            var acID = Session[SessionEnum.AccountId];
            if (acID == null)
            {
                return Redirect("/Login/Index?returnUrl=/Core/Images/Create");
            }
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HttpPostedFileBase[] fileS, string KieuChu)
        {
            var acID = Session[SessionEnum.AccountId];
            if (acID == null)
            {
                return Redirect("/Login/Index?returnUrl=/Core/Images/Create");
            }

            KieuChu = string.IsNullOrEmpty(KieuChu) ? "000Kieu" : KieuChu;
            var i = Request.Files;
            foreach (var file in fileS)
            {
                if (ModelState.IsValid)
                {
                    string fullFilePath = "/Uploads/images/";
                    string filePath = Server.MapPath("~/Uploads/images/");
                    if (!Directory.Exists(filePath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(filePath);
                    }
                    if (file != null && file.ContentLength > 0)
                    {
                        Guid idImage = Guid.NewGuid();
                        string fileName = string.Format("{0}{1}", idImage.ToString().Replace("-", ""), ".png");

                        /*string fileName = string.Format("{0}{1}", idImage.ToString().Replace("-", ""), Path.GetExtension(file.FileName));*/

                        var path = Path.Combine(filePath, fileName);
                        file.SaveAs(path);
                        fullFilePath = fullFilePath + fileName;

                        var item = new Image();
                        item.code = "";// ghi gile theo cấu trúc
                        item.Name = Path.GetFileNameWithoutExtension(fileName);// ghi gile theo cấu trúc
                        item.Description = file.FileName;
                        item.TrangThai = 0;
                        item.Uri = fullFilePath;
                        item.KieuChu = KieuChu;

                        db.Images.Add(item);
                        await db.SaveChangesAsync();
                        await CreateLogAsync(item.ToString(), acID + " Đã ghi kèm file: fullFilePath");

                        return Json(new ExecuteResult() { Isok = true });
                    }
                    return Json(new ExecuteResult() { Isok = false });
                }

            }
            return View();
        }

        // GET: Images/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = await db.Images.FindAsync(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Images/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,code,Name,Description,Uri,TrangThai")] Image image)
        {
            // chưa làm 



            if (ModelState.IsValid)
            {
                db.Entry(image).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(image);
        }

        // GET: Images/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            var acId = Session[SessionEnum.AccountId];
            if (acId == null) return Json(new ExecuteResult() { Isok = true, Message = "Không thực hiện được thao tác", Data = null });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = await db.Images.FindAsync(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            try
            {
               
                db.Images.Remove(image);
                var result = await db.SaveChangesAsync();
                // remove file sau
                await FileHelper.DeleteFileAsync(image.Uri);
                if (result > 0)
                {
                    return Json(new ExecuteResult() { Isok = true, Data = image.Name });
                }
                return Json(new ExecuteResult() { Isok = true, Message = "Không thực hiện được thao tác", Data = null });
            }
            catch (Exception ex)
            {
                return Json(new ExecuteResult() { Isok = true, Message = ex.Message, Data = null });
            }
        }


        public async Task<ActionResult> DeleteRef(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = await db.Images.FindAsync(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            try
            {
                var acId = Session[SessionEnum.AccountId];
                if(acId== null) return Json(new ExecuteResult() { Isok = true, Message = "Không thực hiện được thao tác", Data = null });
                var listCroped = await db.ImageCropeds.Where(o => o.ImageId == id).ToListAsync();
                foreach (var item in listCroped)
                {
                    db.ImageCropeds.Remove(item);
                }
                db.Images.Remove(image);
                var result = await db.SaveChangesAsync();
                // remove file sau
                await FileHelper.DeleteFileAsync(image.Uri);
                if (result > 0)
                {
                    return Json(new ExecuteResult() { Isok = true, Data = image.Name });
                }
                return Json(new ExecuteResult() { Isok = true, Message = "Không thực hiện được thao tác", Data = null });
            }
            catch (Exception ex)
            {
                return Json(new ExecuteResult() { Isok = true, Message = ex.Message, Data = null });
            }
        }
    }
}