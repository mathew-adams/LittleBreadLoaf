using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace littlebreadloaf.Pages.Service
{
    [Authorize]
    public class ServiceModel : PageModel
    {
        private readonly ProductContext _context;
        private readonly IConfiguration _config;
        public ServiceModel(ProductContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty]
        public littlebreadloaf.Data.Service Service { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> ParkingOptions { get; set; }

        
        public async Task<IActionResult> OnGetAsync()
        {
            ParkingOptions = new SelectList(new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "True", Value = "Yes", Selected = false },
                new SelectListItem(){ Text = "False", Value = "No", Selected = false }
            }, "Text", "Value", null);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {


            return Page();
        }
    }
}
