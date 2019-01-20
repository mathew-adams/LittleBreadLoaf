using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using littlebreadloaf.Data;

namespace littlebreadloaf.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly ProductContext _context;
        public CartCountViewComponent(ProductContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string cartID)
        {
            List<CartItem> cartItems = new List<CartItem>();

            if(String.IsNullOrEmpty(cartID))
            {
                return View(cartItems);
            }

            cartItems = await _context.CartItem.Where(c => c.CartID == Guid.Parse(cartID)).ToListAsync();
            return View(cartItems);
        }

    }
}