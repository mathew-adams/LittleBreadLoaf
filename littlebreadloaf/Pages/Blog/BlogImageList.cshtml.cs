using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Blog
{
    [Authorize]
    public class BlogImageListModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogImageListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string BlogID { get; set; }

        [BindProperty]
        public List<BlogImage> BlogImages { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (String.IsNullOrEmpty(BlogID) || !Guid.TryParse(BlogID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Blog/BlogList");
            }

            if (!await _context.Blog.AnyAsync(m =>m.BlogID == parsedID))
            {
                return new RedirectToPageResult("/Blog/BlogList");
            }

            BlogImages = await _context.BlogImage.Where(w => w.BlogID == parsedID).ToListAsync();

            return Page();
        }
    }
}