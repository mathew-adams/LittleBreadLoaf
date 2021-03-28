using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;

namespace littlebreadloaf.Pages.Maintenance
{
    public class MaintenancePreOrderListModel : PageModel
    {
        private readonly ProductContext _context;
        public MaintenancePreOrderListModel(ProductContext context)
        {
            _context = context;
        }


        [BindProperty]
        public List<PreOrdersWithAddresses> PreOrderSources { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {

            PreOrderSources =   await ( from pos in _context.PreOrderSource
                                        join add in _context.NzAddressDeliverable on pos.AddressID equals add.address_id
                                        select new PreOrdersWithAddresses() 
                                        { 
                                            PreOrderSource = pos,
                                            Address = add.full_address
                                        }).ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostClearAsync()
        {
            HttpContext.Response.Cookies.Delete(CartHelper.PreOrderCookie);
            return new RedirectToPageResult("/Products/ProductList");
        }

    }
    public class PreOrdersWithAddresses
    {
        public PreOrderSource PreOrderSource { get; set; }
        public string Address { get; set; }
    }
}
