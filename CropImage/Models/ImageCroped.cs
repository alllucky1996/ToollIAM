using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CropImage.Models
{
    public class ImageCroped
    {
        [Key]
        public long Id { get; set; }
        [Display(Name = "Crop từ hình ảnh")]
        public long? ImageId { get; set; }
        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }
        public string code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name ="Góc trái X")]
        public int X { get; set; }
        [Display(Name = "Góc trái y")]
        public int Y { get; set; }
        [Display(Name = "Độ rộng")]
        public int Width { get; set; }
        [Display(Name = "Chiều cao")]
        public int Height { get; set; }
        public string LoaiTu { get; set; }
       
        [Required(ErrorMessage = "Không được để trống")]
        public string Lable { get; set; }
        public int? IdDau { get; set; }
        [ForeignKey("IdDau")]
        public virtual Dau Dau { get; set; }
        [Display(Name = "Mẫu này đúng hay sai")]
        public bool IsOK { get; set; }
        public DateTime CreateDate { get; set; }
        [Display(Name = "Đường dẫn đến file")]
        public string Uri { get; set; }
        [Display(Name = "Dòng số")]
        public int Line { get; set; }
        [Display(Name = "Vị trí số")]
        public int Index { get; set; }

        // chưa chạy
        [NotMapped]
        public string NameFile { get {
                if (Image != null)
                {

                }
                return "";
            } }
        public ImageCroped()
        {
            CreateDate = DateTime.Now;
        }
        public string Info
        {
            get
            {
                string ok = IsOK == true ? "ok" : "err";
                // gray
                string Diem = X.ToString() + " " + Y.ToString();
                string wh = Width.ToString() + " " + Height.ToString();
                return ok + " " + "255" + " " + Diem + " " + wh + " " + "TL" + " " + Lable.Trim();
            }
        }
    }
   
}