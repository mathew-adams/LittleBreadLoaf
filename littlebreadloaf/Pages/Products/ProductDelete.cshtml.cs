using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
    [Authorize]
    public class ProductDeleteModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductDeleteModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        public IActionResult OnGet(string ProductID)
        {
            if (String.IsNullOrEmpty(ProductID) || !Guid.TryParse(ProductID, out Guid parsedID))
            {
                return new RedirectResult("/Products/ProductList");
            }

            Product = _context.Product.FirstOrDefault(m => m.ProductID == parsedID);
            if (Product == null)
            {
                return new RedirectResult("/Products/ProductList");
            }

            return Page();
        }


        public async Task<IActionResult> OnPost()
        {
            if(Product == null || Product.ProductID == null)
            {
                return new RedirectResult("/Products/ProductList");
            }

            Product = _context.Product.FirstOrDefault(m => m.ProductID == Product.ProductID);
            if (Product != null)
            {
                var imgHel = new ImageHelper(Product.ProductID.ToString());
                imgHel.DeleteAll();

                _context.RemoveRange(_context.ProductBadge.Where(b => b.ProductID == Product.ProductID));
                _context.RemoveRange(_context.ProductIngredient.Where(b => b.ProductID == Product.ProductID));
                _context.RemoveRange(_context.ProductSuggestion.Where(b => b.ProductID == Product.ProductID));
                _context.RemoveRange(_context.ProductImage.Where(b => b.ProductID == Product.ProductID));
                _context.Remove(Product);
                await _context.SaveChangesAsync();
            }
            return new RedirectResult("/Products/ProductList");
        }

    }
}