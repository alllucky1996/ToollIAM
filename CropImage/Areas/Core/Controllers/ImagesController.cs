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


        private async Task<int> CreateLogAsync(Image img, string Mota = null)
        {
            // var ac = Session[SessionEnum.AccountId] == null ? accountId : Session[SessionEnum.AccountId];
            string value = JsonConvert.SerializeObject(img);
            return await _log.CreateAsync(accountId, value, Mota);
        }
        private async Task<int> CreateLogAsync(Image img, string action, string Mota = null)
        {
            //   var ac = Session[SessionEnum.AccountId] == null ? accountId : Session[SessionEnum.AccountId];
            string value = JsonConvert.SerializeObject(img);
            return await _log.CreateAsync(accountId, value, action, Mota);
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
            if (accountId == -1) return Redirect("/Login/Index");
            return View(await db.Images.Where(o => o.AccountId == accountId).ToListAsync());
        }
        public async Task<ActionResult> Table()
        {
            if (accountId == -1) return Redirect("/Login/Index");
            var model = await db.Images.Where(o => o.AccountId == accountId).ToListAsync();
            return View(model);
        }

        // GET: Images/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (accountId == -1) return Redirect("/Login/Index");
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
        public ActionResult Create( string key)
        {
            if (accountId == -1) return Redirect("/Login/Index?returnUrl=/Core/Images/Create");
            if (key == "1")
                ViewBag.Mess = "Bạn chưa có hình ảnh nào. Hãy upload hình ảnh để làm dữ liệu";
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create(HttpPostedFileBase file, string KieuChu)
        //{
        //    var acID = Session[SessionEnum.AccountId];
        //    if (acID == null)
        //    {
        //        return Redirect("/Login/Index?returnUrl=/Core/Images/Create");
        //    }

        //   // var i = Request.Files;
        //    if (ModelState.IsValid)
        //    {
        //        var fileS = file;// fileUpload[0];
        //         KieuChu = string.IsNullOrEmpty(KieuChu) ? "000Kieu" : KieuChu;

        //        string fullFilePath = "/Uploads/images/";
        //        string filePath = Server.MapPath("~/Uploads/images/");
        //        if (!Directory.Exists(filePath))
        //        {
        //            DirectoryInfo di = Directory.CreateDirectory(filePath);
        //        }
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            Guid idImage = Guid.NewGuid();
        //            string fileName = string.Format("{0}{1}", idImage.ToString().Replace("-", ""), ".png");

        //            /*string fileName = string.Format("{0}{1}", idImage.ToString().Replace("-", ""), Path.GetExtension(file.FileName));*/

        //            var path = Path.Combine(filePath, fileName);
        //            file.SaveAs(path);
        //            fullFilePath = fullFilePath + fileName;

        //            var item = new Image();
        //            item.code = "";// ghi gile theo cấu trúc
        //            item.Name = Path.GetFileNameWithoutExtension(fileName);// ghi gile theo cấu trúc
        //            item.Description = file.FileName;
        //            item.TrangThai = 0;
        //            item.Uri = fullFilePath;
        //            item.KieuChu = KieuChu;

        //            db.Images.Add(item);
        //            await db.SaveChangesAsync();
        //            await CreateLogAsync(item.ToString(), acID + " Đã ghi kèm file: " + fullFilePath);

        //            return Json(new ExecuteResult() { Isok = true });
        //        }
        //        return Json(new ExecuteResult() { Isok = false });
        //    }
        //    return View();
        //}
        public async Task<ActionResult> Create(ImageUpload imageUpload)
        {
            if (accountId == -1) return Redirect("/Login/Index?returnUrl=/Core/Images/Create");

            var KieuChu = string.IsNullOrEmpty(imageUpload.KieuChu) ? "000Kieu" : imageUpload.KieuChu;
            var i = Request.Files;

            if (ModelState.IsValid)
            {
                string fullFilePath = "/Uploads/images/";
                string filePath = Server.MapPath("~/Uploads/images/");
                if (!Directory.Exists(filePath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(filePath);
                }
                if (imageUpload.File != null && imageUpload.File.ContentLength > 0)
                {
                    Guid idImage = Guid.NewGuid();
                    string fileName = string.Format("{0}{1}", idImage.ToString().Replace("-", ""), ".png");

                    /*string fileName = string.Format("{0}{1}", idImage.ToString().Replace("-", ""), Path.GetExtension(file.FileName));*/

                    var path = Path.Combine(filePath, fileName);
                    imageUpload.File.SaveAs(path);
                    fullFilePath = fullFilePath + fileName;

                    var item = new Image();
                    item.code = "";// ghi gile theo cấu trúc
                    item.Name = Path.GetFileNameWithoutExtension(fileName);// ghi gile theo cấu trúc
                    item.Description = imageUpload.File.FileName;
                    item.MaTrangThai = 1;
                    item.Uri = fullFilePath;
                    item.KieuChu = KieuChu;
                    item.AccountId = accountId;
                    db.Images.Add(item);
                    int a= await db.SaveChangesAsync();
                    await CreateLogAsync(item, accountId + " Đã ghi kèm file: " + fullFilePath);

                    return Json(new ExecuteResult() { Isok = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new ExecuteResult() { Isok = false }, JsonRequestBehavior.DenyGet);
            }


            return View();
        }

        // GET: Images/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            //var acID = Session[SessionEnum.AccountId];
            //if (acID == null)
            //{
            //    return Redirect("/Login/Index?returnUrl=/Core/Images/Edit/" + id);
            //}
            if (accountId == -1) return Redirect("/Login/Index?returnUrl=/Core/Images/Edit/" + id); 
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
        public async Task<ActionResult> Edit(Image image)
        {
            // chưa làm 
            //var acID = Session[SessionEnum.AccountId];
            //if (acID == null)
            //{
            //    return Redirect("/Login/Index?returnUrl=/Core/Images/Edit/" + image.Id);
            //}
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));
            try
            {
                if (ModelState.IsValid)
                {
                    var oldImg = await db.Images.FindAsync(image.Id);
                    oldImg.KieuChu = image.KieuChu;
                    oldImg.MaTrangThai = image.MaTrangThai;
                    oldImg.Name = image.Name;
                    oldImg.Description = image.Description;
                    oldImg.code = image.code;
                    db.Entry(oldImg).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    await CreateLogAsync(oldImg, "Edit", "Update");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.MaTrangThai = new SelectList(await db.TrangThais.ToListAsync(), "Code", "Name", image.MaTrangThai);
                return View(image);
            }
            ViewBag.MaTrangThai = new SelectList(await db.TrangThais.ToListAsync(), "Code", "Name", image.MaTrangThai);
            return View(image);
        }

        // GET: Images/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
         //   var acId = Session[SessionEnum.AccountId];
            if (accountId == -1) return Json(new ExecuteResult() { Isok = true, Message = "Không thực hiện được thao tác", Data = null });
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
            if (accountId == -1) return Redirect("/Login/Index"); 
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
               // var acId = Session[SessionEnum.AccountId];
                if (accountId == -1) return Json(new ExecuteResult() { Isok = false, Message = "Không thực hiện được thao tác :( ", Data = null, });
                var listCroped = await db.ImageCropeds.Where(o => o.ImageId == id).ToListAsync();
                foreach (var item in listCroped)
                {
                    db.ImageCropeds.Remove(item);
                }
                db.Images.Remove(image);
                var result = await db.SaveChangesAsync();
                // remove file sau
                //var remoI= await FileHelper.DeleteFileAsync(Server.MapPath(image.Uri));
                var remoI = FileHelper.DeleteFile(Server.MapPath(image.Uri));
                var remoIC = new List<DeleteItem<ImageCroped>>();
                foreach (var item in listCroped)
                {
                    var ic = FileHelper.DeleteFile(Server.MapPath(item.Uri));
                    remoIC.Add(new DeleteItem<ImageCroped>() { DeleteOK = ic, idItem = item.Id });
                }
                // lưu ok xóa not ok
                bool d = true;
                if (remoI == false || remoIC.Any(o => o.DeleteOK = false)) d = false;
                if (result > 0 && d != true)
                {
                    return Json(new ExecuteResult() { Isok = true, Message = " Kiểm tra lại hình tồn tại trên hệ thống", IsWarning = true });
                }
                // xóa hình và lưu ok
                if (result > 0 && d == true)
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
    public class DeleteItem
    {
        public long idItem { get; set; }
        public bool DeleteOK { get; set; }
        public string Description { get; set; }
    }
    public class DeleteItem<T> where T : class, new()
    {
        public long idItem { get; set; }
        public bool DeleteOK { get; set; }
        public string Description { get; set; }
        public string DoiTuong { get { return typeof(T).Name; } }
    }
}