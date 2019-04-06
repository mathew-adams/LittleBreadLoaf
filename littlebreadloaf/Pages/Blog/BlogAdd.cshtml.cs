using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Blog
{
    [Authorize]
    public class BlogAddModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogAddModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public littlebreadloaf.Data.Blog Blog { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> Tags { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> Categories { get; set; }

        [BindProperty]
        public IEnumerable<string> SelectedTags { get; set; }

        [BindProperty]
        public string SelectedCategory { get; set; }

        public void OnGet()
        {
            Tags = _context.Tag.Select(s => new SelectListItem()
            {
                Text = s.Name,
                Value = s.Name
            });

            Categories = _context.BlogCategory.Select(s => new SelectListItem()
            {
                Text = s.Name,
                Value = s.BlogCategoryID.ToString()
            });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //ModelState.ErrorCount
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(m => m.Errors);
                return Page();
            }
            
            Blog.BlogID = Guid.NewGuid();
            Blog.Created = DateTime.Now;
            Blog.Published = new DateTime(9999, 12, 31);

            _context.Blog.Add(Blog);

            if(!string.IsNullOrEmpty(SelectedCategory) && Guid.TryParse(SelectedCategory, out Guid parsedCategoryID))
            {
                if(_context.BlogCategory.Any(a => a.BlogCategoryID == parsedCategoryID))
                {
                    var category = new CategoryToBlog()
                    {
                        CategoryToBlogID = Guid.NewGuid(),
                        BlogCategoryID = parsedCategoryID,
                        BlogID = Blog.BlogID
                    };
                    _context.CategoryToBlog.Add(category);
                }
            }

            string url = Url.Page("/Blog/BlogView", new { Blog.BlogID });

            await TagHelper.AddTags(_context, Blog.BlogID, "Blog", url, SelectedTags);

            await _context.SaveChangesAsync();

            return new LocalRedirectResult(url);
        }

    }
}