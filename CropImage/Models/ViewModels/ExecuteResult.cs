using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CropImage.Models.ViewModels
{
    public class ExecuteResult
    {
        public bool Isok { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
        public string PreVeiwImage { get; set; }
        public bool? IsWarning { get; set; }
    }
}