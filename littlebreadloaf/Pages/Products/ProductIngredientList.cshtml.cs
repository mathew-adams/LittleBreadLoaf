using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
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

            ProductIngredients = _context.ProductIngredient.Where(m => m.ProductID == parsedID).ToList();

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

            Product = _context.Product.FirstOrDefault(m => m.ProductID == parsedProductID);

            if (Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            var ingredient = _context.ProductIngredient.FirstOrDefault(m => m.ProductIngredientID == parsedIngredientID);
            if (ingredient != null)
            {
                _context.ProductIngredient.Remove(ingredient);
                await _context.SaveChangesAsync();
            }

            return new RedirectToPageResult("/Products/ProductIngredientList", new { ProductID = ProductID });
        }
    }
}