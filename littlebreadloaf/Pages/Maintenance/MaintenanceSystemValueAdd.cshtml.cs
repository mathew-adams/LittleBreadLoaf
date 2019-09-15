using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Authorization;
using littlebreadloaf.ConfigurationProvider;

namespace littlebreadloaf.Pages.Maintenance
{
    [Authorize]
    public class MaintenanceSystemValueAddModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenanceSystemValueAddModel(ProductContext context)
        {
            _context = context;
        }
        
        [BindProperty]
        public LittleBreadLoafSystem LittleBreadLoafSystem { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.LittleBreadLoafSystem.Add(LittleBreadLoafSystem);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Maintenance/MaintenanceSystemValueList");
        }
    }
}