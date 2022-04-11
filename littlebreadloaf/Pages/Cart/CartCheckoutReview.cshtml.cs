using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace littlebreadloaf.Pages.Cart
{
    public class CartCheckoutReviewModel : PageModel
    {
        private readonly ProductContext _context;
        private readonly IConfiguration _config;

        private const int Number_of_days_till_due = 14;

        public CartCheckoutReviewModel(ProductContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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
            if(ProductOrder == null)
            {
                return new RedirectToPageResult("/Cart/CartView");
            }

            NzAddressDeliverable = await _context.NzAddressDeliverable.FirstOrDefaultAsync(f => f.address_id == ProductOrder.ContactAddress);

            if (ProductOrder.DeliveryDate == new DateTime(9999, 12, 31))
                ProductOrder.DeliveryDate = null;
            if (ProductOrder.PickupDate == new DateTime(9999, 12, 31))
                ProductOrder.PickupDate = null;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ProductOrder.DeliveryDate.HasValue && ProductOrder.DeliveryDate.Value < new DateTime(9999,12,31))
                ProductOrder.PickupDate = new DateTime(9999, 12, 31);
            if (ProductOrder.PickupDate.HasValue && ProductOrder.PickupDate.Value < new DateTime(9999,12,31))
                ProductOrder.DeliveryDate = new DateTime(9999, 12, 31);

            ProductOrder.Confirmed = DateTime.Now;
            while (true)
            {
                ProductOrder.ConfirmationCode = GenerateConfirmationNumber();
                if (!await _context.ProductOrder.AnyAsync(a => a.ConfirmationCode == ProductOrder.ConfirmationCode && a.ContactEmail.Equals(ProductOrder.ContactEmail, StringComparison.OrdinalIgnoreCase)))
                {
                    break;
                }
            }

            //Create invoicing records
            var invoice = new Invoice()
            {
                InvoiceID = Guid.NewGuid(),
                ProductOrderID = ProductOrder.OrderID.Value,
                Created = DateTime.Now,
                Due = InvoiceHelper.GetDueDate(Number_of_days_till_due)
            };

            List<InvoiceTransaction> invoiceTransactions = await  _context
                                                                    .Product
                                                                    .Join(_context.CartItem,
                                                                            p => p.ProductID,
                                                                            ci => ci.ProductID,
                                                                            (p, ci) => new { p.ProductID, p.Price, p.Name, p.Description, ci.CartItemID, ci.CartID, ci.Quantity })
                                                                    .Where(w => w.CartID == ProductOrder.CartID)
                                                                    .Select(s => new InvoiceTransaction()
                                                                    {
                                                                        InvoiceID = invoice.InvoiceID,
                                                                        Type = InvoiceHelper.Transaction_Type_Debit,
                                                                        Category = InvoiceHelper.Transaction_Catgory_Product,
                                                                        Name = s.Name,
                                                                        Description = s.Description,
                                                                        Quantity = s.Quantity.Value,
                                                                        Price = s.Price.Value,
                                                                        Posted = DateTime.Now
                                                                    }).ToListAsync();

            invoiceTransactions.ConvertAll(c => c.InvoiceTransactionID = Guid.NewGuid());

            _context.Invoice.Add(invoice);
            _context.InvoiceTransaction.AddRange(invoiceTransactions);
            _context.ProductOrder.Update(ProductOrder);
            await _context.SaveChangesAsync();
            
            var url = Url.Page("/Orders/InvoicePrint", new { ProductOrder.OrderID });

            //var rslt = await ConfirmationHelper.SendConfirmation(_config,
            //                                                       _context,
            //                                                       ProductOrder,
            //                                                       HttpContext,
            //                                                       url);
            
            // Clear cart cookies
            HttpContext.Response.Cookies.Delete(littlebreadloaf.CartHelper.CartCookieName);
            HttpContext.Response.Cookies.Delete(littlebreadloaf.CartHelper.PreOrderCookie);

            return new RedirectToPageResult("/Cart/CartCheckoutConfirmation", new { ProductOrderID = ProductOrder.OrderID, ProductOrder.CartID });
        }

        private string GenerateConfirmationNumber()
        {
            var chars = "BCDFGHJKLMNPQRSTVWXYZ23456789"; //Remove vowels, zero, one, and I 
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

    }
}