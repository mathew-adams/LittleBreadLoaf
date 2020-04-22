using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Blog
{
    [Authorize]
    public class BlogPublishModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogPublishModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public littlebreadloaf.Data.Blog Blog { get; set; }

        public async Task<IActionResult> OnGetAsync(string blogID)
        {
            if (String.IsNullOrEmpty(blogID) || !Guid.TryParse(blogID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Blog/BlogList");
            }

            Blog = await _context.Blog.FirstOrDefaultAsync(w => w.BlogID == parsedID);

            if (Blog == null)
            {
                return new RedirectToPageResult("/Blog/BlogList");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (Blog == null || Blog.BlogID == null)
            {
                return new RedirectResult("/Blog/BlogList");
            }

            Blog = await _context.Blog.FirstOrDefaultAsync(m => m.BlogID == Blog.BlogID);

            if (Blog != null)
            {
                var highDate = new DateTime(9999, 12, 31);
                if(Blog.Published == highDate)
                {
                    Blog.Published = DateTime.Now;
                }
                else
                {
                    Blog.Published = highDate;
                }
                
                _context.Update(Blog);
                await _context.SaveChangesAsync();
            }
            return new RedirectToPageResult("/Blog/BlogView", new { Blog.BlogID });
        }
    }
}