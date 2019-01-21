using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using System.IO;

namespace littlebreadloaf.Pages.Products
{
    public class ProductViewModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductViewModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public List<ProductBadge> ProductBadges { get; set; }

        [BindProperty]
        public List<ProductIngredient> ProductIngredients { get; set; }

        [BindProperty]
        public List<ProductSuggestion> ProductSuggestions { get; set; }

        [BindProperty]
        public List<ProductImage> Images { get; set; }

        public CartItem CartItem { get; set; }
        public littlebreadloaf.Data.Cart Cart { get; set; }

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
            ProductBadges = _context.ProductBadge.Where(m => m.ProductID == parsedID).ToList();
            ProductIngredients = _context.ProductIngredient.Where(m => m.ProductID == parsedID).ToList();
            ProductSuggestions = _context.ProductSuggestion.Where(m => m.ProductID == parsedID).ToList();
            Images = _context.ProductImage.Where(m => m.ProductID == parsedID).ToList();

            return new PageResult();
            
        }

        public async Task<IActionResult> OnPostCartAddAsync(string productID)
        {
            if (String.IsNullOrEmpty(productID) || !Guid.TryParse(productID, out Guid parsedID))
            {
                return new RedirectResult("/Products/ProductList");
            }
            
            return await CartHelper.AddToCart(_context,
                                              parsedID,
                                              User, 
                                              HttpContext);
        }
    }
}