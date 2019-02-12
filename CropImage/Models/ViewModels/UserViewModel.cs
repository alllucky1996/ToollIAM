using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CropImage.Models.ViewModels
{
    public class UserViewModel
    {
       
    }
    public class LogInModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
    public class RegisterModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
    }
    public class LoginModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}