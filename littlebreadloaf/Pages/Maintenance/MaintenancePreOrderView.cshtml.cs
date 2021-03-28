using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;

namespace littlebreadloaf.Pages.Maintenance
{
    public class MaintenancePreOrderViewModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenancePreOrderViewModel(ProductContext context)
        {
            _context = context;
        }
        [BindProperty(SupportsGet = true)]
        public string Source { get; set; }

        [BindProperty]
        public PreOrderSource PreOrderSource { get; set; }

        [BindProperty]
        public string Full_Address { get; set; }

        [BindProperty]
        public string FullUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Source))
                return new RedirectToPageResult("/Maintenance/MaintenancePreOrderList");
            
            PreOrderSource = await _context.PreOrderSource.Where(w => w.Source == Source).FirstOrDefaultAsync();
            if(PreOrderSource == null)
                return new RedirectToPageResult("/Maintenance/MaintenancePreOrderList");

            var address = await _context.NzAddressDeliverable.Where(w => w.address_id == PreOrderSource.AddressID).FirstOrDefaultAsync();
            Full_Address = address.full_address;

            FullUrl = string.Format("{0}://{1}/Products/ProductPreOrder?Source={2}", 
                                    HttpContext.Request.Scheme, 
                                    HttpContext.Request.Host, 
                                    PreOrderSource.Source);

            return Page();
        }
    }
}
