using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
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

        public IActionResult OnGet(string ProductID)
        {
            if (String.IsNullOrEmpty(ProductID) || !Guid.TryParse(ProductID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            Product = _context.Product.FirstOrDefault(m => m.ProductID == parsedID);

            if (Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            ProductBadges = _context.ProductBadge.Where(m => m.ProductID == parsedID).ToList();

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

            Product = _context.Product.FirstOrDefault(m => m.ProductID == parsedProductID);

            if (Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            var badge = _context.ProductBadge.FirstOrDefault(m => m.ProductBadgeID == parsedBadgeID);
            if (badge != null)
            {
                _context.ProductBadge.Remove(badge);
                await _context.SaveChangesAsync();
            }

            return new RedirectToPageResult("/Products/ProductBadgeList", new { ProductID = ProductID });
        }
    }
}