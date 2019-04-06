using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using littlebreadloaf.Data;

namespace littlebreadloaf.ViewComponents
{

    public class CartView
    {
        public littlebreadloaf.Data.Cart Cart { get; set; }
        
        public List<Product> Products { get; set; }
        
        public List<ProductImage> ProductImages { get; set; }
        
        public List<CartItem> CartItems { get; set; }
        
        public bool HasCart { get; set; }
    }


    public class CartViewComponent : ViewComponent
    {
        private readonly ProductContext _context;
        public CartViewComponent(ProductContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string cartID)
        {
            var parsedCartID = Guid.Parse(cartID);
            var cartView = new CartView()
            {
                HasCart = !string.IsNullOrEmpty(cartID),
                CartItems = await _context.CartItem.Where(m => m.CartID == parsedCartID).ToListAsync(),
                Products = await _context.Product
                                   .Join(_context.CartItem,
                                         p => p.ProductID,
                                         ci => ci.ProductID,
                                         (p, ci) => new { p.ProductID, p.Price, p.Name, p.Description, ci.CartItemID, ci.CartID })
                                   .Where(w => w.CartID == parsedCartID)
                                   .Select(s => new Product()
                                   {
                                       ProductID = s.ProductID,
                                       Name = s.Name,
                                       Description = s.Description,
                                       Price = s.Price
                                   })
                                   .ToListAsync(),
                ProductImages = await _context.ProductImage
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
                                       .ToListAsync()
            };

            return View(cartView);
        }

    }
}