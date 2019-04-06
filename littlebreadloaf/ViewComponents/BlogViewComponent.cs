using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using littlebreadloaf.Data;

namespace littlebreadloaf.ViewComponents
{
    public class BlogViewComponent : ViewComponent
    {
        private readonly ProductContext _context;
        public BlogViewComponent(ProductContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid blogID)
        {
            var blog = await _context.Blog.FirstOrDefaultAsync(w => w.BlogID == blogID);
            return View(blog);
        }

    }
}