using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace littlebreadloaf.Pages
{
    public class MarketsModel : PageModel
    {
        [BindProperty]
        public bool IsPreOrder { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            IsPreOrder = HttpContext.Request.Cookies[CartHelper.PreOrderCookie] != null;

            return new RedirectToPageResult("/Products/ProductList");
        }
    }
}