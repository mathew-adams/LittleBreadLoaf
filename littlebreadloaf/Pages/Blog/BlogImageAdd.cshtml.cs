using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace littlebreadloaf.Pages.Blog
{
    [Authorize]
    public class BlogImageAddModel : PageModel
    {
        private readonly ProductContext _context;

        public BlogImageAddModel(ProductContext context)
        {
            _context = context;
        }
        
        [BindProperty(SupportsGet = true)]
        public string BlogID { get; set; }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        [BindProperty]
        public BlogImage BlogImage { get; set; }

        [BindProperty]
        public ImageHelper.ImageResizeMode ImageSizeMode { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (String.IsNullOrEmpty(BlogID) || !Guid.TryParse(BlogID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Blog/BlogList");
            }

            var blog = await _context.Blog.FirstOrDefaultAsync(m => m.BlogID == parsedID);

            if (blog == null)
            {
                return new RedirectToPageResult("/Blog/BlogList");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (String.IsNullOrEmpty(BlogID) || !Guid.TryParse(BlogID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Blog/BlogList");
            }

            var blog = await _context.Blog.FirstOrDefaultAsync(m => m.BlogID == parsedID);

            if (blog == null)
            {
                return new RedirectToPageResult("/Blog/BlogList");
            }
            
            if(await _context.BlogImage.AnyAsync(a => a.BlogID == parsedID))
            {
                ModelState.AddModelError("Duplicate", "A blog image already exists. Please delete if you would like to add a new image.");
                return Page();
            }

            var blogImageID = Guid.NewGuid();
            var imgHelper = new ImageHelper(blog.BlogID.ToString());

            List<Size> sizes = new List<Size>();
            var imageMode = "";

            switch (ImageSizeMode)
            {
                case ImageHelper.ImageResizeMode.Banner:
                    imageMode = "BANNER";
                    sizes = new List<Size>
                    {
                        new Size(1110, 400),
                        new Size(555, 200),
                        new Size(222, 80)
                    };
                    break;
                case ImageHelper.ImageResizeMode.AspectRatio:
                    imageMode = "ASPECT";
                    sizes = new List<Size>
                    {
                        new Size(100, 100),
                        new Size(200, 200),
                        new Size(350, 350),
                        new Size(500, 500),
                    };
                    break;
                default:
                    imageMode = "SQUARE";
                    sizes = new List<Size>
                    {
                        new Size(100, 100),
                        new Size(200, 200),
                        new Size(350, 350),
                        new Size(500, 500),
                    };
                    break;
            }


            try
            {
                imgHelper.AddImages(ImageSizeMode,
                                    blogImageID.ToString(),
                                    sizes.ToArray(),
                                    FileUpload);
            }
            catch (Exception ex)
            {
                var systemError = new SystemError
                {
                    ErrorID = Guid.NewGuid(),
                    RequestID = blog.BlogID.ToString(),
                    Path = "BlogImageAddModel",
                    Error = ex.ToString(),
                    Occurred = DateTime.Now
                };
                _context.SystemError.Add(systemError);
                await _context.SaveChangesAsync();

                ModelState.AddModelError("BadFile", "The file is invalid. It must be an image");
                return Page();
            }
            
            BlogImage.BlogID = blog.BlogID.Value;
            BlogImage.BlogImageID = blogImageID;
            BlogImage.FileLocation = imgHelper.GetDisplayFileName(blogImageID.ToString());
            BlogImage.Mode = imageMode;

            _context.BlogImage.Add(BlogImage);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Blog/BlogImageList", new { BlogID });
        }

    }
}