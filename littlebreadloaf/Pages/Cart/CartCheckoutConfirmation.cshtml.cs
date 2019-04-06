using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;

namespace littlebreadloaf.Pages.Cart
{
    public class CartCheckoutConfirmationModel : PageModel
    {
        private readonly ProductContext _context;
        public CartCheckoutConfirmationModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductOrder ProductOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ProductOrderID { get; set; }

        [BindProperty]
        public NzAddressDeliverable NzAddressDeliverable { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(ProductOrderID) || !Guid.TryParse(ProductOrderID, out Guid productOrderID))
            {
                return new RedirectToPageResult("/Cart/CartView");
            }

            ProductOrder = await _context.ProductOrder.FirstOrDefaultAsync(f => f.OrderID == productOrderID);
            if (ProductOrder == null)
            {
                return new RedirectToPageResult("/Cart/CartView");
            }

            ViewData["HasAddress"] = ProductOrder.ContactAddress > 0;

            if(ProductOrder.ContactAddress > 0)
            {
                NzAddressDeliverable = await _context.NzAddressDeliverable.FirstOrDefaultAsync(f => f.address_id == ProductOrder.ContactAddress);
            }
            
            return Page();
        }
    }
}