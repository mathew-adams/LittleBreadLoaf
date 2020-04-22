using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;

namespace littlebreadloaf.Pages.Orders
{
    [Authorize]
    public class OrdersListModel : PageModel
    {
        private readonly ProductContext _context;

        public OrdersListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<ProductOrder> ProductOrders { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterConfirmationCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDeliveryDateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDeliveryDateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterPhone { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool FilterShowAll { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var query = (from orders in _context.ProductOrder select orders);

            query = query.Where(w => w.ConfirmationCode != "");

            if (!FilterShowAll)
                query = query.Where(w => w.Payment == new DateTime(9999,12,31));

            if (!string.IsNullOrEmpty(FilterConfirmationCode))
                query = query.Where(w => w.ConfirmationCode.Contains(FilterConfirmationCode, StringComparison.OrdinalIgnoreCase));

            if (FilterDeliveryDateFrom != null)
                query = query.Where(df => df.DeliveryDate >= FilterDeliveryDateFrom);

            if (FilterDeliveryDateTo != null)
                query = query.Where(dt => dt.DeliveryDate <= FilterDeliveryDateTo);

            if (!string.IsNullOrEmpty(FilterEmail))
                query = query.Where(e => e.ContactEmail.Contains(FilterEmail, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(FilterPhone))
                query = query.Where(e => e.ContactPhone.Contains(FilterPhone, StringComparison.OrdinalIgnoreCase));

            ProductOrders = await query.ToListAsync();

            ProductOrders = ProductOrders.OrderByDescending(o => o.Created).ToList();

            return Page();
        }
    }
}