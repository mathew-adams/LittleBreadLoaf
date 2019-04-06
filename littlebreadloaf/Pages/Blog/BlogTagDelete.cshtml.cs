using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Blog
{
    [Authorize]
    public class BlogTagDeleteModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogTagDeleteModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string BlogTagID { get; set; }

        [BindProperty]
        public Tag BlogTag { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(BlogTagID) || !Guid.TryParse(BlogTagID, out Guid blogTagID))
            {
                return new RedirectToPageResult("/Blog/BlogTagList");
            }

            BlogTag = await _context.Tag.FirstOrDefaultAsync(w => w.TagID == blogTagID);
            if (BlogTag == null)
            {
                return new RedirectToPageResult("/Blog/BlogTagList");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.SourceToTag.RemoveRange(_context.SourceToTag.Where(w => w.TagID == BlogTag.TagID));
            _context.Tag.Remove(BlogTag);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Blog/BlogTagList");
        }
    }
}