using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CropImage.Models
{
    public class Training
    {
        [Key]
        public long Id { get; set; }
        public long? ImageId { get; set; }
        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }
        public long? ImageCropedId { get; set; }
        [ForeignKey("ImageCropedId")]
        public virtual ImageCroped ImageCroped { get; set; }

        public int GrayLever { get;set;}
        public int TrangThai { get; set; }
    }
   
}