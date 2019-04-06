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
    public class BlogCategoryDeleteModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogCategoryDeleteModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string BlogCategoryID { get; set; }

        [BindProperty]
        public BlogCategory BlogCategory { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(BlogCategoryID) || !Guid.TryParse(BlogCategoryID, out Guid blogCategoryID))
            {
                return new RedirectToPageResult("/Blog/BlogCategoryList");
            }

            BlogCategory = await _context.BlogCategory.FirstOrDefaultAsync(w => w.BlogCategoryID == blogCategoryID);
            if (BlogCategory == null)
            {
                return new RedirectToPageResult("/Blog/BlogCategoryList");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.CategoryToBlog.RemoveRange(_context.CategoryToBlog.Where(w => w.BlogCategoryID == BlogCategory.BlogCategoryID));
            _context.BlogCategory.Remove(BlogCategory);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Blog/BlogCategoryList");
        }
    }
}