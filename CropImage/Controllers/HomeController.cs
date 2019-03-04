using CropImage.Areas;
using CropImage.Commons;
using CropImage.Handler.Crop;
using CropImage.Models;
using CropImage.Models.SysTem;
using CropImage.Models.ViewModels;
using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CropImage.Controllers
{
    public class HomeController : BaseController
    {
        private LogHelper<ImageCroped> _log;

        public HomeController()
        {
            _log = new LogHelper<ImageCroped>(db);
        }

        private async Task<int> CreateLogAsync(string value)
        {
            return await _log.CreateAsync(accountId, value);
        }
        private async Task<int> CreateLogAsync(string value, string Mota = null)
        { 
            return await _log.CreateAsync(accountId, value, Mota);
        }
        private async Task<int> CreateLogAsync(ImageCroped img, string Mota = null)
        { 
            string value = JsonConvert.SerializeObject(img);
            return await _log.CreateAsync(accountId, value, Mota);
        }
        private async Task<int> CreateLogAsync(string value, string action, string Mota = null)
        {
           // var ac = Session[SessionEnum.AccountId] == null ? accountId : Session[SessionEnum.AccountId];
            return await _log.CreateAsync(accountId, value, action, Mota);
        }
        public ActionResult Index()
        {
            return RedirectToAction("Index", "CoreHome", new { area = "Core" });
           // return Redirect("/Login");
        }

        #region old 

        //   private DataContext db = new DataContext();
        private string PreViewImage = "~/TempImage/";
        private int widthImage = 0;
        private int heightImage = 0;
        #region get 
        //void baseView()
        //{
        //    var img = db.Images.Where(o=>o.AccountId == accountId).FirstOrDefault();

        //    ViewBag.Image = img.Uri == null ? "/Uploads/Images/Mau1.jpg" : img.Uri;
        //    ViewBag.idImage = 1;
        //    int h;
        //    string link = img.Uri == null ? "~/Uploads/Images/Mau1.jpg" : "~" + img.Uri;
        //    ViewBag.widthImage = CropHelper.WidthImage(Server.MapPath(link), out h);
        //    ViewBag.heightImage = h;
        //    ViewBag.PreViewImage = "/TempImage/tempImages.jpg";
        //    #region drop
        //    var listDau = db.Daus;
        //    ViewBag.IdDau = new SelectList(listDau, "Code", "Name");
        //    var listLoaiTu = db.LoaiTus;
        //    ViewBag.IdLoaiTu = new SelectList(listLoaiTu, "Code", "Name");
        //    #endregion
        //}
        void baseView(Image img)
        {
            ViewBag.Image = img.Uri;
            ViewBag.idImage = img.Id;
            int h;
            ViewBag.widthImage = CropHelper.WidthImage(Server.MapPath("~" + img.Uri), out h);
            ViewBag.heightImage = h;
            #region drop
            var listDau = db.Daus;
            ViewBag.IdDau = new SelectList(listDau, "Code", "Name");
            var listLoaiTu = db.LoaiTus;
            ViewBag.IdLoaiTu = new SelectList(listLoaiTu, "Code", "Name");
            #endregion
        }
         
        public async Task<ActionResult> Pre(long id)
        {
            if (accountId == -1) return Redirect("/Login/Index"); 
            var firstId = db.Images.FirstOrDefault(o => o.AccountId == accountId).Id;
            Image img = null;
            string error = "";
            if ( id == firstId || id > firstId)
            {
                img = db.Images.Where(o=>o.AccountId == accountId && o.Id == (id - 1)).FirstOrDefault();
               
                while (img == null)
                {
                    id--;
                    img = db.Images.Where(o => o.AccountId == accountId && o.Id == (id)).FirstOrDefault();
                    //  img = await db.Images.FindAsync(id);
                }
            }
            else
            {
                img = await db.Images.FindAsync(id);
                error = "Không có ảnh trước đó";
            }
            ViewBag.Error = error;
            baseView(img);
             await CreateLogAsync("Đang làm dữ liệu đến" + img.Id);
            #region drop
            var listDau = db.Daus;
            ViewBag.Dau = new SelectList(listDau, "Code", "Name");
            var listLoaiTu = db.LoaiTus;
            ViewBag.LoaiTu = new SelectList(listLoaiTu, "Code", "Name");
            #endregion
            return View();
            // return Json(new ExecuteResult() { Isok = true, Data = img}, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Next(long id)
        {
            if (accountId == -1) return Redirect("/Login/Index"); 
            var lastImg = db.Images.Where(o=>o.AccountId == accountId).OrderByDescending(u => u.Id).FirstOrDefault();
            string error = "";
            Image img;
            if (id < lastImg.Id)
            {
                img = db.Images.Where(o => o.AccountId == accountId && o.Id == (id + 1)).FirstOrDefault();
                // img = await db.Images.FindAsync(id + 1 );
                while (img == null)
                {
                    id++;
                    img = db.Images.Where(o => o.AccountId == accountId && o.Id == (id)).FirstOrDefault();
                    // img = await db.Images.FindAsync(id);
                }
                widthImage = CropHelper.WidthImage(Server.MapPath("~" + img.Uri), out heightImage);

            }
            else
            {
                img = lastImg;
                error = "Không còn ảnh tiếp theo";
            }
            ViewBag.Error = error;
            baseView(img);
            await CreateLogAsync("Đang làm dữ liệu đến" + img.Id);
            return PartialView();// View();
            //if (error != "")
            //    return Json(new { Isok = false, Error= error}, JsonRequestBehavior.AllowGet);
            //return Json(new { Isok = true, Data = img.Uri, objectId = img.Id, Width=widthImage, Height= heightImage}, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ThisIsOk(long id)
        {
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));
            try
            {

                var item = await db.Images.FindAsync(id);
                item.MaTrangThai = 2;
                db.Entry(item).State = EntityState.Modified;
                var r = await db.SaveChangesAsync();
                await CreateLogAsync("Cắt đến hình: " + id);
                // next
                var lastImg = db.Images.Where(o => o.AccountId == accountId).OrderByDescending(u => u.Id).FirstOrDefault();
                string error = "";
                Image img;
                if (id < lastImg.Id)
                {
                    img = db.Images.Where(o => o.AccountId == accountId && o.Id == (id + 1)).FirstOrDefault();
                    // img = await db.Images.FindAsync(id + 1 );
                    while (img == null)
                    {
                        id++;
                        img = db.Images.Where(o => o.AccountId == accountId && o.Id == (id)).FirstOrDefault();
                        // img = await db.Images.FindAsync(id);
                    }
                    widthImage = CropHelper.WidthImage(Server.MapPath("~" + img.Uri), out heightImage);

                }
                else
                {
                    img = lastImg;
                    error = "Không còn ảnh tiếp theo";
                }
                ViewBag.Error = error;
                baseView(img);
                await CreateLogAsync("Đang làm dữ liệu đến" + img.Id);
                return PartialView();// View();
            }
            catch (Exception)
            {
                ViewBag.Error = "lỗi hoàn thành hình";
                var img = await db.Images.FindAsync(id);
                baseView(img);
                await CreateLogAsync("lỗi hoàn thành hình" + id);
                return PartialView();// View();
            }
           


        }
        public ActionResult Cau()
        {
            //  baseView();
            if (accountId == -1) return Redirect("/Login/Index"); 
            return PartialView();
        }
        public ActionResult Tu()
        {
            // baseView();
            if (accountId == -1) return Redirect("/Login/Index"); 
            #region drop
            var listDau = db.Daus;
            ViewBag.IdDau = new SelectList(listDau, "Code", "Name");
            var listLoaiTu = db.LoaiTus;
            ViewBag.IdLoaiTu = new SelectList(listLoaiTu, "Code", "Name");
            #endregion
            return PartialView();
        }
        public ActionResult AmTiet()
        {
            //  baseView();
            if (accountId == -1) return Redirect("/Login/Index"); 
            #region drop
            var listDau = db.Daus;
            ViewBag.IdDau = new SelectList(listDau, "Code", "Name");
            var listTuLoai = db.LoaiTus;
            ViewBag.IdTuLoai = new SelectList(listTuLoai, "Code", "Name");
            #endregion
            return PartialView();
        }
        public ActionResult Chu()
        {
            // baseView();
            if (accountId == -1) return Redirect("/Login/Index"); 
            #region drop
            var listDau = db.Daus;
            ViewBag.IdDau = new SelectList(listDau, "Code", "Name");
            #endregion
            return PartialView();
        }
        #endregion
        #region post
        [HttpPost]
        public async Task<ActionResult> CropCau(ImageCroped model, long idImage)
        {
            if (accountId == -1) return Redirect("/Login/Index"); 
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.X == model.Y && model.Width == model.Height && model.Height == 0 && model.Y == 0) Json(new ExecuteResult() { Isok = false, Data = null, Message = "Crop k hợp lệ" });
                    var croped = new ImageCroped();

                    croped = model;
                    croped.Lable = model.Lable.Trim().Replace("  ", " ");
                    croped.ImageId = idImage;
                    db.ImageCropeds.Add(croped);
                    await db.SaveChangesAsync();
                    await CreateLogAsync(croped);

                    // cắt hình && show
                    var img = db.Images.Find(idImage);
                    if (img != null)
                    {
                        string pat = FileHelper.GetRunningPath();
                        string ax = Server.MapPath("~/" + img.Uri);
                        var rootImage = new Image<Bgr, byte>(ax);
                        // get Id insered 
                        db.Entry(croped).GetDatabaseValues();
                        long i = croped.Id;
                        PreViewImage = PreViewImage + idImage + "_" + i + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + ".jpg";
                        var ok = CropHelper.Save(CropHelper.Crop(rootImage, model.X, model.Y, model.Width, model.Height), Server.MapPath(PreViewImage));
                        if (!ok)
                        {
                            return Json(new ExecuteResult() { Isok = false, Data = PreViewImage.Replace("~", ""), Message = "Lưu đươc vào db nhưng lỗi trong quá trình xử lý ảnh", PreVeiwImage = PreViewImage.Replace("~", "") });
                        }
                    }

                    return Json(new ExecuteResult() { Isok = true, Data = PreViewImage.Replace("~", ""), Message = "Saved", PreVeiwImage = PreViewImage.Replace("~", "") });
                }
                catch (Exception ex)
                {
                    return Json(new ExecuteResult() { Isok = false, Data = null, Message = ex.Message });
                }
            }
            return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Hãy nhập đủ thông tin" });

        }
        [HttpPost]
        public async Task<ActionResult> CropTu(ImageCroped model, long idImage)
        {
            if (accountId == -1) return Redirect("/Login/Index"); 
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.X == model.Y && model.Width == model.Height && model.Height == 0 && model.Y == 0) Json(new ExecuteResult() { Isok = false, Data = null, Message = "Crop k hợp lệ" });
                    var croped = new ImageCroped();
                    croped.Lever = 2;
                    croped = model;
                    croped.Lable = model.Lable.Trim().Replace("  ", " ");
                    croped.ImageId = idImage;
                    db.ImageCropeds.Add(croped);
                    await db.SaveChangesAsync();
                    await CreateLogAsync(croped);

                    //
                    // cắt hình && show nếu là từ ghép 
                    var c = model.Lable.Trim().Replace("  ", " ").Split(' ').Length;
                    if (c > 1)
                    {
                        var img = db.Images.Find(idImage);
                        if (img != null)
                        {
                            string pat = FileHelper.GetRunningPath();
                            string ax = Server.MapPath("~/" + img.Uri);
                            var rootImage = new Image<Bgr, byte>(ax);
                            // get Id insered 
                            db.Entry(croped).GetDatabaseValues();
                            long i = croped.Id;
                            PreViewImage = PreViewImage + idImage + "_" + i + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + ".jpg";
                            var ok = CropHelper.Save(CropHelper.Crop(rootImage, model.X, model.Y, model.Width, model.Height), Server.MapPath(PreViewImage));
                            if (!ok)
                            {
                                return Json(new ExecuteResult() { Isok = false, Data = "", Message = "Lưu đươc vào db nhưng lỗi trong quá trình xử lý ảnh", PreVeiwImage = PreViewImage.Replace("~", "") });
                            }
                            return Json(new ExecuteResult() { Isok = true, Data = PreViewImage.Replace("~", ""), Message = "Save crop is ok", PreVeiwImage = PreViewImage.Replace("~", "") });
                        }
                    }
                    return Json(new ExecuteResult() { Isok = true, Data = null, Message = "Saved", PreVeiwImage = PreViewImage.Replace("~", "") });
                }
                catch (Exception ex)
                {
                    return Json(new ExecuteResult() { Isok = false, Data = "", Message = ex.Message });
                }
            }
            return Json(new ExecuteResult() { Isok = false, Data = "", Message = "Hãy nhập đủ thông tin" });

        }
        [HttpPost]
        public async Task<ActionResult> CropAmTiet(ImageCroped model, long idImage)
        {
            if (accountId == -1) return Redirect("/Login/Index"); 
            if (ModelState.IsValid)
            {
                model.Lable = model.Lable.Trim();
                model.LoaiTu = "TL";
                // nhầm mức tự động chuyển hay báo ra 
                if (model.Lable.Replace("  ", " ").Split(' ').Count() > 1)
                {
                    // goto Tu??0
                    return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Nhầm mức " });
                }
                try
                {
                    var croped = new ImageCroped() {
                        Lever = 3,
                   // croped = model,
                    Lable = model.Lable.Trim().Replace(" ", ""),
                    ImageId = idImage,
                   Description = model.Description,
                   Dau = model.Dau,
                   IsOK = model.IsOK,
                   X = model.X,
                   Y = model.Y,
                   Width = model.Width,
                   Height = model.Height,
                   Index = model.Index,
                   Name = model.Name,
                   Line = model.Line,
                   LoaiTu = model.LoaiTu,
                   
                   
                };
                    
                    db.ImageCropeds.Add(croped);
                    await db.SaveChangesAsync();
                    await CreateLogAsync(croped);
                    return Json(new ExecuteResult() { Isok = true, Data = 1, Message = "Saved" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new ExecuteResult() { Isok = false, Data = null, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Hãy nhập đủ thông tin" }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public async Task<ActionResult> CropChuOrDau(ImageCroped model, long idImage)
        {
            if (accountId == -1) return Redirect("/Login/Index"); 
            try
            {
                if (model.IdDau != null)
                {
                    model.Description = model.Lable;
                    var l = await db.Daus.FindAsync(model.IdDau);
                    model.Lable = l.Name;
                }
                if (model.Lable.Replace("  ", " ").Split(' ').Count() > 1)
                {
                    // goto Tu??0
                    return Json(new ExecuteResult() { Isok = false, Data = null, Message = "Nhầm mức " });
                }
                try
                {
                    var croped = new ImageCroped();
                    croped.Lever = 4;
                    croped = model;
                    croped.Lable = model.Lable.Trim().Replace("  ", " ");
                    croped.ImageId = idImage;
                    db.ImageCropeds.Add(croped);
                    await db.SaveChangesAsync();
                    await CreateLogAsync(croped.ToString());
                    return Json(new ExecuteResult() { Isok = true, Data = 1, Message = "Saved" });
                }
                catch (Exception ex)
                {
                    return Json(new ExecuteResult() { Isok = false, Data = null, Message = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new ExecuteResult() { Isok = false, Data = null, Message = ex.Message });
            }



        }






        #endregion

        #endregion

    }
}