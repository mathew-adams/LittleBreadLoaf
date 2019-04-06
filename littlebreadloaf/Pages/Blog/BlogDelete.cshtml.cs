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
    public class BlogDeleteModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogDeleteModel(ProductContext context)
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
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            _context.CategoryToBlog.RemoveRange(_context.CategoryToBlog.Where(w => w.BlogID == Blog.BlogID));
            _context.SourceToTag.RemoveRange(_context.SourceToTag.Where(w => w.SourceArea == "Blog" && w.SourceID == Blog.BlogID));
            _context.Blog.Remove(Blog);

            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Blog/BlogList");
        }
    }
}