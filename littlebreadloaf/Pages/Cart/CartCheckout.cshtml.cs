using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace littlebreadloaf.Pages.Cart
{
    public class CartCheckoutModel : PageModel
    {
        private readonly ProductContext _context;
        private readonly IConfiguration _config;
        public CartCheckoutModel(ProductContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty]
        public littlebreadloaf.Data.Cart Cart { get; set; }
        
        [BindProperty]
        public ProductOrder ProductOrder { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> PaymentMethod { get; set; }

        [BindProperty]
        public string Full_Address { get; set; }

        [BindProperty]
        public string GoogleRecaptchaToken { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var cartID = HttpContext.Request.Cookies[CartHelper.CartCookieName];

            if(string.IsNullOrEmpty(cartID))
            {
                return new RedirectResult("/Cart/CartView");
            }

            Guid.TryParse(cartID, out Guid parsedCartID);
            ProductOrder = await _context.ProductOrder.FirstOrDefaultAsync(f => f.CartID == parsedCartID);

            if(ProductOrder != null && ProductOrder.ContactAddress > 0)
            {
                var nzAddress = await _context.NzAddressDeliverable.FirstOrDefaultAsync(f => f.address_id == ProductOrder.ContactAddress);
                if(nzAddress != null)
                {
                    Full_Address = nzAddress.full_address;
                }
            }

            PaymentMethod = new SelectList(new List<SelectListItem>()
                                {
                                    new SelectListItem(){ Text = "CASH", Value = "Cash - on delivery / pickup", Selected = false },
                                    new SelectListItem(){ Text = "EFTPOS", Value = "EFTPOS - on delivery / pickup - no credit cards", Selected = false },
                                    new SelectListItem(){ Text = "BANK", Value = "Bank transfer", Selected = false }
                                },"Text","Value",null);
            
            return Page();
        }

        public JsonResult OnGetAddressSearch(string addressFilter)
        {
            var addresses = _context.NzAddressDeliverable
                                    .Where(w => w.full_address.Contains(addressFilter, StringComparison.OrdinalIgnoreCase))
                                    .Take(10)
                                    .Select(s => new
                                    {
                                        s.address_id,
                                        s.full_address
                                    })
                                    .ToList();
            return new JsonResult(addresses);
        }

        public async Task<IActionResult> OnPostAsync()
        {

            PaymentMethod = new SelectList(new List<SelectListItem>()
                                {
                                    new SelectListItem(){ Text = "CASH", Value = "Cash - on delivery", Selected = false },
                                    new SelectListItem(){ Text = "EFTPOS", Value = "EFTPOS - on delivery - no credit cards", Selected = false },
                                    new SelectListItem(){ Text = "BANK", Value = "Bank transfer", Selected = false }
                                }, "Text", "Value", null);

            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors);

                return Page();
            }
            
            if(ProductOrder.Pickup)
            {
                ProductOrder.ContactAddress = 0;
            }
            else
            {
                if(ProductOrder.ContactAddress == 0)
                {
                    ModelState.AddModelError("Address.Missing", "Please select an address to be delivered to.");
                    return Page();
                }
            }

            var cartID = HttpContext.Request.Cookies[CartHelper.CartCookieName];
            if (string.IsNullOrEmpty(cartID) || !Guid.TryParse(cartID, out Guid parsedCartID))
            {
                return new RedirectResult("/Cart/CartView");
            }

            if(!await _context.Cart.AnyAsync(a => a.CartID == parsedCartID))
            {
                return new RedirectResult("/Cart/CartView");
            }
            if(string.IsNullOrEmpty(GoogleRecaptchaToken))
            {
                ModelState.AddModelError("Google.Recaptcha.TokenMissing", "You are missing a token.");
                return Page();
            }
            if(!await ValidateGoogleRecaptchaAsync(HttpContext.Connection.RemoteIpAddress.ToString(), GoogleRecaptchaToken))
            {
                ModelState.AddModelError("Google.Recaptcha.TokenInvalid", "You are not a human!");
                return Page();
            }

            if (await _context.ProductOrder.AnyAsync(f => f.CartID == parsedCartID))
            {
                ProductOrder.CartID = parsedCartID;
                ProductOrder.ContactEmail = ProductOrder.ContactEmail.ToLower();
                ProductOrder.Payment = new DateTime(9999, 12, 31);
                ProductOrder.Confirmed = new DateTime(9999, 12, 31);
                ProductOrder.ConfirmationCode = ""; //Default to empty, this will be set when checked out

                _context.ProductOrder.Update(ProductOrder);
            }
            else
            {
                ProductOrder.ContactEmail = ProductOrder.ContactEmail.ToLower();
                ProductOrder.OrderID = Guid.NewGuid();
                ProductOrder.CartID = parsedCartID;
                ProductOrder.Payment = new DateTime(9999, 12, 31);
                ProductOrder.Confirmed = new DateTime(9999, 12, 31);
                ProductOrder.Created = DateTime.Now;
                ProductOrder.ConfirmationCode = ""; //Default to empty, this will be set when checked out

                _context.ProductOrder.Add(ProductOrder);
            }
            
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Cart/CartCheckoutReview", new { ProductOrderID = ProductOrder.OrderID, CartID = ProductOrder.CartID });
        }

        private async Task<bool> ValidateGoogleRecaptchaAsync(string ipAddress, string recaptchaResponse)
        {
            //Validate google recaptcha
            using (var client = new HttpClient { BaseAddress = new Uri("https://www.google.com") })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", _config["Google.Recaptcha.SecretKey"]),
                    new KeyValuePair<string, string>("response", recaptchaResponse),
                    new KeyValuePair<string, string>("remoteip", ipAddress)
                });
                var result = await client.PostAsync("/recaptcha/api/siteverify", content);
                result.EnsureSuccessStatusCode();
                string jsonString = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<RecaptchaResponse>(jsonString);
                return response.Success;
            }
        }


        [DataContract]
        internal class RecaptchaResponse
        {
            [DataMember(Name = "success")]
            public bool Success { get; set; }
            [DataMember(Name = "score")]
            public decimal Score { get; set; }
            [DataMember(Name = "action")]
            public string Action { get; set; }
            [DataMember(Name = "challenge_ts")]
            public DateTime ChallengeTimeStamp { get; set; }
            [DataMember(Name = "hostname")]
            public string Hostname { get; set; }
            [DataMember(Name = "error-codes")]
            public IEnumerable<string> ErrorCodes { get; set; }
        }


       // {
       //   "success": true|false,      // whether this request was a valid reCAPTCHA token for your site
       //   "score": number             // the score for this request (0.0 - 1.0)
       //   "action": string            // the action name for this request (important to verify)
       //   "challenge_ts": timestamp,  // timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)
       //   "hostname": string,         // the hostname of the site where the reCAPTCHA was solved
       //   "error-codes": [...]        // optional
       // }


}
}