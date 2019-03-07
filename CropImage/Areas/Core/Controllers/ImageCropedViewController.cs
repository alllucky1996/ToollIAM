using CropImage.Controllers;
using CropImage.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    }
}