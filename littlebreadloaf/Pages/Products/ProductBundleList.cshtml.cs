using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
    public class ProductBundleListModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductBundleListModel(ProductContext context)
        {
            _context = context;
        }
        
        [BindProperty]
        public List<ProductBundle> ProductBundles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ProductBundles = await _context.ProductBundle.AsNoTracking().ToListAsync();
            return Page();
        }
    }
}