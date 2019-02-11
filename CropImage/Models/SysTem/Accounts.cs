using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CropImage.Models.SysTem
{
    public class Accounts
    {
        [Key]
        public long id { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PassWord { get; set; }
        public DateTime CreateDate { get; set; }
        public bool? IsDelete { get; set; }
    }
    
}