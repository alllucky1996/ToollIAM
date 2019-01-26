using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CropImage.Models.ViewModels
{
    public class CropInfo
    {
        public bool IsOK { get; set; }
        public int GrayLever { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Lable { get; set; }
        public string Info { get {
                string ok = IsOK == true ? "ok" : "err";
                // gray
                string Diem= X.ToString()+" "+Y.ToString();
                string wh = Width.ToString() + " " + Height.ToString();
                return ok+" "+"255"+" "+Diem+" "+wh+" "+"TL"+" " + Lable.Trim();
         } }
    }
}