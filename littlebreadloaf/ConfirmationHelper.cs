using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Http;
using SelectPdf;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using littlebreadloaf.Services;
using littlebreadloaf.ViewComponents;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Pages.Cart;
using QuestPDF.Fluent;

namespace littlebreadloaf
{
    public static class ConfirmationHelper
    {
        public static async Task<ObjectResult> SendConfirmation(IConfiguration config,
                                                                Invoice invoice,
                                                                List<InvoiceTransaction> invoiceTransactions,
                                                                ProductOrder order,
                                                                ProductContext context,
                                                                IHostingEnvironment env)
        {
            //Build PDF
            var invoiceView = new InvoiceModel();

            invoiceView.Name = config["LittleBreadLoaf.Name"];
            invoiceView.AddressLine1 = config["LittleBreadLoaf.AddressLine1"];
            invoiceView.AddressLine2 = config["LittleBreadLoaf.AddressLine2"];
            invoiceView.BankNumber = config["LittleBreadLoaf.BankNumber"];
            invoiceView.Phone = config["LittleBreadLoaf.Phone"];
            invoiceView.ProductOrder = await context.ProductOrder.FirstOrDefaultAsync(p => p.OrderID == order.OrderID);
            invoiceView.HasAddress = false;
            if (invoiceView.ProductOrder.ContactAddress > 0)
            {
                invoiceView.NzAddressDeliverable = await context.NzAddressDeliverable.FirstOrDefaultAsync(a => a.address_id == invoiceView.ProductOrder.ContactAddress);
                invoiceView.HasAddress = true;
            }
            invoiceView.Invoice = await context.Invoice.FirstOrDefaultAsync(f => f.ProductOrderID == order.OrderID);
            invoiceView.InvoiceTransactions = await context
                                                    .InvoiceTransaction
                                                    .AsNoTracking()
                                                    .Where(w => w.InvoiceID == invoiceView.Invoice.InvoiceID)
                                                    .ToListAsync();

            invoiceView.Balance = invoiceView.InvoiceTransactions.Sum(s => s.Quantity * s.Price);
            invoiceView.Status = (invoiceView.Balance == 0) ? "PAID" : "DUE";

            using (var msInvoice = new System.IO.MemoryStream())
            {
                var document = new InvoiceDocument(invoiceView, invoiceView.GetLogoUrl(env));

                document.GeneratePdf(msInvoice);
                msInvoice.Position = 0;

                //Send confirmation email
                var emailBody = config["LittleBreadLoaf.ConfirmationEmailBody"].Replace("{{CONFIRMATION_CODE}}", order.ConfirmationCode);
                var emailSubject = config["LittleBreadLoaf.ConfirmationEmailSubject"].Replace("{{CONFIRMATION_CODE}}", order.ConfirmationCode);
                var attachmentName = config["LittleBreadLoaf.ConfirmationAttachmentName"].Replace("{{CONFIRMATION_CODE}}", order.ConfirmationCode);

                var emailResponse = await EmailHelper.SendEmail(config,
                                                                $"{config["LittleBreadLoaf.Name"]} <mailgun@{config["Mailgun.Uri.Request"]}>",
                                                                order.ContactEmail,
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

                if (!successful)
                {
                    var systemError = new SystemError()
                    {
                        ErrorID = Guid.NewGuid(),
                        RequestID = order.ConfirmationCode,
                        Path = "CartCheckoutReview.SendEmail",
                        Error = $"Status:{emailResponse.StatusCode}, Message:{message}",
                        Occurred = DateTime.Now
                    };

                    context.SystemError.Add(systemError);
                    await context.SaveChangesAsync();
                    return new UnprocessableEntityObjectResult(systemError);
                }
            }
            return new OkObjectResult("OK");
        
        }
    }
}
