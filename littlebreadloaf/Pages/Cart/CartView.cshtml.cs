using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Cart
{
    public class CartViewModel : PageModel
    {
        private readonly ProductContext _context;
        public CartViewModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public littlebreadloaf.Data.Cart Cart { get; set; }

        [BindProperty]
        public List<Product> Products { get; set; }

        [BindProperty]
        public List<ProductImage> ProductImages { get; set; }

        [BindProperty]
        public List<CartItem> CartItems { get; set; }
        
        [BindProperty]
        public bool HasCart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Request.Cookies[CartHelper.CartCookieName] == null)
            {
                HasCart = false;
                return Page();
            }
            else
            {
                var cartId = HttpContext.Request.Cookies[CartHelper.CartCookieName];
                if(string.IsNullOrEmpty(cartId) || !Guid.TryParse(cartId, out Guid parsedCartID))
                {
                    HasCart = false;
                    return Page();
                }

                HasCart = true;

                //Cart is checked out?

                CartItems = await _context.CartItem.Where(m => m.CartID == parsedCartID).ToListAsync();
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
                                   .ToListAsync();

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
                                       .ToListAsync();
            }

            return Page();

        }
        
        public async Task<IActionResult> OnPostIncreaseAsync(string cartItemID)
        {
            //Sanitize inputs
            if (String.IsNullOrEmpty(cartItemID)
                || !Guid.TryParse(cartItemID, out Guid parsedCartItemID))
            {
                return new RedirectToPageResult("/Cart/CartView");
            }
            
            var cartItem = await _context.CartItem.FirstOrDefaultAsync(ci => ci.CartItemID == parsedCartItemID);

            if (cartItem == null)
            {
                return new RedirectToPageResult("/Cart/CartView");
            }

            cartItem.Quantity++;
            _context.CartItem.Update(cartItem);

            await _context.SaveChangesAsync();

            //return new OkObjectResult(cartItem);
            return new RedirectToPageResult("/Cart/CartView");
        }

        public async Task<IActionResult> OnPostDecreaseAsync(string cartItemID)
        {
            //Sanitize inputs
            if (String.IsNullOrEmpty(cartItemID)
                || !Guid.TryParse(cartItemID, out Guid parsedCartItemID))
            {
                return new RedirectToPageResult("/Cart/CartView");
            }

            var cartItem = _context.CartItem.FirstOrDefault(ci => ci.CartItemID == parsedCartItemID);

            if (cartItem == null)
            {
                return new RedirectToPageResult("/Cart/CartView");
            }
            if(cartItem.Quantity>1)
            {
                cartItem.Quantity--;
                _context.CartItem.Update(cartItem);
                await _context.SaveChangesAsync();
            }
            //return new OkObjectResult(cartItem);
            return new RedirectToPageResult("/Cart/CartView");
        }

        public async Task<IActionResult> OnPostDeleteAsync(string cartItemID)
        {
            //Sanitize inputs
            if (String.IsNullOrEmpty(cartItemID)
                || !Guid.TryParse(cartItemID, out Guid parsedCartItemID))
            {
                return new RedirectToPageResult("/Cart/CartView");
            }

            var cartItem = _context.CartItem.FirstOrDefault(ci => ci.CartItemID == parsedCartItemID);

            if (cartItem == null)
            {
                return new RedirectToPageResult("/Cart/CartView");
            }

            try
            {
                _context.CartItem.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                //OK to move on
            }
            catch (Exception)
            {
                throw;
            }

            return new RedirectToPageResult("/Cart/CartView");
        }

    }
}