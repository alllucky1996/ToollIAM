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
using System.Diagnostics;
using Newtonsoft.Json;

namespace CropImage.Areas.Core.Controllers
{
    public class ImageCropedsController : BaseController
    {
        // private DataContext db = new DataContext();
        #region log helper
        private LogHelper<ImageCroped> _log;
        private async Task<int> CreateLogAsync(string value, string Mota = null)
        {
            //    var ac = Session[SessionEnum.AccountId] == null ? 0 : Session[SessionEnum.AccountId];
            var x = await _log.CreateAsync(accountId, value, Mota);
            return x;
        }
        private async Task<int> CreateLogAsync(ImageCroped model, string action, string mota = null)
        {
            string value = JsonConvert.SerializeObject(model);
            return await _log.CreateAsync(accountId, value, action, mota);
        }
        #endregion
        public ImageCropedsController()
        {
            _log = new LogHelper<ImageCroped>(db);
        }
        #region index
        // GET: ImageCropeds
        public ActionResult Index()
        {
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));
            return View();
        }
        public async Task<ActionResult> Table()
        {
            //if (accountId == -1) return Redirect("/Login/Index");
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));
            var imageCropeds = await db.ImageCropeds.Where(o => o.Image.AccountId == accountId).Include(o => o.Image).ToListAsync();
            return View(imageCropeds);
        }

        #endregion
        // GET: ImageCropeds/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));
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
        #region cắt ảnh hàng loạt
        // cắt file hàng loạt rồi hiển thị ra tại trang index
        public async Task<ActionResult> Create()
        {
            string Error = "";
            //var acID = accountId;// Session[SessionEnum.AccountId];
            //if (accountId ==-1)
            //{
            //    return Json(new ExecuteResult() { Isok = false, Data = "/Login" });
            //}
            if (accountId == -1) return Redirect("/Login/Index");
            try
            {
                ViewBag.ImageId = new SelectList(db.Images, "Id", "code");
                // return View();
                // truy vấn croped
                if (true)
                {
                    // xóa thư mục cũ : leve 3
                    string pathByAccount = Server.MapPath("~/Traning/data/" + 3 + "/" + accountId);
                    if (FileHelper.DeleteFolder(pathByAccount))
                    {
                        Debug.WriteLine("xóa thư mục thành công");
                    }
                    Debug.WriteLine("Không xóa được thư mục, tạo thư mục và ghi vào ");

                    var listAll = await db.ImageCropeds.Where(o => o.Lever == 3 && o.Image.AccountId == accountId).ToListAsync();
                    var list = listAll.Where(o => o.Lable.Split(' ').Count() == 1);
                    foreach (var item in list)
                    {
                        string erW = "", erS = "";
                        // gọi về hình gốc 
                        string kieu = string.IsNullOrEmpty(item.Image.KieuChu) ? "000kieu" : item.Image.KieuChu;
                        string nameTemplate = item.Lever + "/" + accountId + "/" + accountId + "-" + kieu;
                        string path = Server.MapPath("~/Traning/data/" + nameTemplate);
                        //var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + item.Image.Uri), path, item, acID + "-" + kieu);

                        var newUrl = await GhiFileTraining.CutImage(Server.MapPath("~" + item.Image.Uri), path, item, accountId + "-" + kieu);
                        if (!newUrl.Contains("[ERROR]"))
                        {
                            item.Uri = "/Traning/data/" + item.Lever + "/" + accountId + "/" + accountId + "-" + kieu + "/" + newUrl;
                            // item.code = "";
                            db.Entry(item).State = EntityState.Modified;
                            var result = await db.SaveChangesAsync();
                            if (result < 1)
                            {
                                erS = " Không sửa được dữ liệu, ";
                            }
                        }
                        else
                            Error += item.Id + erS + newUrl + ";";
                    }
                    if (Error != "")
                        return Json(new ExecuteResult() { Isok = false, Message = Error });
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
                Log logExportImage = new Log();
                var x = await logExportImage.CreateAsync(accountId, new Log() { Action = "ExporstFile", EntityName = "Orther", Descript = "Đã xuất tất cả các file ảnh đã khoanh" });
                return Json(new ExecuteResult() { Isok = true, Data = "ok" });
            }
            catch (Exception ex)
            {
                return Json(new ExecuteResult() { Isok = false, Message = ex.Message });
            }
        }
        #endregion

        // GET: ImageCropeds/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));
            string er = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageCroped item = await db.ImageCropeds.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            //nếu chưa có link ảnh thì cắt hình

            //try
            //{
            //    // quay về ảnh gốc lấy hình và tên

            //    //string path = Server.MapPath("~/Traning/data/" + imageCroped.Image.Name + "/" + imageCroped.Lever);
            //    //var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + imageCroped.Image.Uri), path, imageCroped);
            //    string kieu = string.IsNullOrEmpty(imageCroped.Image.KieuChu) ? "000kieu" : imageCroped.Image.KieuChu;
            //    string nameTemplate = imageCroped.Lever + "/" + acID + "/" + acID + "-" + kieu;
            //    string path = Server.MapPath("~/Traning/data/" + nameTemplate);
            //    var newUrl = await GhiFileTraining.CutImage(Server.MapPath("~" + imageCroped.Image.Uri), path, imageCroped, acID + "-" + kieu);
            //    if (!newUrl.Contains("[ERROR]"))
            //    {
            //        imageCroped.Uri = "/Traning/data/" + nameTemplate + newUrl;
            //        db.Entry(imageCroped).State = EntityState.Modified;
            //        await db.SaveChangesAsync();

            //    }
            //}
            //catch (Exception ex)
            //{
            //    er = ex.Message;
            //}
            string Error = "";

            string erW = "", erS = "";
            // gọi về hình gốc 
            string kieu = string.IsNullOrEmpty(item.Image.KieuChu) ? "000kieu" : item.Image.KieuChu;
            string nameTemplate = item.Lever + "/" + accountId + "/" + accountId + "-" + kieu;
            string path = Server.MapPath("~/Traning/data/" + nameTemplate);
            //var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + item.Image.Uri), path, item, acID + "-" + kieu);

            var newUrl = await GhiFileTraining.CutImage(Server.MapPath("~" + item.Image.Uri), path, item, accountId + "-" + kieu);
            if (!newUrl.Contains("[ERROR]"))
            {
                item.Uri = "/Traning/data/" + item.Lever + "/" + accountId + "/" + accountId + "-" + kieu + "/" + newUrl;
                // item.code = "";
                db.Entry(item).State = EntityState.Modified;
                var result = await db.SaveChangesAsync();
                if (result < 1)
                {
                    erS = " Không sửa được dữ liệu, ";
                }
            }
            else
                Error += item.Id + erS + newUrl + ";";

            ViewBag.Error = er;
            ViewBag.ImageId = new SelectList(db.Images, "Id", "Name", item.ImageId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ImageCroped imageCroped)
        {
            //var acID = accountId;// Session[SessionEnum.AccountId];
            //if (acID == -1)
            //{
            //    return Redirect("/Login");
            //}
            if (accountId == -1) return Redirect("/Login/Index");
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
                    // gọi về hình gốc 
                    string kieu = string.IsNullOrEmpty(editIteam.Image.KieuChu) ? "000kieu" : editIteam.Image.KieuChu;
                    string nameTemplate = editIteam.Lever + "/" + accountId + "/" + accountId + "-" + kieu;
                    string path = Server.MapPath("~/Traning/data/" + nameTemplate);
                    var newUrl = await GhiFileTraining.CutImage(Server.MapPath("~" + editIteam.Image.Uri), path, editIteam, accountId + "-" + kieu);
                    if (!newUrl.Contains("[ERROR]"))
                    {
                        editIteam.Uri = "/Traning/data/" + editIteam.Lever + "/" + accountId + "/" + accountId + "-" + kieu + "/" + newUrl;
                    }
                }
                editIteam.Index = imageCroped.Index;
                editIteam.Line = imageCroped.Line;

                db.Entry(editIteam).State = EntityState.Modified;
                var r = await db.SaveChangesAsync();
                if (r < 1)
                    return View(imageCroped);
                else
                {
                    return Redirect("/Core/ImageCropeds/Index");
                }
            }
            ViewBag.ImageId = new SelectList(db.Images, "Id", "code", imageCroped.ImageId);
            return View(imageCroped);
        }

        // GET: ImageCropeds/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (accountId == -1) return Redirect("/Login/Index");
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
        #endregion
        // ghi từ db vào file nhãn 
        public async Task<ActionResult> WriteFile(long? AccountId)
        {
            //format: a01-000u-00-00| ok| 154| 408 768 27 51| AT| A
            var account = await db.Accounts.FindAsync(Session[SessionEnum.AccountId]);
            if (account.UserName == "HaPT")
            {
                Session.Clear();
                return await Write("abc@2018", "all", account);
            }
            Session.Clear();
            return await Write("abc@2018", "one", account);
        }
        public async Task<ActionResult> WriteByAllFile(long? AccountId)
        {
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));
            //   if (accountId == -1) return Redirect("/Login/Index"); 
            if (accountId != -1)
            {
                var account = await db.Accounts.FindAsync(accountId);
                Session.Clear();
                await CreateLogAsync("Đã tạo file training");
                return await Write("abc@2018", "all", account);
            }
            return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Bạn không có quyền sử dụng chức năng này" });
        }
        // view exporst
        #region export
        public ActionResult WriteWord()
        {
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));
            return View();
        }
        #endregion
        public async Task<ActionResult> ImageAll(long? AccountId)
        {
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));
            string Error = "";
            // với mỗi 1 account id sẽ ghi 1 gile not backup
            if (true)
            {
                // xóa thư mục cũ : leve 3
                string pathByAccount = Server.MapPath("~/Traning/data/" + 3 + "/" + accountId);
                if (FileHelper.DeleteFolder(pathByAccount))
                {
                    Debug.WriteLine("xóa thư mục thành công");
                }
                Debug.WriteLine("Không xóa được thư mục, tạo thư mục và ghi vào ");
                var listAll = await db.ImageCropeds.Where(o => o.Lever == 3 && o.Image.AccountId == accountId).ToListAsync();
                var list = listAll.Where(o => o.Lable.Split(' ').Count() == 1);
                foreach (var item in list)
                {
                    string erW = "", erS = "";
                    // gọi về hình gốc 
                    string kieu = string.IsNullOrEmpty(item.Image.KieuChu) ? "000kieu" : item.Image.KieuChu;
                    string nameTemplate = item.Lever + "/" + accountId + "/" + accountId + "-" + kieu;
                    string path = Server.MapPath("~/Traning/data/" + nameTemplate);
                    //var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + item.Image.Uri), path, item, acID + "-" + kieu);

                    var newUrl = await GhiFileTraining.CutImage(Server.MapPath("~" + item.Image.Uri), path, item, accountId + "-" + kieu);
                    if (!newUrl.Contains("[ERROR]"))
                    {
                        item.Uri = "/Traning/data/" + item.Lever + "/" + accountId + "/" + accountId + "-" + kieu + "/" + newUrl;
                        // item.code = "";
                        db.Entry(item).State = EntityState.Modified;
                        var result = await db.SaveChangesAsync();
                        if (result < 1)
                        {
                            erS = " Không sửa được dữ liệu, ";
                        }
                    }
                    else
                        Error += item.Id + erS + newUrl + ";";
                }
                //if (Error != "")
                //    return Json(new ExecuteResult() { Isok = false, Message = Error });
            }
            string auth = accountId.ToString();
            string Source = Server.MapPath("~/Traning/data/3/"+auth);
            string target = Server.MapPath("~/Traning/Temp/" + auth);
            var r = AddZipFile(Source, target);
            
            if (r != "")
            {
                await CreateLogAsync("Đã tải tập dữ liệu training: ");
                return Json(new ExecuteResult() { Isok = true, Data = "/Traning/Temp/" + auth + "/" + r, Message = "Tạo file Thành công" });
            }
            await CreateLogAsync("Trích xuất file tải về có vấn đề: " + r);
            return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Không tạo được file" });
        }


        private async Task<JsonResult> Write(string key, string type, Account authencation)
        {
            type = type == null ? "one" : type;
            try
            {
                ////string keyEn = Commons.StringHelper.stringToSHA512(key);
                //var dbKey = db.Khoas.FirstOrDefault().KeyValue;
                //dbKey = string.IsNullOrEmpty(dbKey) == true ? "ẹc" : dbKey;
                //if (key == dbKey)
                if (true)
                {

                    List<string> listFileName = new List<string>();
                    List<string> listLable = new List<string>();
                    // lấy list nhãn đã gán
                    // lọc từ nào mà có 1 âm tiết thôi
                    // var listCroped = db.ImageCropeds.Where(o=>o.Lable.Split(' ').Count()==1).ToList();
                    var listAll = await db.ImageCropeds.Where(o => o.Lever == 2 || o.Lever == 3).ToListAsync();
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

                    string comment = "#" + authencation.FullName + "Create date: " + DateTime.Now.ToString() + "#";// "#" + db.Khoas.FirstOrDefault().Description + " " + DateTime.Now.ToString() + "#";
                    string temp = "/TrainingFile/";
                    if (type.Equals("one"))
                        temp = "/TrainingFile/Thay/";
                    string word = "word" + authencation.Id + ".txt";
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
                string fileName =DateTime.Now.ToString("yyyy_MM_dd") +"_"+ Guid.NewGuid().ToString() + ".zip";
                ZipFile.CreateFromDirectory(source, targetFolder + "\\" + fileName, CompressionLevel.Fastest, true);
                return fileName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        #endregion
    }
}
