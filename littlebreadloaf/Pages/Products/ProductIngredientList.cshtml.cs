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
    public class ProductIngredientListModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductIngredientListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public List<ProductIngredient> ProductIngredients { get; set; }

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

            ProductIngredients = await _context
                                        .ProductIngredient
                                        .Where(m => m.ProductID == parsedID)
                                        .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string ProductIngredientID, string ProductID)
        {
            //Sanitize inputs
            if (String.IsNullOrEmpty(ProductID)
                || !Guid.TryParse(ProductID, out Guid parsedProductID)
                || String.IsNullOrEmpty(ProductIngredientID)
                || !Guid.TryParse(ProductIngredientID, out Guid parsedIngredientID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            Product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == parsedProductID);

            if (Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            var ingredient = await _context.ProductIngredient.FirstOrDefaultAsync(m => m.ProductIngredientID == parsedIngredientID);
            if (ingredient != null)
            {
                _context.ProductIngredient.Remove(ingredient);
                await _context.SaveChangesAsync();
            }

            return new RedirectToPageResult("/Products/ProductIngredientList", new { ProductID = ProductID });
        }
    }
}