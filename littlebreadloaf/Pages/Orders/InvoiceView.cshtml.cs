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
    public class InvoiceViewModel : PageModel
    {
        private readonly ProductContext _context;
        public InvoiceViewModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string OrderID { get; set; }

        [BindProperty]
        public ProductOrder ProductOrder { get; set; }

        [BindProperty]
        public Invoice Invoice { get; set; }

        [BindProperty]
        public List<InvoiceTransaction> InvoiceTransaction { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (String.IsNullOrEmpty(OrderID) || !Guid.TryParse(OrderID, out Guid parsedID))
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            ProductOrder = await _context.ProductOrder.FirstOrDefaultAsync(p => p.OrderID == parsedID);
            if(ProductOrder == null)
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            Invoice = await _context.Invoice.FirstOrDefaultAsync(f => f.ProductOrderID == parsedID);
            if(Invoice == null)
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            InvoiceTransaction = await _context
                                        .InvoiceTransaction
                                        .AsNoTracking()
                                        .Where(w => w.InvoiceID == Invoice.InvoiceID)
                                        .ToListAsync();
            return Page();
        }
    }
}