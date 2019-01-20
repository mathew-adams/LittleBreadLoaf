using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Cart
{
    public class CartCheckoutModel : PageModel
    {
        private readonly ProductContext _context;
        public CartCheckoutModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public littlebreadloaf.Data.Cart Cart { get; set; }

        [BindProperty]
        public List<Product> Products { get; set; }

        [BindProperty]
        public List<CartItem> CartItems { get; set; }

        [BindProperty]
        public List<ProductImage> ProductImages { get; set; }
        
        public IActionResult OnGet() 
        {
            var cartId = HttpContext.Request.Cookies["CartID"];

            if (cartId == null || !Guid.TryParse(cartId, out Guid parsedCartID))
            {
                return new RedirectResult("/Cart/CartView");
            }

            CartItems = _context.CartItem.Where(ci => ci.CartID == parsedCartID).ToList();

            Products = _context.Product
                               .Join(_context.CartItem,
                                     p => p.ProductID,
                                     ci => ci.ProductID,
                                     (p, ci) => new { p.ProductID, p.Name, p.Description, p.Price, ci.Quantity, ci.CartID })
                               .Where(w => w.CartID == parsedCartID)
                               .Select(s => new Product()
                                       {
                                           ProductID = s.ProductID,
                                           Name = s.Name,
                                           Description = s.Description,
                                           Price = s.Price
                                       })
                               .ToList();
            ProductImages = _context.ProductImage
                                    .Join(_context.CartItem,
                                          p => p.ProductID,
                                          ci => ci.ProductID,
                                          (p, ci) => new { p.ProductID, p.ProductImageID, p.PrimaryImage, p.FileLocation, p.Title, ci.CartItemID, ci.CartID })
                                   .Where(w => w.CartID == parsedCartID && w.PrimaryImage == true)
                                   .Select(s => new ProductImage()
                                           {
                                               ProductImageID = s.ProductImageID,
                                               ProductID = s.ProductID,
                                               FileLocation = s.FileLocation,
                                               PrimaryImage = s.PrimaryImage,
                                               Title = s.Title
                                           })
                                   .ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string productID)
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            return Page();
        }
    }
}