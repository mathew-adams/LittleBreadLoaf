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
    public class MaintenanceSystemValueEditModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenanceSystemValueEditModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string LittleBreadLoafSystemKey { get; set; }

        [BindProperty]
        public LittleBreadLoafSystem LittleBreadLoafSystem { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {

            if(string.IsNullOrEmpty(LittleBreadLoafSystemKey))
            {
                return new RedirectToPageResult("/Maintenance/MaintenanceList");
            }

            LittleBreadLoafSystem = await _context.LittleBreadLoafSystem.FirstOrDefaultAsync(l => l.Key == LittleBreadLoafSystemKey);
            if(LittleBreadLoafSystem == null)
            {
                return new RedirectToPageResult("/Maintenance/MaintenanceList");
            }


            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            _context.LittleBreadLoafSystem.Update(LittleBreadLoafSystem);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Maintenance/MaintenanceSystemValueList");
        }
    }
}