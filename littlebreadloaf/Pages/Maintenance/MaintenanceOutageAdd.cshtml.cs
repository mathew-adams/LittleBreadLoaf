using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.ConfigurationProvider;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Authorization;

namespace littlebreadloaf.Pages.Maintenance
{
    [Authorize]
    public class MaintenanceOutageAddModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenanceOutageAddModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductOrderOutage ProductOrderOutage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string OutageID { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //if(string.IsNullOrEmpty(OutageID) || !Guid.TryParse(OutageID,out Guid parsedID))
            //{
            //    return new RedirectToPageResult("/Maintenance/MaintenanceOutageList");
            //}

            //ProductOrderOutage = await _context.ProductOrderOutage.FirstOrDefaultAsync(o => o.OutageID == parsedID);

            //if(ProductOrderOutage == null)
            //{
            //    return new RedirectToPageResult("/Maintenance/MaintenanceOutageList");
            //}

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            if(ProductOrderOutage.Start > ProductOrderOutage.Stop)
            {
                ModelState.AddModelError("Outage.StartLessThanStop", "Start date cannot be before the stop date.");
                return Page();
            }

            ProductOrderOutage.OutageID = Guid.NewGuid();

            _context.ProductOrderOutage.Add(ProductOrderOutage);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Maintenance/MaintenanceOutageList");
        }

    }
}