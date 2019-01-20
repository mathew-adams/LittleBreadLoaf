using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace littlebreadloaf
{
    public class ImageHelper
    {
        private readonly string _productID;
        
        public ImageHelper(string ProductID)
        {
            _productID = ProductID;
        }

        public void BuildDirectory(int size)
        {
            string productDirectory = ProductDirectory();

            if (!Directory.Exists(productDirectory))
            {
                Directory.CreateDirectory(productDirectory);
            }
        }

        public bool HasDirectory()
        {
            return Directory.Exists(ProductDirectory());
        }

        public string ProductDirectory()
        {
            return Path.Combine(Environment.CurrentDirectory,
                                "wwwroot",
                                "images",
                                _productID);
        }

        public string GetNewFileName(int size, string ProductImageID)
        {
            return Path.ChangeExtension(Path.Combine(ProductDirectory(), String.Concat(ProductImageID,"_",size)),".png");
        }

        public string GetDisplayFileName(string ProductImageID)
        {
            return string.Concat("/images/", _productID, "/", ProductImageID,"_{1}.png");
        }

        public void DeleteAll()
        {
            var di = new DirectoryInfo(ProductDirectory());
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

        public void DeleteImage(string ProductImageID)
        {
            var di = new DirectoryInfo(ProductDirectory());
            foreach(var file in di.GetFiles().Where(f=>f.Name.StartsWith(ProductImageID)))
            {
                if(file.Exists)
                {
                    file.Delete();
                }
            }
        }

        public void AddImages(string productImageID,
                              int[] sizes,
                              IFormFile upload)
        {
            foreach(var size in sizes)
            {
                SaveImage(productImageID, size, upload);
            }
        }

        private void SaveImage(string productImageID,
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
                        
                        var imgHelper = new ImageHelper(_productID);
                        imgHelper.BuildDirectory(size);
                        string fileName = imgHelper.GetNewFileName(size, productImageID);
                        newImg.Save(fileName, ImageFormat.Jpeg);
                    }
                }
            }
        }
    }
}
