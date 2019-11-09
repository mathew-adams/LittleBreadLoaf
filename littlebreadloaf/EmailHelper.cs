using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace littlebreadloaf
{
    public static class EmailHelper
    {
        const string Mailgun_Api_Key_Preamble = "api:";
        public static async Task<HttpResponseMessage> SendEmail(IConfiguration configuration,
                                                                string from,
                                                                string to,
                                                                string subject,
                                                                string message,
                                                                string attachmentName,
                                                                Stream attachment)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(configuration["Mailgun.Uri.Base"]) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                                                                                           Convert.ToBase64String(Encoding.ASCII.GetBytes(Mailgun_Api_Key_Preamble + configuration["Mailgun.Private.APIKey"])));

                var content = new MultipartFormDataContent
                {
                    { new StringContent(from), "from" },
                    { new StringContent(to), "to" },
                    { new StringContent(subject), "subject" },
                    { new StringContent(message), "html" }
                };

                if (!string.IsNullOrEmpty(configuration["LittleBreadLoad.InternalEmail"]))
                {
                    content.Add(new StringContent(configuration["LittleBreadLoad.InternalEmail"]), "bcc");
                }
                
                if (attachment != null)
                {
                    content.Add(CreateAttachmentContent(attachment, attachmentName, "application/pdf"));
                }

                return await client.PostAsync($"{configuration["Mailgun.Uri.Request"]}/messages",
                                              content).ConfigureAwait(false);
            }
        }

        private static StreamContent CreateAttachmentContent(Stream stream, string fileName, string contentType)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"attachment[]\"",
                FileName = "\"" + fileName + "\""
            }; // the extra quotes are key here
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return fileContent;
        }



    }

    public class MailGunResponse
    {
        public string message { get; set; }
        public string id { get; set; }
    }
}

