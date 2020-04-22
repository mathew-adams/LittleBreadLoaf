using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace littlebreadloaf.Pages.Orders
{

    [Authorize]
    public class OrderConfirmationResendModel : PageModel
    {

        private readonly ProductContext _context;
        private readonly IConfiguration _config;

        public OrderConfirmationResendModel(ProductContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public string OrderID { get; set; }

        [BindProperty]
        public ProductOrder ProductOrder { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (String.IsNullOrEmpty(OrderID) || !Guid.TryParse(OrderID, out Guid parsedID))
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            ProductOrder = await _context.ProductOrder.FirstOrDefaultAsync(m => m.OrderID == parsedID);
            if (ProductOrder == null)
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if(ProductOrder.OrderID == Guid.Empty)
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            //TODO add centralized logic here
            var url = Url.Page("/Orders/InvoicePrint", new { ProductOrder.OrderID });
            var rslt = await ConfirmationHelper.SendConfirmation(_config, 
                                                                 _context, 
                                                                 ProductOrder, 
                                                                 HttpContext, 
                                                                 url);

            return new RedirectToPageResult("/Orders/OrderView", new { ProductOrder.OrderID, ResendEmailSuccess = rslt.StatusCode==200 });
        }
    }
}
