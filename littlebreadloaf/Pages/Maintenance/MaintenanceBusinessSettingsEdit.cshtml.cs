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
    public class MaintenanceBusinessSettingsEditModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenanceBusinessSettingsEditModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BusinessSettings BusinessSettings { get; set; }

        public async Task<IActionResult> OnGetAsync(string businessSettingID)
        {
            if (String.IsNullOrEmpty(businessSettingID) || !Guid.TryParse(businessSettingID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Maintenance/MaintenanceBusinessSettingsList");
            }

            BusinessSettings = await _context.BusinessSettings.FirstOrDefaultAsync(w => w.BusinessSettingID == parsedID);

            if (BusinessSettings == null)
            {
                return new RedirectToPageResult("/Maintenance/MaintenanceBusinessSettingsList");
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.BusinessSettings.Update(BusinessSettings);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Maintenance/MaintenanceBusinessSettingsList");
        }
    }
}
