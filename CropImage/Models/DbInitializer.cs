using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CropImage.Models
{
    public class DbInitializer: System.Data.Entity.DropCreateDatabaseIfModelChanges<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            var Daus = new List<Dau>();
            Daus.Add(new Dau() { Code = 1, Name = "Huyền", Description = "Dấu huyền" });
            Daus.Add(new Dau() { Code = 2, Name = "Sắc", Description = "Dấu sắc" });
            Daus.Add(new Dau() { Code = 3, Name = "Nặng", Description = "Dấu nặng" });
            Daus.Add(new Dau() { Code = 4, Name = "Hỏi", Description = "Dấu hỏi" });
            Daus.Add(new Dau() { Code = 5, Name = "Ngã", Description = "Dấu ngã" });
            Daus.ForEach(s => context.Daus.Add(s));
            context.SaveChanges();

            var loaiTu = new List<LoaiTu>();
            loaiTu.Add(new LoaiTu() { Code = "TL", Name = "Tag later", Description = "đánh dấu sau" });
            loaiTu.Add(new LoaiTu() { Code = "V", Name = "Động từ" });
            loaiTu.Add(new LoaiTu() { Code = "adj", Name = "Tính từ" });
            loaiTu.Add(new LoaiTu() { Code = "adv", Name = "Trạng từ" });
            loaiTu.ForEach(s => context.LoaiTus.Add(s));
            context.SaveChanges();
            var im = new List<Image>();
            im.Add(new Image() { Name = "1", Uri = "/Uploads/Images/Mau1.jpg", TrangThai = 0 });
            im.ForEach(s => context.Images.Add(s));

            context.SaveChanges();
            context.Khoas.Add(new Khoa() { KeyValue = "abc@2018", Description = "dũng tạo" });
        }
    }
}
