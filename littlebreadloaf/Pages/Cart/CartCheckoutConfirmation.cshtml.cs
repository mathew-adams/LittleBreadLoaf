using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using littlebreadloaf.Services;
using QuestPDF.Fluent;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace littlebreadloaf.Pages.Cart
{
    public class CartCheckoutConfirmationModel : PageModel
    {
        private readonly ProductContext _context;
        private readonly IConfiguration _config;
        private readonly RenderViewComponentService _renderer;
        private readonly IHostingEnvironment _env;
        public CartCheckoutConfirmationModel(ProductContext context, 
                                             IConfiguration config,
                                             RenderViewComponentService renderer,
                                             IHostingEnvironment env)
        {
            _context = context;
            _config = config;
            _renderer = renderer;
            _env = env;
        }

        [BindProperty]
        public ProductOrder ProductOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ProductOrderID { get; set; }

        [BindProperty]
        public NzAddressDeliverable NzAddressDeliverable { get; set; }

        [BindProperty]
        public bool IsPreOrder { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(ProductOrderID) || !Guid.TryParse(ProductOrderID, out Guid productOrderID))
            {
                return new RedirectToPageResult("/Cart/CartView");
            }

            IsPreOrder = HttpContext.Request.Cookies[CartHelper.PreOrderCookie] != null;

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
        public async Task<IActionResult> OnGetPrintAsync ()
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

            if (ProductOrder.ContactAddress > 0)
            {
                NzAddressDeliverable = await _context.NzAddressDeliverable.FirstOrDefaultAsync(f => f.address_id == ProductOrder.ContactAddress);
            }

            var invoiceView = new InvoiceModel();

            invoiceView.Name = _config["LittleBreadLoaf.Name"];
            invoiceView.AddressLine1 = _config["LittleBreadLoaf.AddressLine1"];
            invoiceView.AddressLine2 = _config["LittleBreadLoaf.AddressLine2"];
            invoiceView.BankNumber = _config["LittleBreadLoaf.BankNumber"];
            invoiceView.Phone = _config["LittleBreadLoaf.Phone"];
            invoiceView.ProductOrder = await _context.ProductOrder.FirstOrDefaultAsync(p => p.OrderID == productOrderID);
            invoiceView.HasAddress = false;
            if (invoiceView.ProductOrder.ContactAddress > 0)
            {
                invoiceView.NzAddressDeliverable = await _context.NzAddressDeliverable.FirstOrDefaultAsync(a => a.address_id == invoiceView.ProductOrder.ContactAddress);
                invoiceView.HasAddress = true;
            }
            invoiceView.Invoice = await _context.Invoice.FirstOrDefaultAsync(f => f.ProductOrderID == productOrderID);
            invoiceView.InvoiceTransactions = await _context
                                                    .InvoiceTransaction
                                                    .AsNoTracking()
                                                    .Where(w => w.InvoiceID == invoiceView.Invoice.InvoiceID)
                                                    .ToListAsync();

            invoiceView.Balance = invoiceView.InvoiceTransactions.Sum(s => s.Quantity * s.Price);
            invoiceView.Status = (invoiceView.Balance == 0) ? "PAID" : "DUE";

            using(var msInvoice = new System.IO.MemoryStream())
            {
                var document = new InvoiceDocument(invoiceView, invoiceView.GetLogoUrl(_env));

                document.GeneratePdf(msInvoice);
                var file = File(msInvoice.ToArray(), "application/pdf");
                return file;
            }
        }
    }
}