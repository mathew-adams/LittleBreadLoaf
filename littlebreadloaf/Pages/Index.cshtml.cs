using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace littlebreadloaf.Pages
{
    public class IndexModel : PageModel
    {

        private readonly ProductContext _context;
        private readonly IConfiguration _config;
        public IndexModel(ProductContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var source = HttpContext.Request.Cookies[CartHelper.PreOrderCookie];

            if (source != null)
            {
                new RedirectToPageResult("/Products/ProductPreOrder",new { Source = source });
            }
            //Doesn't need to be async
            return new RedirectToPageResult("/Products/ProductList");


        }
    }
}
