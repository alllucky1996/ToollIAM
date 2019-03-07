using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CropImage.Models.ViewModels.ThongKe
{
    /// <summary>
    /// Thống kê theo thờ gian
    /// </summary>
    public class ThongKeTG<T> where T : class, new()
    {

    }
    /// <summary>
    /// Thống kê theo đối tượng
    /// </summary>
    public class ThongKeObject<T> where T:class, new ()
    {

    }
    /// <summary>
    /// Thống kê thwo trạng thái
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThongKeTrangThai<T> where T : class, new()
    {

    }
    /// <summary>
    /// Thống kê theo lịch sử hệ thống
    /// </summary>
    /// <typeparam name="T">Tên các thực thể </typeparam>
    public class ThongKeOfLog<T> where T : class, new()
    {

    }
    /// <summary>
    /// Thống kê người dùng
    /// </summary>
    /// <typeparam name="T">Tên các thực thể </typeparam>
    public class ThongKeUser
    {
        
    }
    public class TongQuan
    {
        [Display(Name ="Người làm")]
        public long User { get; set; }
        public long UserOnline { get; set; }


        [Display(Name = "Hình ảnh đã cắt ra")]
        public long ImageCroped { get; set; }
        [Display(Name = "Hình ảnh gốc")]
        public long Image { get; set; }
        [Display(Name = "Ảnh đang cắt")]
        public long ImageImageCroping { get; set; }
    }
}