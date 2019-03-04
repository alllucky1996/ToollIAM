using CropImage.Models.SysTem;
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
            context.TrangThais.Add(new TrangThai() { Code = 1, Name = "Chưa cắt" });
            context.TrangThais.Add(new TrangThai() { Code = 2, Name = "Đã cắt" });
            context.TrangThais.Add(new TrangThai() { Code = 3, Name = "Đã có dữ liễu" });
            context.SaveChanges();

            var loaiTu = new List<LoaiTu>();
            loaiTu.Add(new LoaiTu() { Code = "TL", Name = "Tag later", Description = "đánh dấu sau" });
            loaiTu.Add(new LoaiTu() { Code = "V", Name = "Động từ" });
            loaiTu.Add(new LoaiTu() { Code = "adj", Name = "Tính từ" });
            loaiTu.Add(new LoaiTu() { Code = "adv", Name = "Trạng từ" });
            loaiTu.ForEach(s => context.LoaiTus.Add(s));
            context.Accounts.Add(new Account() { FullName = "Phạm Thu Hà", UserName = "HaPT", PassWord = Commons.StringHelper.stringToSHA512("123456") });
            context.Accounts.Add(new Account() { FullName = "Nguyễn Anh Dũng", UserName = "alllucky", PassWord = Commons.StringHelper.stringToSHA512("123456"),Code = "admin" });
            context.Accounts.Add(new Account() { FullName = "Kim Văn Sáng", UserName = "sangkv", PassWord = Commons.StringHelper.stringToSHA512("654321") });
            context.Accounts.Add(new Account() { FullName = "Nguyễn Thị Thơm", UserName = "thomnt", PassWord = Commons.StringHelper.stringToSHA512("654321") });
            context.Accounts.Add(new Account() { FullName = "ND01", UserName = "user01", PassWord = Commons.StringHelper.stringToSHA512("654321") });
            context.Accounts.Add(new Account() { FullName = "ND02", UserName = "user02", PassWord = Commons.StringHelper.stringToSHA512("654321") });
            context.Accounts.Add(new Account() { FullName = "ND03", UserName = "user03", PassWord = Commons.StringHelper.stringToSHA512("654321") });
            context.Accounts.Add(new Account() { FullName = "ND04", UserName = "user04", PassWord = Commons.StringHelper.stringToSHA512("654321") });
            context.Accounts.Add(new Account() { FullName = "ND05", UserName = "user05", PassWord = Commons.StringHelper.stringToSHA512("654321") });
            context.SaveChanges();
            var im = new List<Image>();
            im.Add(new Image() { Name = "1", Uri = "/Uploads/Images/Mau1.jpg", MaTrangThai = 1,AccountId = 1});
            im.ForEach(s => context.Images.Add(s));

            context.SaveChanges();
            context.Khoas.Add(new Khoa() { KeyValue = "abc@2018", Description = "dũng tạo" });
            
            context.Logs.Add(new Log() {AccountId=0,Action="create",EntityName= "Account",NewValue="Phạm Thu Hà" });
            
            context.SaveChanges();
        }
    }
}
