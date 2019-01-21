using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
    public class ProductListModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<Product> Products { get; set; }

        [BindProperty]
        public List<ProductBadge> ProductBadges { get; set; }

        [BindProperty]
        public List<ProductImage> ProductImages { get; set; }

        public void OnGet()
        {
            Products = _context.Product.ToList();
            ProductBadges = _context.ProductBadge.ToList();
            ProductImages = _context.ProductImage.Where(m => m.PrimaryImage == true).ToList();
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