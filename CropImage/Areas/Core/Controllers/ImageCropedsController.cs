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

namespace CropImage.Areas.Core.Controllers
{
    public class ImageCropedsController : Controller
    {
        private DataContext db = new DataContext();

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
            ViewBag.ImageId = new SelectList(db.Images, "Id", "code");
            // return View();
            // truy vấn croped
            var list = await db.ImageCropeds.ToListAsync();
            foreach (var item in list)
            {
                // gọi về hình gốc 
                string path = Server.MapPath("~/Traning/data/" + item.Image.Name + "/" + item.Lever);
                var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + item.Image.Uri), path, item);
                if (newUrl != "")
                {
                    item.Uri = "/Traning/data/" + item.Image.Name + "/" + item.Lever + "/" + newUrl;
                    db.Entry(item).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
            // Crop(Image<Bgr, byte> img, int x, int y, int width, int height);
            return RedirectToAction("Index");
        }

        // GET: ImageCropeds/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
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

                    string path = Server.MapPath("~/Traning/data/" + imageCroped.Image.Name + "/" + imageCroped.Lever);
                    var newUrl = await GhiFileTraining.CutImageAsync(Server.MapPath("~" + imageCroped.Image.Uri), path, imageCroped);
                    if (newUrl != "")
                    {
                        imageCroped.Uri = "/Traning/data/" + imageCroped.Image.Name + "/" + imageCroped.Lever + "/" + newUrl;
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
                    var rootImage = new Image<Bgr, byte>(Server.MapPath("~" + editIteam.Image.Uri));
                    string path = Server.MapPath("~/Traning/data/" + editIteam.Image.Name);
                    string nameFile = editIteam.Image.Name + "-" + editIteam.Line.ToString("D2") + "-" + editIteam.Index.ToString("D2") + ".png";
                    ok = CropHelper.Save(CropHelper.Crop(rootImage, editIteam.X, editIteam.Y, editIteam.Width, editIteam.Height), path + "\\" + nameFile);
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
        public ActionResult WriteFile(long? AccountId)
        {
            //format: a01-000u-00-00| ok| 154| 408 768 27 51| AT| A
            return Write("abc@2018");
        }
        public ActionResult WriteByAllFile(long? AccountId)
        {
            //format: a01-000u-00-00| ok| 154| 408 768 27 51| AT| A
            return Write("abc@2018", "all");
        }
        public ActionResult WriteWord()
        {
            return View();
        }
        public ActionResult ImageAll(long? AccountId)
        {
            string Source = Server.MapPath("~/Traning/data");
            string auth = AccountId == null ? "1" : AccountId.Value.ToString();
            string target = Server.MapPath("~/Traning/Temp/" + auth);
            var r = AddZipFile(Source, target);
            if (r != "")
                return Json(new ExecuteResult() { Isok = true, Data = "/Traning/Temp/" + auth + "/" + r, Message = "Tạo file Thành công" });
            // return Json(new ExecuteResult() { Isok = true, Data = File(target+"\\"+ r,"zip"), Message = "Tạo file Thành công" });
            return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Không tạo được file" });
        }
        #endregion

        public JsonResult Write(string key, string type = "one")
        {
            try
            {
                //string keyEn = Commons.StringHelper.stringToSHA512(key);
                var dbKey = db.Khoas.FirstOrDefault().KeyValue;
                dbKey = string.IsNullOrEmpty(dbKey) == true ? "ẹc" : dbKey;
                if (key == dbKey)
                {

                    List<string> listFileName = new List<string>();
                    List<string> listLable = new List<string>();
                    // lấy list nhãn đã gán
                    var listCroped = db.ImageCropeds.ToList();
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

                    string comment = "#" + db.Khoas.FirstOrDefault().Description + " " + DateTime.Now.ToString() + "#";
                    string temp = "/TrainingFile/";
                    if (type.Equals("one"))
                        temp = "/TrainingFile/Thay/";
                    string word = "word.txt";
                    string pathFile = Path.Combine(temp, word);
                    string path = Server.MapPath("~" + pathFile);
                    if (!System.IO.File.Exists(path))
                    {
                        FileHelper.CreateFile(Server.MapPath("~" + temp), word, comment);
                    }
                    foreach (var item in listLable)
                    {
                        FileHelper.AppenAllText(path, "\n" + item);
                    }
                    return Json(new ExecuteResult() { Isok = true, Data = pathFile, Message = "Is ok" }, JsonRequestBehavior.AllowGet);

                }
                return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Key k đúng hoặc k có quyền ghi file" }, JsonRequestBehavior.AllowGet);
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
