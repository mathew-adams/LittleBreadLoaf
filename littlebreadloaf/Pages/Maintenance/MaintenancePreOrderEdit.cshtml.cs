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
    public class MaintenancePreOrderEditModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenancePreOrderEditModel(ProductContext context)
        {
            _context = context;
        }
        [BindProperty(SupportsGet = true)]
        public string Source { get; set; }

        [BindProperty]
        public PreOrderSource PreOrderSource { get; set; }

        [BindProperty]
        public string Full_Address { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Source))
                return new RedirectToPageResult("/Maintenance/MaintenancePreOrderList");

            PreOrderSource = await _context.PreOrderSource.Where(w => w.Source == Source).FirstOrDefaultAsync();
            if (PreOrderSource == null)
                return new RedirectToPageResult("/Maintenance/MaintenancePreOrderList");

            var address = await _context.NzAddressDeliverable.Where(w => w.address_id == PreOrderSource.AddressID).FirstOrDefaultAsync();
            Full_Address = address.full_address;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.PreOrderSource.Update(PreOrderSource);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Maintenance/MaintenancePreOrderView", new { Source = PreOrderSource.Source });

        }
    }
}
