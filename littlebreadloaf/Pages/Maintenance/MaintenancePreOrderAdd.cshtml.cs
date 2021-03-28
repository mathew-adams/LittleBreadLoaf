using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace littlebreadloaf.Pages.Maintenance
{
    [Authorize]
    public class MaintenancePreOrderAddModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenancePreOrderAddModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PreOrderSource PreOrderSource { get; set; }

        [BindProperty]
        public string Full_Address { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                if(string.IsNullOrEmpty(Full_Address))
                {
                    ModelState.AddModelError("AddressRequired", "A delivery address is required.");
                }
                return Page();
            }

            if(await _context.PreOrderSource.AnyAsync(a => a.Source.Equals(PreOrderSource.Source, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Duplicate", "The pre-order source name is already taken.");
                return Page();
            }


            PreOrderSource.Active = true;
            PreOrderSource.Created = DateTime.Now;

            _context.PreOrderSource.Add(PreOrderSource);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Maintenance/MaintenancePreOrderList");
        }
    }
}
