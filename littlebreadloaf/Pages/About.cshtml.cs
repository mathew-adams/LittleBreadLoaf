using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
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

        public void OnGet()
        {
            //Doesn't need to be async

            ViewData["MinumumDelivery"] = _config["LittleBreadLoad.MinimumDelivery"];

        }
    }
}
