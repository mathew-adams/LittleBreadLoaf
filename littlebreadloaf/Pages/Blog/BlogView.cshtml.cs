using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Blog
{
    public class BlogViewModel : PageModel
    {
        private readonly ProductContext _context;
        public BlogViewModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public littlebreadloaf.Data.Blog Blog { get; set; }
        
        [BindProperty]
        public BlogImage BlogImage { get; set; }

        [BindProperty]
        public bool IsPreOrder { get; set; }
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

            IsPreOrder = HttpContext.Request.Cookies[CartHelper.PreOrderCookie] != null;


            BlogImage = await _context.BlogImage.FirstOrDefaultAsync(i => i.BlogID == parsedID);

            return Page();
        }
    }
}