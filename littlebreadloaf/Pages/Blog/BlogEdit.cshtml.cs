using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace littlebreadloaf.Pages.Blog
{
    [Authorize]
    public class BlogEditModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogEditModel(ProductContext context)
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

            Tags = await _context
                        .Tag
                        .GroupJoin(_context.SourceToTag,
                                    t => t.TagID,
                                    s => s.TagID,
                                    (t, s) => new { Tag = t, SourceToTag = s })
                        .SelectMany(xy => xy.SourceToTag
                                            .Where(w => w.SourceID == parsedID)
                                            .DefaultIfEmpty(),
                                   (t, s) => new { t.Tag, SourceToTag = s })
                        .Select(s => new SelectListItem
                        {
                            Text = s.Tag.Name,
                            Value = s.Tag.Name,
                            Selected = s.SourceToTag.SourceID == parsedID
                        })
                        .OrderBy(o => o.Value)
                        .ToListAsync();

            Categories = await _context
                               .BlogCategory
                               .GroupJoin(_context.CategoryToBlog,
                                          c => c.BlogCategoryID,
                                          cb => cb.BlogCategoryID, (c, cb) => new { BlogCategory = c, CategoryToBlog = cb })
                               .SelectMany(xy => xy.CategoryToBlog
                                                   .Where(w => w.BlogID == parsedID)
                                                   .DefaultIfEmpty(),
                                                   (c, cb) => new { c.BlogCategory, CategoryToBlog = cb })
                               .Select(s => new SelectListItem
                               {
                                   Text = s.BlogCategory.Name,
                                   Value = s.BlogCategory.BlogCategoryID.ToString(),
                                   Selected = s.CategoryToBlog.BlogID == parsedID
                               })
                               .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //ModelState.ErrorCount
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            _context.Blog.Update(Blog);

            string url = Url.Page("/Blog/BlogView", new { Blog.BlogID });

            await TagHelper.AddTags(_context, Blog.BlogID, "Blog", url, SelectedTags);

            if(!string.IsNullOrEmpty(SelectedCategory) && Guid.TryParse(SelectedCategory, out Guid parsedCategoryID))
            {
                if(_context.BlogCategory.Any(a => a.BlogCategoryID == parsedCategoryID))
                {
                    _context.CategoryToBlog.RemoveRange(_context.CategoryToBlog.Where(w => w.BlogID == Blog.BlogID));
                    _context.CategoryToBlog.Add(new CategoryToBlog()
                    {
                        CategoryToBlogID = Guid.NewGuid(),
                        BlogCategoryID = parsedCategoryID,
                        BlogID = Blog.BlogID
                    });
                }
            }

            await _context.SaveChangesAsync();

            return new LocalRedirectResult(url);
        }

    }
}