using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.ConfigurationProvider;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Maintenance
{
    [Authorize]
    public class MaintenanceListModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenanceListModel(ProductContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }


    }
}