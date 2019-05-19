using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Authorization;

namespace littlebreadloaf.Pages.Products
{
    [Authorize]
    public class ProductBundleDeleteModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductBundleDeleteModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string ProductBundleID { get; set; }

        [BindProperty]
        public ProductBundle ProductBundle { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(ProductBundleID) || !Guid.TryParse(ProductBundleID, out Guid parsedID))
            {
                return new RedirectResult("/Products/ProductBundleList");
            }

            ProductBundle = await _context.ProductBundle.FirstOrDefaultAsync(m => m.ProductBundleID == parsedID);
            if (ProductBundle == null)
            {
                return new RedirectResult("/Products/ProductBundleList");
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(s => s.Errors);
                return Page();
            }

            if (string.IsNullOrEmpty(ProductBundleID) || !Guid.TryParse(ProductBundleID, out Guid parsedID))
            {
                return new RedirectResult("/Products/ProductBundleList");
            }
            
            _context.ProductBundleItem.RemoveRange(_context.ProductBundleItem.Where(w => w.ProductBundleID == parsedID));
            _context.ProductBundle.Remove(ProductBundle);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Products/ProductBundleList");
        }
    }
}