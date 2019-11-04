using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using SelectPdf;
using Newtonsoft.Json;

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

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(ProductOrderID) || !Guid.TryParse(ProductOrderID, out Guid productOrderID))
            {
                return new RedirectToPageResult("/Cart/CartView");
            }

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
            var absUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, url);

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

                //Send confirmation email
                var emailBody = _config["LittleBreadLoaf.ConfirmationEmailBody"].Replace("{{CONFIRMATION_CODE}}", ProductOrder.ConfirmationCode);
                var emailSubject = _config["LittleBreadLoaf.ConfirmationEmailSubject"].Replace("{{CONFIRMATION_CODE}}", ProductOrder.ConfirmationCode);
                var attachmentName = _config["LittleBreadLoaf.ConfirmationAttachmentName"].Replace("{{CONFIRMATION_CODE}}", ProductOrder.ConfirmationCode);

                var emailResponse = await EmailHelper.SendEmail(_config,
                                                                $"{_config["LittleBreadLoaf.Name"]} <mailgun@{_config["Mailgun.Uri.Request"]}>",
                                                                ProductOrder.ContactEmail,
                                                                emailSubject,
                                                                emailBody,
                                                                attachmentName,
                                                                msInvoice);
                var successful = true;
                var message = "";
                if (emailResponse.IsSuccessStatusCode)
                {
                    using (Stream receiveStream = await emailResponse.Content.ReadAsStreamAsync())
                    {
                        using (StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8))
                        {
                            var response = JsonConvert.DeserializeObject<MailGunResponse>(readStream.ReadToEnd());
                            successful = response.message.Contains("queued", StringComparison.OrdinalIgnoreCase);
                            message = response.message;
                        }
                    }
                }
                else
                {
                    successful = false;
                }

                if(!successful)
                {
                    var systemError = new SystemError()
                    {
                        ErrorID = Guid.NewGuid(),
                        RequestID = ProductOrder.ConfirmationCode,
                        Path = "CartCheckoutReview.SendEmail",
                        Error = $"Status:{emailResponse.StatusCode}, Message:{message}",
                        Occurred = DateTime.Now
                    };

                    _context.SystemError.Add(systemError);
                    await _context.SaveChangesAsync();
                }
            }            

            // Clear cart cookies
            HttpContext.Response.Cookies.Delete(littlebreadloaf.CartHelper.CartCookieName);
            
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