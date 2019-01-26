using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace CropImage.Models
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DataContext")
        {
        }

        public DbSet<Dau> Daus { get; set; }
        public DbSet<LoaiTu> LoaiTus { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ImageCroped> ImageCropeds { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<Khoa> Khoas { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           // modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

           //Không khởi tạo data, tạo, tạo lại bảng
            //Database.SetInitializer<DataContext>(null);
            //Có khởi tạo data, tạo, tạo lại bảng khi không tồn tại
            Database.SetInitializer<DataContext>(new DropCreateDatabaseIfModelChanges<DataContext>());
            Database.SetInitializer<DataContext>(new DropCreateDatabaseAlways<DataContext>());
            //Bỏ tùy chọn chuyển tên bảng thành dạng số nhiều khi đọc trong CSDL
           

   
            //Tạo DbSet
            //ConfigureModel(modelBuilder);
            this.Configuration.LazyLoadingEnabled = true;
            base.OnModelCreating(modelBuilder);
        }
    }
}