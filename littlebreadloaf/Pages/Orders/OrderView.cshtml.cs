using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace littlebreadloaf.Pages.Orders
{
    [Authorize]
    public class OrderViewModel : PageModel
    {
        private readonly ProductContext _context;
        public OrderViewModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string OrderID { get; set; }

        [BindProperty]
        public ProductOrder ProductOrder { get; set; }

        [BindProperty]
        public NzAddressDeliverable NzAddressDeliverable { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (String.IsNullOrEmpty(OrderID) || !Guid.TryParse(OrderID, out Guid parsedID))
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            ProductOrder = await _context.ProductOrder.FirstOrDefaultAsync(m => m.OrderID == parsedID);
            if (ProductOrder == null)
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            NzAddressDeliverable = await _context.NzAddressDeliverable.FirstOrDefaultAsync(f => f.address_id == ProductOrder.ContactAddress);

            return Page();
        }
    }
}