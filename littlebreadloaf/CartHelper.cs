using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace littlebreadloaf
{
    public static class CartHelper
    {

        public const string CartCookieName = "LittleBreadLoaf.CartID";
        public const string PreOrderCookie = "LittleBreadLoaf.PreOrderMode";
        public static async Task<ObjectResult> AddToCart(ProductContext context, 
                                                         Guid productID,
                                                         ClaimsPrincipal user,
                                                         HttpContext http)
        {
            Data.Cart cart = null;
            Data.CartItem item = null;

            var cookie = http.Request.Cookies[CartCookieName];
            bool invalidCookieDetected = false;
            if(!string.IsNullOrEmpty(cookie) && !Guid.TryParse(cookie, out Guid parsedCookie))
            {
                http.Response.Cookies.Delete(CartCookieName);
                invalidCookieDetected = true;
            }

            if (http.Request.Cookies[CartCookieName] == null || invalidCookieDetected)
            {
                cart = new Data.Cart()
                {
                    CartID = Guid.NewGuid(),
                    Created = DateTime.Now,
                    CheckedOut = new DateTime(9999, 12, 31)
                };

                context.Cart.Add(cart);

                http.Response.Cookies.Append(CartCookieName, cart.CartID.ToString());
            }
            else
            {
                var cartID = http.Request.Cookies[CartCookieName];
                var parsedCartId = Guid.Parse(cartID);
                cart = context.Cart.FirstOrDefault(m => m.CartID == parsedCartId);
                item = context.CartItem.FirstOrDefault(m => m.CartID == parsedCartId && m.ProductID == productID);
            }

            if (item != null)
            {
                item.Quantity++;
                context.CartItem.Update(item);
                await context.SaveChangesAsync();
            }
            else
            {
                var product = context.Product.FirstOrDefault(m => m.ProductID == productID);
                item = new CartItem()
                {
                    CartItemID = Guid.NewGuid(),
                    CartID = cart.CartID,
                    ProductID = productID,
                    Quantity = 1,
                    Price = product.Price,
                    Created = DateTime.Now
                };
                context.CartItem.Add(item);

                await context.SaveChangesAsync();
            }

            //Get product name and cart count
            int? cartCount = await context.CartItem.Where(c => c.CartID == cart.CartID).SumAsync(s => s.Quantity);
            string productName = context.Product.FirstOrDefault(p => p.ProductID == productID).Name;
            return new OkObjectResult(new { CartCount=cartCount, ProductName=productName });
        }
    }
}
