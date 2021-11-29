using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text;
namespace littlebreadloaf.Extensions
{
    public class EmailSettings
    {
        public string ApiKey { get; set; }
        public string ApiBaseUri { get; set; }
        public string RequestUri { get; set; }
        public string From { get; set; }
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        public EmailSender(IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
        }

        const string Mailgun_Api_Key_Preamble = "api:";
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(_emailSettings.ApiBaseUri) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                                                                                           Convert.ToBase64String(Encoding.ASCII.GetBytes(Mailgun_Api_Key_Preamble + _emailSettings.ApiKey)));
                var content = new MultipartFormDataContent
                {
                    { new StringContent(_emailSettings.From), "from" },
                    { new StringContent(email), "to" },
                    { new StringContent(subject), "subject" },
                    { new StringContent(htmlMessage), "html" }
                };

                await client.PostAsync($"{_emailSettings.RequestUri}/messages", content).ConfigureAwait(false);
            }
        }
    }
}

