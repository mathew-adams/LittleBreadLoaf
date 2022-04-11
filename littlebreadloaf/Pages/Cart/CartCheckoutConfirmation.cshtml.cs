using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;
using SelectPdf;
using System.IO;

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

            var url = Url.Page("/Orders/InvoicePrint", new { ProductOrder.OrderID });

            var absUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Scheme, HttpContext.Request.Host, url);

            HtmlToPdf converter = new SelectPdf.HtmlToPdf();

            converter.Options.MarginBottom = 20;
            converter.Options.MarginTop = 20;
            converter.Options.MarginRight = 20;
            converter.Options.MarginLeft = 20;
            PdfDocument doc = converter.ConvertUrl(absUrl);

            using (var msInvoice = new System.IO.MemoryStream())
            {
                doc.Save(msInvoice);
                // close pdf document

                doc.Close();
                msInvoice.Position = 0;

                string contentType = "application/pdf";
               // string fileName = "sample.pdf";

                var file = File(msInvoice.ToArray(), contentType);

                return file;
            }
        }
    }
}