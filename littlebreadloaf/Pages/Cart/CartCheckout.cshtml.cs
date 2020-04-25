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
        public IEnumerable<SelectListItem> PaymentMethodOptions { get; set; }

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

            if(ProductOrder != null)
            {
                if (ProductOrder.DeliveryDate == new DateTime(9999, 12, 31))
                    ProductOrder.DeliveryDate = null;
                if (ProductOrder.PickupDate == new DateTime(9999, 12, 31))
                    ProductOrder.PickupDate = null;
            }
            else
            {
                ProductOrder = new ProductOrder();
                ProductOrder.DeliveryTime = "Between 14:00 and 18:00";

            }

            PaymentMethodOptions = new SelectList(new List<SelectListItem>()
            {
                //new SelectListItem(){ Text = "CASH", Value = "Cash - on delivery / pickup", Selected = false },
                //new SelectListItem(){ Text = "EFTPOS", Value = "EFTPOS - on delivery / pickup - no credit cards", Selected = false },
                new SelectListItem(){ Text = "BANK", Value = "Bank transfer (only option during level 3)", Selected = false }
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
            var validDeliveryDaysOfWeek = new List<DayOfWeek>()
            {
                DayOfWeek.Friday
            };

            var validPickupDaysOfWeek = new List<DayOfWeek>()
            {
                DayOfWeek.Friday
            };

            PaymentMethodOptions = new SelectList(new List<SelectListItem>()
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

            var cartID = HttpContext.Request.Cookies[CartHelper.CartCookieName];
            if (string.IsNullOrEmpty(cartID) || !Guid.TryParse(cartID, out Guid parsedCartID))
            {
                return new RedirectResult("/Cart/CartView");
            }

            //DATE
            if (!ProductOrder.PickupDate.HasValue && !ProductOrder.DeliveryDate.HasValue)
            {
                ModelState.AddModelError("Validation.DeliveryOrPickup", "Choose either a delivery date or pickup date.");
                return Page();
            }

            var validPickupDate = (ProductOrder.PickupDate.HasValue && ProductOrder.PickupDate.Value < new DateTime(9999, 12, 31));
            var validDeliveryDate = (ProductOrder.DeliveryDate.HasValue && ProductOrder.DeliveryDate.Value < new DateTime(9999, 12, 31));

            if(validDeliveryDate && validPickupDate)
            {
                ModelState.AddModelError("Validation.DeliveryOrPickup", "Choose either a delivery date or pickup date.");
                return Page();
            }

            if (validPickupDate && ProductOrder.PickupDate.Value < DateTime.Now)
            {
                ModelState.AddModelError("Validation.PickupDateInPast", "Pickup date cannot be in the past.");
                return Page();
            }

            if (validDeliveryDate && ProductOrder.DeliveryDate.Value < DateTime.Now)
            {
                ModelState.AddModelError("Validation.DeliveryDateInPast", "Delivery date cannot be in the past.");
                return Page();
            }

            //TIME
            if (validPickupDate && string.IsNullOrEmpty(ProductOrder.PickupTime))
            {
                ModelState.AddModelError("Validation.PickupTimeRequired", "A pickup time is required.");
                return Page();
            }

            if(validDeliveryDate && string.IsNullOrEmpty(ProductOrder.DeliveryTime))
            {
                //ModelState.AddModelError("Validation.DeliveryTimeRequired", "A delivery time is required.");
                //return Page();
                ProductOrder.DeliveryTime = "";
            }

            if(validPickupDate) 
            {
                ProductOrder.ContactAddress = 0; // If pickup, don't worry about a delivery address

                var dayOfWeek = ProductOrder.PickupDate.Value.DayOfWeek;
                if (!validPickupDaysOfWeek.Contains(dayOfWeek))
                {
                    ModelState.AddModelError("Validation.PickupDayOfWeek", "Pickup date must be Friday.");
                    return Page();
                }
            }

            if(validDeliveryDate) //Delivery must have an address
            {
                if(ProductOrder.ContactAddress == null || ProductOrder.ContactAddress == 0)
                {
                    ModelState.AddModelError("Address.Missing", "Begin typing your address to validate.");
                    return Page();
                }

                if (!Decimal.TryParse(_config["LittleBreadLoad.MinimumDelivery"], out decimal minDeliveryAmount))
                {
                    throw new Exception("LittleBreadLoad.MinimumDelivery is not configured.");
                }

                var dayOfWeek = ProductOrder.DeliveryDate.Value.DayOfWeek;
                if (!validDeliveryDaysOfWeek.Contains(dayOfWeek))
                {
                    ModelState.AddModelError("Validation.DeliveryDayOfWeek", "Delivery date must be Friday.");
                    return Page();
                }

                var cartItems = await _context
                                        .CartItem
                                        .Where(w => w.CartID == parsedCartID)
                                        .Select(s => new { s.Price, s.Quantity })
                                        .ToListAsync();

                if (cartItems.Sum(s => s.Price * s.Quantity) < minDeliveryAmount)
                {
                    ModelState.AddModelError("Validation.DeliveryMinNotMet", $"The minimum delivery amount is ${_config["LittleBreadLoad.MinimumDelivery"]}");
                    return Page();
                }
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

            if (ProductOrder.DeliveryInstructions == null)
                ProductOrder.DeliveryInstructions = "";

            //Determine if pickup or delivery
            if (ProductOrder.DeliveryDate.HasValue)
            {
                ProductOrder.PickupDate = new DateTime(9999, 12, 31);
                ProductOrder.PickupTime = "";
            }else if(ProductOrder.PickupDate.HasValue)
            {
                ProductOrder.DeliveryDate = new DateTime(9999, 12, 31);
                ProductOrder.DeliveryTime = "";
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