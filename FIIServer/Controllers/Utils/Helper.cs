using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static System.String;


namespace FIIServer.Controllers.Utils
{
    public class Helper
    {
        public static Image ConvertBase64ToImage(string base64) => (Bitmap)new ImageConverter().ConvertFrom(Convert.FromBase64String(base64));

        public static bool SaveImage(Image image, string filepath, ImageFormat imageFormat)
        {
            try
            {
                image.Save(filepath + ".png", imageFormat);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteImageByPath(string path)
        {
            var file = new FileInfo(path);
            if (!file.Exists) return false;
            file.Delete();
            return true;
        }

        public static ImageFormat ReturnImageFormat(string imageFormat)
        {
            if (IsNullOrEmpty(imageFormat) || imageFormat.ToLower().Equals("png"))
            {
                return ImageFormat.Png;
            }

            if (imageFormat.ToLower().Equals("jpg") || imageFormat.ToLower().Equals("jpeg"))
            {
                return ImageFormat.Jpeg;
            }

            return ImageFormat.Png;
        }
    }
}
