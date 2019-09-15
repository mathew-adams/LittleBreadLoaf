using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Authorization;

namespace littlebreadloaf.Pages.Maintenance
{
    [Authorize]
    public class MaintenanceErrorListModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenanceErrorListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<SystemError> SystemErrors { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? From { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? To { get; set; }

        [BindProperty(SupportsGet = true)]
        public String SearchTerm { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!From.HasValue)
                From = DateTime.Now.Date.AddDays(-7);
            if (!To.HasValue)
                To = DateTime.Now.Date;

            if(SearchTerm != null)
            {
                SystemErrors = await _context
                                    .SystemError
                                    .AsNoTracking()
                                    .Where(w => w.Occurred >= From 
                                                && w.Occurred <= To 
                                                && w.Error.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                                    .ToListAsync();
            }else
            {
                SystemErrors = await _context
                                    .SystemError
                                    .AsNoTracking()
                                    .Where(w => w.Occurred >= From 
                                                && w.Occurred <= To)
                                    .ToListAsync();
            }

            return Page();
        }
    }
}