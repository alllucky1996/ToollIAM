using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CropImage.Models.ViewModels
{
    public class ImageUpload
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
        public string KieuChu { get; set; }
    }
}