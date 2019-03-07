using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CropImage.Models.SysTem
{
    public class Account
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PassWord { get; set; }
        public string Picture { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsOnline { get; set; }
        public Account()
        {
            this.CreateDate = DateTime.Now;
            this.IsDelete = false;
            this.IsOnline = false;
        }
        public virtual ICollection<Image> ListImg { get; set; }

    }
    
}