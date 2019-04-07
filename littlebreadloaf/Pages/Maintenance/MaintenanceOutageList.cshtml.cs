using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.ConfigurationProvider;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Maintenance
{
    [Authorize]
    public class MaintenanceOutageListModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenanceOutageListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<ProductOrderOutage> ProductOrderOutage { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            ProductOrderOutage = await _context.ProductOrderOutage.AsNoTracking().ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string OutageID)
        {
            if(string.IsNullOrEmpty(OutageID) || !Guid.TryParse(OutageID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Maintenance/MaintenanceOutageList");
            }
            var productOrderOutage = await _context.ProductOrderOutage.FirstOrDefaultAsync(f => f.OutageID == parsedID);

            if(productOrderOutage == null)
            {
                return new RedirectToPageResult("/Maintenance/MaintenanceOutageList");
            }

            _context.ProductOrderOutage.Remove(productOrderOutage);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Maintenance/MaintenanceOutageList");
        }
    }
}