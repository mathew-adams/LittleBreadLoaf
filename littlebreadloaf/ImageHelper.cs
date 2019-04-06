using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace littlebreadloaf
{
    public class ImageHelper
    {
        private readonly string _sourceID;

        #region "Public Functions - Image Upload"
       
        public ImageHelper(string sourceID)
        {
            _sourceID = sourceID;
        }

        public void BuildDirectory(int size)
        {
            string sourceDirectory = SourceDirectory();

            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }
        }

        public bool HasDirectory()
        {
            return Directory.Exists(SourceDirectory());
        }

        public string SourceDirectory()
        {
            return Path.Combine(Environment.CurrentDirectory,
                                "wwwroot",
                                "images",
                                _sourceID);
        }

        public string GetNewFileName(int size, string SourceImageID)
        {
            return Path.ChangeExtension(Path.Combine(SourceDirectory(), String.Concat(SourceImageID,"_",size)),".png");
        }

        public string GetDisplayFileName(string SourceImageID)
        {
            return string.Concat("/images/", _sourceID, "/", SourceImageID,"_{1}.png");
        }

        public void DeleteAll()
        {
            var di = new DirectoryInfo(SourceDirectory());
            if(di.Exists)
            {
                foreach (var file in di.GetFiles())
                {
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                di.Delete();
            }            
        }

        public void DeleteImage(string sourceImageID)
        {
            var di = new DirectoryInfo(SourceDirectory());
            foreach(var file in di.GetFiles().Where(f=>f.Name.StartsWith(sourceImageID)))
            {
                if(file.Exists)
                {
                    file.Delete();
                }
            }
        }

        public void AddImages(string sourceImageID,
                              int[] sizes,
                              IFormFile upload)
        {
            foreach(var size in sizes)
            {
                SaveImage(sourceImageID, size, upload);
            }
        }
        #endregion

        #region "Private Functions"

        private void SaveImage(string souceImageID,
                               int size, 
                               IFormFile upload)
        {
            using (var stmImage = new MemoryStream())
            {
                upload.CopyTo(stmImage);

                using (var img = Image.FromStream(stmImage))
                {
                    using (var newImg = new Bitmap(img, size, size))
                    {
                        using (var graphics = Graphics.FromImage(newImg))
                        {
                            graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, size, size);
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            int t = 0, l = 0;
                            if (img.Height > img.Width)
                                t = (img.Height - img.Width) / 2;
                            else
                                l = (img.Width - img.Height) / 2;
                            graphics.DrawImage(img,
                                               new Rectangle(0, 0, size, size),
                                               new Rectangle(l, t, img.Width - l * 2, img.Height - t * 2),
                                               GraphicsUnit.Pixel);
                        }
                        
                        var imgHelper = new ImageHelper(_sourceID);
                        imgHelper.BuildDirectory(size);
                        string fileName = imgHelper.GetNewFileName(size, souceImageID);
                        newImg.Save(fileName, ImageFormat.Jpeg);
                    }
                }
            }
        }
        #endregion

    }
}
