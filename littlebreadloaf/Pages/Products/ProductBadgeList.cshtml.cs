using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
    [Authorize]
    public class ProductBadgeListModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductBadgeListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public List<ProductBadge> ProductBadges { get; set; }

        public async Task<IActionResult> OnGetAsync(string ProductID)
        {
            if (String.IsNullOrEmpty(ProductID) || !Guid.TryParse(ProductID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            Product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == parsedID);

            if(Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            ProductBadges = await _context
                                    .ProductBadge
                                    .Where(m => m.ProductID == parsedID)
                                    .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string ProductBadgeID, string ProductID)
        {
            //Sanitize inputs
            if (String.IsNullOrEmpty(ProductID)
                || !Guid.TryParse(ProductID, out Guid parsedProductID)
                || String.IsNullOrEmpty(ProductBadgeID)
                || !Guid.TryParse(ProductBadgeID, out Guid parsedBadgeID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            Product = await _context
                            .Product
                            .FirstOrDefaultAsync(m => m.ProductID == parsedProductID);

            if (Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            var badge = await _context
                                .ProductBadge
                                .FirstOrDefaultAsync(m => m.ProductBadgeID == parsedBadgeID);
            if (badge != null)
            {
                _context.ProductBadge.Remove(badge);
                await _context.SaveChangesAsync();
            }

            return new RedirectToPageResult("/Products/ProductBadgeList", new { ProductID = ProductID });
        }
    }
}