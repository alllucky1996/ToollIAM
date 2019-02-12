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
using CropImage.Models.ViewModels;
using CropImage.Handler.Crop;
using Emgu.CV;
using Emgu.CV.Structure;
using CropImage.Commons;
using System.IO;
using CropImage.Handler;
using System.IO.Compression;
using CropImage.Controllers;
using CropImage.Models.SysTem;

namespace CropImage.Areas.Core.Controllers
{
    public class ImageCropedsController : BaseController
    {
       // private DataContext db = new DataContext();

        // GET: ImageCropeds
        public async Task<ActionResult> Index()
        {
            var imageCropeds = db.ImageCropeds.Include(i => i.Image);
            return View(await imageCropeds.ToListAsync());
        }
        public async Task<ActionResult> Table()
        {
            var imageCropeds = db.ImageCropeds.Include(i => i.Image);
            return View(await imageCropeds.ToListAsync());
        }

        // GET: ImageCropeds/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageCroped imageCroped = await db.ImageCropeds.FindAsync(id);
            if (imageCroped == null)
            {
                return HttpNotFound();
            }
            return View(imageCroped);
        }

        // GET: ImageCropeds/Create
        // cắt file hàng loạt rồi hiển thị ra
        public async Task<ActionResult> Create()
        {
            var acID = Session[SessionEnum.AccountId];
            if (acID == null)
            {
                return Json(new ExecuteResult() { Isok = false, Data = "/Login" });
            }
            try
            {
                ViewBag.ImageId = new SelectList(db.Images, "Id", "code");
                // return View();
                // truy vấn croped
                if (true)
                {
                    var listAll = await db.ImageCropeds.Where(o =>  o.Lever == 3).ToListAsync();
                    var list = listAll.Where(o => o.Lable.Split(' ').Count() == 1);
                    foreach (var item in list)
                    {
                        // gọi về hình gốc 
                        string kieu = string.IsNullOrEmpty(item.Image.KieuChu) ? "000kieu" : item.Image.KieuChu;
                        string nameTemplate = item.Lever + "/" + acID + "/" + acID + "-" + kieu;
                        string path = Server.MapPath("~/Traning/data/" + nameTemplate);
                        var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + item.Image.Uri), path, item, acID + "-" + kieu);
                        if (newUrl != "")
                        {
                            item.Uri = "/Traning/data/" + item.Lever + "/" + acID + "/" + acID + "-" + kieu + "/" + newUrl;
                            // item.code = "";
                            db.Entry(item).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                    }
                    // Crop(Image<Bgr, byte> img, int x, int y, int width, int height);
                    //  return RedirectToAction("Index");
                }
                if (true)
                {
                    //var list = await db.ImageCropeds.ToListAsync();
                    //foreach (var item in list)
                    //{
                    //    // gọi về hình gốc 
                    //    string kieu = string.IsNullOrEmpty(item.Image.KieuChu) ? "00kieu" : item.Image.KieuChu;
                    //    string path = Server.MapPath("~/Traning/data/" + item.Lever + "/" + item.Image.Name + "/" + item.Image.Name + "-" + kieu);
                    //    var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + item.Image.Uri), path, item);
                    //    if (newUrl != "")
                    //    {
                    //        item.Uri = "/Traning/data/" + item.Lever + "/" + item.Image.Name + "/" + item.Image.Name + "-" + kieu + "/" + newUrl;
                    //        // item.code = "";
                    //        db.Entry(item).State = EntityState.Modified;
                    //        await db.SaveChangesAsync();
                    //    }
                    //}
                    //// Crop(Image<Bgr, byte> img, int x, int y, int width, int height);
                    ////  return RedirectToAction("Index");
                }
                return Json(new ExecuteResult() { Isok = true, Data = "ok" });
            }
            catch (Exception ex)
            {
                return Json(new ExecuteResult() { Isok = false, Message = ex.Message });
            }
        }

        // GET: ImageCropeds/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            var acID = Session[SessionEnum.AccountId];
            if (acID == null)
            {
                return Redirect("/Login");
            }

            string er = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageCroped imageCroped = await db.ImageCropeds.FindAsync(id);
            if (imageCroped == null)
            {
                return HttpNotFound();
            }
            //nếu chưa có link ảnh thì cắt hình
            if (imageCroped.Uri == null)
            {
                try
                {
                    // quay về ảnh gốc lấy hình và tên

                    //string path = Server.MapPath("~/Traning/data/" + imageCroped.Image.Name + "/" + imageCroped.Lever);
                    //var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + imageCroped.Image.Uri), path, imageCroped);
                    string kieu = string.IsNullOrEmpty(imageCroped.Image.KieuChu) ? "000kieu" : imageCroped.Image.KieuChu;
                    string nameTemplate = imageCroped.Lever + "/" + acID + "/" + acID + "-" + kieu;
                    string path = Server.MapPath("~/Traning/data/" + nameTemplate);
                    var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + imageCroped.Image.Uri), path, imageCroped, acID + "-" + kieu);
                    if (newUrl != "")
                    {
                        imageCroped.Uri = "/Traning/data/" + nameTemplate + newUrl;
                        db.Entry(imageCroped).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    er = ex.Message;
                }
            }
            ViewBag.Error = er;
            ViewBag.ImageId = new SelectList(db.Images, "Id", "Name", imageCroped.ImageId);
            return View(imageCroped);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ImageCroped imageCroped)
        {
            var acID = Session[SessionEnum.AccountId];
            if (acID == null)
            {
                return Redirect("/Login");
            }

            if (ModelState.IsValid)
            {
                var editIteam = await db.ImageCropeds.FindAsync(imageCroped.Id);
                editIteam.code = imageCroped.code;
                if (imageCroped.IdDau != null)
                {
                    editIteam.IdDau = imageCroped.IdDau;
                }
                editIteam.ImageId = imageCroped.ImageId;
                editIteam.Lable = imageCroped.Lable;
                editIteam.IsOK = imageCroped.IsOK;

                //có sửa thư tự thì cập nhật file và data
                bool ok = false;
                if (editIteam.Index != imageCroped.Index || editIteam.Line != imageCroped.Line)
                {
                    //var rootImage = new Image<Bgr, byte>(Server.MapPath("~" + editIteam.Image.Uri));
                    //string path = Server.MapPath("~/Traning/data/" + editIteam.Image.Name);
                    //string nameFile = editIteam.Image.Name + "-" + editIteam.Line.ToString("D2") + "-" + editIteam.Index.ToString("D2") + ".png";
                    //ok = CropHelper.Save(CropHelper.Crop(rootImage, editIteam.X, editIteam.Y, editIteam.Width, editIteam.Height), path + "\\" + nameFile);
                    string kieu = string.IsNullOrEmpty(editIteam.Image.KieuChu) ? "000kieu" : editIteam.Image.KieuChu;
                    string nameTemplate = editIteam.Lever + "/" + acID + "/" + acID + "-" + kieu;
                    string path = Server.MapPath("~/Traning/data/" + nameTemplate);
                    var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + editIteam.Image.Uri), path, editIteam, acID + "-" + kieu);
                }
                editIteam.Index = imageCroped.Index;
                editIteam.Line = imageCroped.Line;

                db.Entry(editIteam).State = EntityState.Modified;
                var r = await db.SaveChangesAsync();

                if (r < 1)
                    return View(imageCroped);
                else
                {
                    return RedirectToAction("Index", "ImageCropeds", null);
                }
            }
            ViewBag.ImageId = new SelectList(db.Images, "Id", "code", imageCroped.ImageId);
            return View(imageCroped);
        }

        // GET: ImageCropeds/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageCroped image = await db.ImageCropeds.FindAsync(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            try
            {
                db.ImageCropeds.Remove(image);

                await db.SaveChangesAsync();
                // remove file sau

                //return RedirectToAction("Index");
                return Json(new ExecuteResult() { Isok = true, Data = image.Lable });
            }
            catch (Exception ex)
            {
                return Json(new ExecuteResult() { Isok = true, Message = ex.Message, Data = null });
            }
        }


        #region ghi file WriteFile
        // ghi từ db vào file nhãn 
        public async Task<ActionResult> WriteFile(long? AccountId)
        {
            //format: a01-000u-00-00| ok| 154| 408 768 27 51| AT| A
            var account = await db.Accounts.FindAsync(Session[SessionEnum.AccountId]);
            if (account.UserName == "HaPT")
            {
                Session.Clear();
                return await Write("abc@2018", "all",account);
            }
            Session.Clear();
            return await Write("abc@2018","one", account);
        }
        public async Task<ActionResult> WriteByAllFile(long? AccountId)
        {// khống chế người ghi chỉ có hà ghi :)
            var account = await db.Accounts.FindAsync(Session[SessionEnum.AccountId]);
            if(account!= null)
            {
                Session.Clear();
                return await Write("abc@2018", "all", account);
            }
            return Json(new ExecuteResult() { Isok = false, Data = null, Message="Bạn không có quyền cho chức năng này" });
        }
        public ActionResult WriteWord()
        {
            var id = Session[SessionEnum.AccountId];
            if (id== null)
            {
               return Redirect("/Login");
            }
            return View();
        }
        public async Task<ActionResult> ImageAll(long? AccountId)
        {
            var acID = Session[SessionEnum.AccountId];
            if (acID == null)
            {
                return Redirect("/Login");
            }

            // với mỗi 1 account id sẽ ghi 1 gile not backup
            if (true)
            {
                // ghi lại file
                var listAll = await db.ImageCropeds.Where(o => o.Lever == 2||o.Lever==3).ToListAsync();
                var list = listAll.Where(o => o.Lable.Split(' ').Count() == 1);
                foreach (var item in list)
                {
                    // gọi về hình gốc 
                    ////string kieu = string.IsNullOrEmpty(item.Image.KieuChu) ? "000kieu" : item.Image.KieuChu;
                    ////string path = Server.MapPath("~/Traning/data/" + item.Lever + "/" + item.Image.Name + "/" + item.Image.Name + "-" + kieu);
                    //string kieu = string.IsNullOrEmpty(item.Image.KieuChu) ? "000kieu" : item.Image.KieuChu;
                    //string path = Server.MapPath("~/Traning/data/" + item.Lever + "/" + acID + "/" + acID + "-" + kieu);
                    //var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + item.Image.Uri), path, item);
                    string kieu = string.IsNullOrEmpty(item.Image.KieuChu) ? "000kieu" : item.Image.KieuChu;
                    string nameTemplate = item.Lever + "/" + acID + "/" + acID + "-" + kieu;
                    string path = Server.MapPath("~/Traning/data/" + nameTemplate);
                    var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + item.Image.Uri), path, item, acID + "-" + kieu);
                    if (newUrl != "")
                    {
                        item.Uri = "/Traning/data/" + item.Lever + "/" + acID + "/" + acID + "-" + kieu + "/" + newUrl;
                        // item.code = "";
                        db.Entry(item).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
            }
            string Source = Server.MapPath("~/Traning/data");
            string auth = acID.ToString() ;// AccountId == null ? "1" : AccountId.Value.ToString();
            string target = Server.MapPath("~/Traning/Temp/" + auth);
            var r = AddZipFile(Source, target);
            if (r != "")
                return Json(new ExecuteResult() { Isok = true, Data = "/Traning/Temp/" + auth + "/" + r, Message = "Tạo file Thành công" });
            return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Không tạo được file" });
        }
        #endregion

        private async Task<JsonResult> Write(string key, string type, Account authencation)
        {
            type = type == null ? "one" : type;
            try
            {
                ////string keyEn = Commons.StringHelper.stringToSHA512(key);
                //var dbKey = db.Khoas.FirstOrDefault().KeyValue;
                //dbKey = string.IsNullOrEmpty(dbKey) == true ? "ẹc" : dbKey;
                //if (key == dbKey)
                if(true)
                {

                    List<string> listFileName = new List<string>();
                    List<string> listLable = new List<string>();
                    // lấy list nhãn đã gán
                    // lọc từ nào mà có 1 âm tiết thôi
                   // var listCroped = db.ImageCropeds.Where(o=>o.Lable.Split(' ').Count()==1).ToList();
                    var listAll = await db.ImageCropeds.Where(o => o.Lever == 2||o.Lever==3).ToListAsync();
                    var listCroped = listAll.Where(o => o.Lable.Split(' ').Count() == 1);
                    // lấy ra tên hình ảnh
                    foreach (var crop in listCroped)
                    {
                        listFileName.Add(db.Images.Find(crop.ImageId).Uri);
                        // dùng cho 1 hình
                        string nameImage = "";
                        if (type.Equals("one"))
                            nameImage = db.Images.Find(crop.ImageId).Name;
                        else
                        {
                            nameImage = Path.GetFileNameWithoutExtension(crop.Uri);
                        }
                        listLable.Add(nameImage + " " + crop.Info);
                    }

                    string comment ="#"+ authencation.FullName+"Create date: "+ DateTime.Now.ToString() + "#" ;// "#" + db.Khoas.FirstOrDefault().Description + " " + DateTime.Now.ToString() + "#";
                    string temp = "/TrainingFile/";
                    if (type.Equals("one"))
                        temp = "/TrainingFile/Thay/";
                    string word = "word"+ authencation.Id+ ".txt";
                    string pathFile = Path.Combine(temp, word);
                    string path = Server.MapPath("~" + pathFile);
                    if (!System.IO.File.Exists(path))
                    {
                        FileHelper.CreateFile(Server.MapPath("~" + temp), word, comment);
                    }
                    System.IO.File.Delete(path);
                    FileHelper.CreateFile(Server.MapPath("~" + temp), word, comment);
                    foreach (var item in listLable)
                    {
                        FileHelper.AppenAllText(path, "\n" + item);
                    }
                    return Json(new ExecuteResult() { Isok = true, Data = pathFile, Message = "Is ok" }, JsonRequestBehavior.AllowGet);

                }
               // return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Key k đúng hoặc k có quyền ghi file" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ExecuteResult() { Isok = false, Data = null, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #region ZipFile
        public string AddZipFile(string source, string targetFolder)
        {
            try
            {
                // làm rỗng thư mục
                FileHelper.CreateFolderIfNotExist(targetFolder);
                FileHelper.DeleteFolder(targetFolder);
                FileHelper.CreateFolderIfNotExist(targetFolder);
                //thêm file mới vào
                string fileName = Guid.NewGuid().ToString() + ".zip";
                ZipFile.CreateFromDirectory(source, targetFolder + "\\" + fileName, CompressionLevel.Fastest,true);
                return fileName;
            }
            catch
            {
                return "";
            }
        }
        #endregion
    }
}
