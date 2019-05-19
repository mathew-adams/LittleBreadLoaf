using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Authorization;

namespace littlebreadloaf.Pages.Blog
{
    [Authorize]
    public class BlogImageDeleteModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogImageDeleteModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string BlogID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string BlogImageID { get; set; }

        [BindProperty]
        public BlogImage BlogImage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (String.IsNullOrEmpty(BlogImageID) || !Guid.TryParse(BlogImageID, out Guid blogImageID))
            {
                return new RedirectToPageResult("/Blog/BlogImageList");
            }

            if (String.IsNullOrEmpty(BlogID) || !Guid.TryParse(BlogID, out Guid blogID))
            {
                return new RedirectToPageResult("/Blog/BlogImageList");
            }

            if (!await _context.Blog.AnyAsync(a => a.BlogID == blogID))
            {
                return new RedirectToPageResult("/Blog/BlogImageList");
            }

            BlogImage = await _context.BlogImage.FirstOrDefaultAsync(i => i.BlogImageID == blogImageID);

            if (BlogImage == null)
            {
                return new RedirectToPageResult("/Blog/BlogImageList");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (String.IsNullOrEmpty(BlogImageID) || !Guid.TryParse(BlogImageID, out Guid blogImageID))
            {
                return new RedirectToPageResult("/Blog/BlogImageList");
            }

            if (String.IsNullOrEmpty(BlogID) || !Guid.TryParse(BlogID, out Guid blogID))
            {
                return new RedirectToPageResult("/Blog/BlogImageList");
            }

            if (!await _context.Blog.AnyAsync(a => a.BlogID == blogID))
            {
                return new RedirectToPageResult("/Blog/BlogImageList");
            }

            var imgHelper = new ImageHelper(BlogImage.BlogID.ToString());
            imgHelper.DeleteImage(BlogImage.BlogImageID.ToString());

            _context.BlogImage.Remove(BlogImage);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Blog/BlogImageList", new { BlogImage.BlogID });
        }



    }
}