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
                    var rootImage = new Image<Bgr, byte>(Server.MapPath("~" + imageCroped.Image.Uri));
                    string path = Server.MapPath("~/Traning/data/" + imageCroped.Image.Name);
                    FileHelper.CreateFolderIfNotExist(path);
                    if (1 == 1)
                    {
                        // ghi file mô tả
                        string data = "được cắt từ hình: id: " + imageCroped.Image.Id + "; tên" + Path.GetFileNameWithoutExtension(imageCroped.Image.Uri) + "ở : " + Path.GetFullPath(imageCroped.Image.Uri);
                        await FileHelper.CreateFileAsync(path, "MoTa.txt", data);
                    }

                    // hình mới tạo ra ghi đè luôn file đã có nếu thao tác là training lại với mỗi phần tử
                    string nameFile = imageCroped.Image.Name + "-" + imageCroped.Line.ToString("D2") + "-" + imageCroped.Index.ToString("D2") + ".png";
                    var ok = CropHelper.Save(CropHelper.Crop(rootImage, imageCroped.X, imageCroped.Y, imageCroped.Width, imageCroped.Height), path + "\\" + nameFile);
                    if (ok)
                    {
                        imageCroped.Uri = "/Traning/data/" + imageCroped.Image.Name + "/" + nameFile;
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
                editIteam.Index = imageCroped.Index;
                editIteam.Line = imageCroped.Line;
                editIteam.Lable = imageCroped.Lable;
                editIteam.IsOK = imageCroped.IsOK;

                db.Entry(editIteam).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
        public ActionResult WriteFile(string key)
        {
            //format: a01-000u-00-00| ok| 154| 408 768 27 51| AT| A
            // get key
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
                        string nameImage = db.Images.Find(crop.ImageId).Name;
                        // dùng với hình đã cắt
                        //   string nameFile = crop.Image.Name + "-" + crop.Line + "-" + crop.Index;
                        // nhãn 
                        listLable.Add(nameImage + " " + crop.Info);
                    }

                    string comment = "#NGuoiTao_NguyenAnhDung" + DateTime.Now.ToString() + "#";
                    string temp = "/TrainingFile/";
                    string word = "word.txt";
                    string pathFile = Path.Combine(temp, word);
                    string path = Server.MapPath("~" + pathFile);
                    if (!System.IO.File.Exists(path))
                    {
                        FileHelper.CreateFile(path, comment);
                    }
                    foreach (var item in listLable)
                    {
                        FileHelper.AppenAllText(path, "\n" + item);
                    }
                    return Json(new ExecuteResult() { Isok = true, Data = "path", Message = "Is ok" }, JsonRequestBehavior.AllowGet);

                }
                return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Key k đúng hoặc k có quyền ghi file" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ExecuteResult() { Isok = false, Data = null, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        
    }
}
