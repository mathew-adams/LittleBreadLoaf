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
    public class BlogCategoryListModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogCategoryListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<BlogCategory> BlogCategories { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            BlogCategories = await _context.BlogCategory.ToListAsync();
            return Page();
        }
    }
}