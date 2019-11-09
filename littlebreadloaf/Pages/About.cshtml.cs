using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace littlebreadloaf.Pages
{
    public class AboutModel : PageModel
    {
        private readonly ProductContext _context;
        private readonly IConfiguration _config;
        public AboutModel(ProductContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["MinumumDelivery"] = _config["LittleBreadLoad.MinimumDelivery"];
            return Page();
        }
    }
}
