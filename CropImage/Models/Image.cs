using CropImage.Models.SysTem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CropImage.Models
{
    public class Image
    {
        [Key]
        public long Id { get; set; }
        public string code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Uri { get; set; }
        // sau sửa thành khóa ngoại nối đến người dùng để lấy tên ng đó vào đây
        public string KieuChu { get; set; }
        //0: chưa crop
        //1: đang crop
        //2: đã crop
        // dùng khóa ngoại chuyển trạng thái sau
        public int MaTrangThai { get; set; }
        [ForeignKey("MaTrangThai")]
        public virtual TrangThai TrangThai { get; set; }
        public long AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Accounts { get; set; }
        public Image()
        {
            MaTrangThai = 0;
        }
        public virtual ICollection<ImageCroped> ListCroped { get; set; }
    }
   
}