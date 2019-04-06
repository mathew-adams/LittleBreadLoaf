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
    public class ProductSuggestionListModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductSuggestionListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public List<ProductSuggestion> ProductSuggestions { get; set; }

        public async Task<IActionResult> OnGetAsync(string productID)
        {
            if (String.IsNullOrEmpty(productID) || !Guid.TryParse(productID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            Product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == parsedID);

            if (Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            ProductSuggestions = await _context
                                        .ProductSuggestion
                                        .Where(m => m.ProductID == parsedID)
                                        .ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostDeleteAsync(string productSuggestionID, string productID)
        {
            //Sanitize inputs
            if (String.IsNullOrEmpty(productID)
                || !Guid.TryParse(productID, out Guid parsedProductID)
                || String.IsNullOrEmpty(productSuggestionID)
                || !Guid.TryParse(productSuggestionID, out Guid parsedSuggestionID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            Product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == parsedProductID);

            if (Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            var suggestion = await _context.ProductSuggestion.FirstOrDefaultAsync(m => m.ProductSuggestionID == parsedSuggestionID);
            if (suggestion != null)
            {
                _context.ProductSuggestion.Remove(suggestion);
                await _context.SaveChangesAsync();
            }

            return new RedirectToPageResult("/Products/ProductSuggestionList", new { productID });
        }

    }
}