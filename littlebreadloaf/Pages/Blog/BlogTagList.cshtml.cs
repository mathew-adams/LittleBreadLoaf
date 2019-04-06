using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Blog
{
    [Authorize]
    public class BlogTagListModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogTagListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<Tag> BlogTags { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            BlogTags = await _context
                            .Tag
                            .Join(_context.SourceToTag,
                                t => t.TagID, st => st.TagID, (t, st) => new { t.TagID, t.Name, st.SourceArea })
                            .Where(w => w.SourceArea == "Blog")
                            .Select(s => new Tag()
                            {
                                TagID = s.TagID,
                                Name = s.Name
                            })
                            .ToListAsync();

            return Page();
        }
    }
}