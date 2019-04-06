using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Blog
{
    [Authorize]
    public class BlogCategoryAddModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogCategoryAddModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BlogCategory BlogCategory { get; set; }      

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            BlogCategory.Name = BlogCategory.Name.ToUpper().Trim();

            if(await _context.BlogCategory.AnyAsync(a=>a.Name == BlogCategory.Name))
            {
                ModelState.AddModelError("Duplicate", "This category already exits");
                return Page();
            }

            BlogCategory.BlogCategoryID = Guid.NewGuid();

            _context.Add(BlogCategory);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Blog/BlogCategoryList");
        }
    }
}