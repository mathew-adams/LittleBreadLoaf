using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using littlebreadloaf.Data;
using Microsoft.Extensions.Configuration;

namespace littlebreadloaf.ViewComponents
{
    public class InvoiceViewComponent : ViewComponent
    {
        private readonly ProductContext _context;
        private readonly IConfiguration _config;
        public InvoiceViewComponent(ProductContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public class ViewInvoice
        {
            public string Status { get; set; }
            public decimal Balance { get; set; }
            public string Name { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string BankNumber { get; set; }
            public string Phone { get; set; }
            public bool HasAddress { get; set; }
            public ProductOrder ProductOrder { get; set; }
            public Invoice Invoice { get; set; }
            public NzAddressDeliverable NzAddressDeliverable { get; set; }
            public List<InvoiceTransaction> InvoiceTransactions { get; set; }
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid orderID)
        {
            var invoiceView = new ViewInvoice();
            invoiceView.Name = _config["LittleBreadLoaf.Name"];
            invoiceView.AddressLine1 = _config["LittleBreadLoaf.AddressLine1"];
            invoiceView.AddressLine2 = _config["LittleBreadLoaf.AddressLine2"];
            invoiceView.BankNumber = _config["LittleBreadLoaf.BankNumber"];
            invoiceView.Phone = _config["LittleBreadLoaf.Phone"];
            invoiceView.ProductOrder = await _context.ProductOrder.FirstOrDefaultAsync(p => p.OrderID == orderID);
            invoiceView.HasAddress = false;
            if (invoiceView.ProductOrder.ContactAddress > 0)
            {
                invoiceView.NzAddressDeliverable = await _context.NzAddressDeliverable.FirstOrDefaultAsync(a => a.address_id == invoiceView.ProductOrder.ContactAddress);
                invoiceView.HasAddress = true;
            }


            invoiceView.Invoice = await _context.Invoice.FirstOrDefaultAsync(f => f.ProductOrderID == orderID);
            invoiceView.InvoiceTransactions = await _context
                                                    .InvoiceTransaction
                                                    .AsNoTracking()
                                                    .Where(w => w.InvoiceID == invoiceView.Invoice.InvoiceID)
                                                    .ToListAsync();

            invoiceView.Balance = invoiceView.InvoiceTransactions.Sum(s => s.Quantity * s.Price);
            invoiceView.Status = (invoiceView.Balance == 0) ? "PAID" : "DUE";

            return View(invoiceView);
        }

    }
}