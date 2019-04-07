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
    public class MaintenanceSystemValueListModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenanceSystemValueListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<LittleBreadLoafSystem> LittleBreadLoafSystem { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            LittleBreadLoafSystem = await _context.LittleBreadLoafSystem.AsNoTracking().ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDefaultAsync()
        {

            var defaultValues = new List<LittleBreadLoafSystem>()
            {
                new LittleBreadLoafSystem(){Key = "Google.Recaptcha.SecretKey", Value = ""},
                new LittleBreadLoafSystem(){Key = "Google.Recaptcha.SiteKey", Value = ""},
                new LittleBreadLoafSystem(){Key = "Mailgun.Private.APIKey", Value = ""},
                new LittleBreadLoafSystem(){Key = "Mailgun.Public.ValidationKey", Value = ""},
                new LittleBreadLoafSystem(){Key = "Mailgun.Uri.Base", Value = ""},
                new LittleBreadLoafSystem(){Key = "Mailgun.Uri.Request", Value = ""}
            };

            foreach(var defaultValue in defaultValues)
            {
                if(! await _context.LittleBreadLoafSystem.AnyAsync(a => a.Key == defaultValue.Key))
                {
                    _context.LittleBreadLoafSystem.Add(defaultValue);
                }
            }

            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Maintenance/MaintenanceSystemValueList");
        }

    }
}