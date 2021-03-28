using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

        [BindProperty]
        public List<ProductOrderOutage> ProductOrderOutages { get; set; }

        [BindProperty]
        public bool IsPreOrder { get; set; }

        public CartItem CartItem { get; set; }
        public littlebreadloaf.Data.Cart Cart { get; set; }

        
        public async Task<IActionResult> OnGetAsync(string productID)
        {
            IsPreOrder = HttpContext.Request.Cookies[CartHelper.PreOrderCookie] != null;

            if (string.IsNullOrEmpty(productID) || !Guid.TryParse(productID, out Guid parsedID))
            {
                if(IsPreOrder)
                    return new RedirectResult("/Products/ProductPreOrder");

                return new RedirectResult("/Products/ProductList");
            }

            Product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == parsedID);
            if (Product == null)
            {
                if (IsPreOrder)
                    return new RedirectResult("/Products/ProductPreOrder");

                return new RedirectResult("/Products/ProductList");
            }

            ProductBadges = await _context.ProductBadge.Where(m => m.ProductID == parsedID).ToListAsync();
            ProductIngredients = await _context.ProductIngredient.Where(m => m.ProductID == parsedID).ToListAsync();
            ProductSuggestions = await _context.ProductSuggestion.Where(m => m.ProductID == parsedID).ToListAsync();
            Images = await _context.ProductImage.Where(m => m.ProductID == parsedID).ToListAsync();
            ProductOrderOutages = await _context.ProductOrderOutage.AsNoTracking().ToListAsync();

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