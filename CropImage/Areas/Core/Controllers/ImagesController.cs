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

namespace CropImage.Areas.Core.Controllers
{
    public class ImagesController : Controller
    {
        //// GET: Core/Images
        //public ActionResult Index()
        //{
        //    return Json("", JsonRequestBehavior.AllowGet);// View();
        //}
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
        }
        private DataContext db = new DataContext();
        

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
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HttpPostedFileBase[] fileS)
        {
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
                        item.Name = fileName;// ghi gile theo cấu trúc
                        item.Description = file.FileName;
                        item.TrangThai = 0;
                        item.Uri = fullFilePath;

                        db.Images.Add(item);
                        await db.SaveChangesAsync();

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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

                await db.SaveChangesAsync();
                // remove file sau

                //return RedirectToAction("Index");
                return Json(new ExecuteResult() { Isok = true, Data = image.Name });
            }
            catch (Exception ex)
            {
                return Json(new ExecuteResult() { Isok = true, Message = ex.Message, Data = null });
            }
        }


       
    }
}