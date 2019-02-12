using CropImage.Commons;
using CropImage.Handler.Crop;
using CropImage.Models;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                FileHelper.CreateFile(folder, fileName, text); return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message); return false;
            }
        }
        public static bool Ghi(string data)
        {

            return false;
        }
        // nhè ra fileName thôi
        public static async Task<string> CutImageAsync(string ImageRoot, string path, ImageCroped imageCroped,string fileNameTarget,string comment = null)
        {
            try
            {
                var rootImage = new Image<Bgr, byte>(ImageRoot);
                FileHelper.CreateFolderIfNotExist(path);

                // ghi file mô tả
                string data = string.IsNullOrEmpty(comment) ? "được cắt từ hình: " + ImageRoot : comment;
                if(!Directory.Exists(path+"\\Mota.txt"))
                await FileHelper.CreateFileAsync(path, "MoTa.txt", data);

                // hình mới tạo ra ghi đè luôn file đã có nếu thao tác là training lại với mỗi phần tử
                //string kieu = string.IsNullOrEmpty(imageCroped.Image.KieuChu) ? "00kieu" : imageCroped.Image.KieuChu;

                //string nameFile = imageCroped.Lever+"-"+ imageCroped.Image.Name+"-"+ kieu + "-" + imageCroped.Line.ToString("D2") + "-" + imageCroped.Index.ToString("D2") + ".png";
                //string nameFile = imageCroped.Image.Name+"-"+ kieu + "-" + imageCroped.Line.ToString("D2") + "-" + imageCroped.Index.ToString("D2") + ".png";
                string nameFile = fileNameTarget + "-" + imageCroped.Line.ToString("D2") + "-" + imageCroped.Index.ToString("D2") + ".png";
                bool ok = false;
                await Task.Run(() => {
                    ok = CropHelper.Save(CropHelper.Crop(rootImage, imageCroped.X, imageCroped.Y, imageCroped.Width, imageCroped.Height), path + "\\" + nameFile);
                });
                if (ok) return nameFile;
                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}