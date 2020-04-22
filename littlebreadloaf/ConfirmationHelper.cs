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

namespace littlebreadloaf
{
    public static class ConfirmationHelper
    {
        public static async Task<ObjectResult> SendConfirmation(IConfiguration config,
                                                                ProductContext context,
                                                                ProductOrder order,
                                                                HttpContext http,
                                                                string url)
        {

            var absUrl = string.Format("{0}://{1}{2}", http.Request.Scheme, http.Request.Host, url);

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
                return new OkObjectResult("OK");
            }
        }
    }
}
