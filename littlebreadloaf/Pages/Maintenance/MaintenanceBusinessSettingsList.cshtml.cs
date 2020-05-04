using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Pages.Maintenance
{
    [Authorize]
    public class MaintenanceBusinessSettingsListModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenanceBusinessSettingsListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BusinessSettings BusinessSettings { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            
            if(!await _context.BusinessSettings.AnyAsync(a=>a.BusinessSettingID != Guid.Empty))
            {
                BusinessSettings = new BusinessSettings()
                { 
                    BusinessSettingID = Guid.NewGuid(),
                    DeliveryEnabled = true, 
                    PickupEnabled = true, 
                    MinimumDeliveryOrderAmount = 50,
                    MinimumPickupOrderAmount = 50
                };

                await _context.BusinessSettings.AddAsync(BusinessSettings);
                await _context.SaveChangesAsync();
            }
            else
            {
                BusinessSettings = await _context.BusinessSettings.AsNoTracking().FirstOrDefaultAsync();
            }

            return Page();
        }
    }
}
