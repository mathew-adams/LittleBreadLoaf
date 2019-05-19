using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace littlebreadloaf
{
    public class ImageHelper
    {
        
        private readonly string _sourceID;

        public enum ImageResizeMode
        {
            Square = 0,
            Banner = 1,
            AspectRatio = 2
        }

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

        public void AddImages(ImageResizeMode mode,
                              string sourceImageID,
                              Size[] sizes,
                              IFormFile upload)
        {

            foreach (var size in sizes)
            {
                SaveImage(mode, sourceImageID, size, upload);
            }
        }
        #endregion

        #region "Private Functions"

        private void SaveImage(ImageResizeMode mode,
                               string souceImageID,
                               Size size, 
                               IFormFile upload)
        {
            Rectangle destRect;
            Rectangle srcRect;
            int width;
            int height;

            using (var stmImage = new MemoryStream())
            {
                upload.CopyTo(stmImage);

                using (var imgRaw = Image.FromStream(stmImage))
                {

                    switch (mode)
                    {
                        case ImageResizeMode.Banner: //sizes are known
                            destRect = new Rectangle(0, 0, size.Width, size.Height);
                            srcRect = new Rectangle(0, 0, imgRaw.Width, imgRaw.Height);
                            width = size.Width;
                            height = size.Width;
                            break;
                        case ImageResizeMode.AspectRatio:
                            decimal scale = Math.Min(decimal.Divide(size.Width, imgRaw.Width), decimal.Divide(size.Height, imgRaw.Height));

                            width = (int)(imgRaw.Width * scale);
                            height = (int)(imgRaw.Height * scale);

                            destRect = new Rectangle(0, 0, width, height);
                            srcRect = new Rectangle(0, 0, imgRaw.Width, imgRaw.Height);
                            size = new Size(width, height);
                            break;
                        case ImageResizeMode.Square:
                            int posY = 0, posX = 0;
                            if (imgRaw.Height > imgRaw.Width)
                                posY = (imgRaw.Height - imgRaw.Width) / 2;
                            else
                                posX = (imgRaw.Width - imgRaw.Height) / 2;
                            width = size.Width;
                            height = size.Height;
                            destRect = new Rectangle(0, 0, size.Width, size.Height);
                            srcRect = new Rectangle(posX, posY, imgRaw.Width - posX * 2, imgRaw.Height - posY * 2);
                            break;
                        default:
                            throw new ArgumentException("Invalid image resize mode.");
                    }
                    
                    using (var imgNew = new Bitmap(imgRaw, size))
                    {
                        using (var graphics = Graphics.FromImage(imgNew))
                        {
                           
                            //graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.DrawImage(imgRaw, destRect, srcRect, GraphicsUnit.Pixel);
                        }
                        
                        var imgHelper = new ImageHelper(_sourceID);
                        imgHelper.BuildDirectory(size.Width);
                        string fileName = imgHelper.GetNewFileName(size.Width, souceImageID);
                        imgNew.Save(fileName, ImageFormat.Jpeg);
                    }
                }
            }
        }
        #endregion

    }
}
