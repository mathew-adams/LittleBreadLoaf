using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        [BindProperty]
        public MovieUser MovieUser { get; set; }

        public IActionResult OnGet()
        {

            if (HttpContext.Request.Cookies[CartHelper.CartCookieName] == null)
            {
                HasCart = false;
                return Page();
            }
            else
            {
                HasCart = true;
                var cartId = HttpContext.Request.Cookies[CartHelper.CartCookieName];
                var parsedCartID = Guid.Parse(cartId);

                //Cart is checked out?

                CartItems = _context.CartItem.Where(m => m.CartID == parsedCartID).ToList();
                Products = _context.Product
                                   .Join(_context.CartItem,
                                         p => p.ProductID,
                                         ci => ci.ProductID,
                                         (p, ci) => new { p.ProductID, p.Price, p.Name, p.Description, ci.CartItemID, ci.CartID })
                                   //.Join(_context.Cart,
                                   //      ci => ci.CartID,
                                   //      c => c.CartID,
                                   //      (ci, c) => new { ci.ProductID, ci.Price, ci.Name, ci.Description, ci.CartItemID, c.CartID })
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
                                   //.Join(_context.Cart,
                                   //      ci => ci.CartID,
                                   //      c => c.CartID,
                                   //      (ci, c) => new { ci.ProductID, ci.ProductImageID, ci.PrimaryImage, ci.FileLocation, ci.Title, ci.CartItemID, c.CartID })
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
            
            var cartItem = _context.CartItem.FirstOrDefault(ci => ci.CartItemID == parsedCartItemID);
            var cart = _context.Cart.Where(c => c.CartID == cartItem.CartID);

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