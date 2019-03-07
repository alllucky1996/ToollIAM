using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CropImage.Models.ViewModels
{
    public class ViewCrop
    {
        public bool IsOK { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Lable { get; set; }
        public string Uri { get; set; }
        public string Color { get; set; }
       
    }
}