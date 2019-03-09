using CropImage.Controllers;
using CropImage.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CropImage.Areas.Core.Controllers
{
    public class ImageCropedViewController : BaseController
    {
        // GET: Core/ImageCropedView
        public async Task<ActionResult> Index(long? id)
        {
            if (id == null) return Redirect("/Core/CoreImage/Index");
            var item = await db.ImageCropeds.FindAsync(id);
            var positions = new List<ViewCrop>();

            positions.Add(new ViewCrop()
            {
                X = item.X,
                Y = item.Y,
                Width = item.Width,
                Height = item.Height,
                IsOK = item.IsOK,
                Lable = item.Lable,
                Uri = item.Uri
            });
            ViewBag.Positions = JsonConvert.SerializeObject(positions);
            return View(item);
        }

        public async Task<ActionResult> Index2(long? id)
        {
            if (id == null) return Redirect("/Core/CoreImage/Index");
            var img = await db.Images.FindAsync(id);
            var positions = new List<ViewCrop>();
            var items = img.ListCroped;
            foreach (var item in items)
            {
                positions.Add(new ViewCrop()
                {
                    X = item.X,
                    Y = item.Y,
                    Width = item.Width,
                    Height = item.Height,
                    IsOK = item.IsOK,
                    Lable = item.Lable,
                    Uri = item.Uri
                });
               
            }
             ViewBag.Positions = JsonConvert.SerializeObject(positions);
            ViewBag.Image = img.Uri;
            Bitmap bm = new Bitmap(Server.MapPath("~"+ img.Uri));
            ViewBag.W = bm.Width;
            ViewBag.H = bm.Height;

            return View();
        }
        public async Task<ActionResult> ListInRoot(long? id)
        {
            //if (id == null) id = 2; // return Redirect("/Core/Images/Index");
            //var item = await db.Images.FindAsync(id);
            //var ListCroped = await db.ImageCropeds.Where(o => o.ImageId == id).ToArrayAsync();
            //ViewBag.Positions = ListCroped;
            //ViewBag.Img = item.Uri;
            //return View();
            return await Index2(id);
        }
    }
}