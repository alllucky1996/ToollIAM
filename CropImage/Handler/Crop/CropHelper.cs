using CropImage.Commons;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CropImage.Handler.Crop
{
    public class CropHelper
    {
        // public Image<Bgr, byte> MainImage { get; set; }
        //public CropHelper() { }

        //public CropHelper(string uriImage)
        //{
        //    MainImage = new Image<Bgr, byte>(uriImage);
        //}
        //public CropHelper(Mat matImg)
        //{
        //    MainImage = matImg.ToImage<Bgr, byte>();
        //}
        public static int WidthImage(string uri, out int Height)
        {
            var any = System.IO.Directory.Exists(uri);
            if (!any)
            {
                Height = 0;
                return 0;
            }
            var temp = new Image<Gray, byte>(uri);
            Height = temp.Height;
            return temp.Width;
        }
        public static Image<Bgr, byte> Crop(Image<Bgr, byte> img, int x, int y, int width, int height)
        {
            return Crop(img, new Rectangle(x, y, width, height));
        }
        public static async Task<string> SaveCropAsync(string img, int x, int y, int width, int height, string fileName)
        {
            try
            {
                await Task.Run(() =>
                {
                    Bitmap source = new Bitmap(img);
                    Bitmap CroppedImage = source.Clone(new Rectangle(x, y, width, height), source.PixelFormat);
                    CroppedImage.Save(fileName);
                });
                return fileName;
            }
            catch (Exception ex)
            {
                return "[ERROR]: 0xD00 '" + ex.Message+"'";

            }

        }
        public static Image<Bgr, byte> Crop(Image<Bgr, byte> img, Rectangle rectangle)
        {
            //Mat mat = new Mat();
            // mat = img.Mat;
            return new Mat(img.Mat, rectangle).ToImage<Bgr, byte>();
        }
        public static Bitmap CropToBitmap(Image<Bgr, byte> img, Rectangle rectangle)
        {
            return new Mat(img.Mat, rectangle).Bitmap;
        }

        // save return ok? true: false
        public static bool Save(Image<Bgr, byte> MainImage, string fileName)
        {
            try
            {
                Bitmap bm = new Bitmap(MainImage.ToBitmap());
                bm.Save(fileName);
                // MainImage.Save(fileName);
                return true;
            }
            catch
            {
                return false;

            }
        }
        public static bool Save(Bitmap MainImage, string fileName)
        {
            try
            {
                MainImage.Save(fileName);
                return true;
            }
            catch
            {
                return false;

            }
        }
        // error = save ok? null: ex
        public static void Save(Image<Bgr, byte> MainImage, string fileName, out string error)
        {
            try
            {
                //MainImage.Save(fileName);
                Bitmap bm = new Bitmap(MainImage.ToBitmap());
                bm.Save(fileName);
                error = null;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }
        }
        public static bool CropAndSave(Image<Bgr, byte> img, Rectangle rectangle, out string fileName)
        {
            try
            {
                var image = Crop(img, rectangle);
                bool x = Save(image, "");
                fileName = "";
                return true;
            }
            catch (Exception)
            {
                fileName = string.Empty;
                return false;
            }
        }
    }
}