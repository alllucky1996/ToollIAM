using CropImage.Commons;
using CropImage.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace CropImage.Handler
{
    public class GhiFileTraining
    {
       // private DataContext db = new DataContext();
        public ImageCroped ImageTraining { get; set; }
        public string FileName { get; set; }
        public bool InitFile(string folder, string fileName, string text)
        {
            try
            {
                FileHelper.CreateFile(folder, fileName, text);return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);return false;
            }
        }
        public static bool Ghi(string data)
        {

            return false;
        }
    }
}