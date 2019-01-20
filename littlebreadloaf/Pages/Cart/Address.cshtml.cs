using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Cart
{
    public class AddressModel : PageModel
    {
        private readonly AddressContext _context;
        public AddressModel(AddressContext context)
        {
            _context = context;
        }
        [ValidateAntiForgeryToken]
        public JsonResult OnGetCartList(string addressFilter) 
        {
            List<NzAddressDeliverable> addresses = _context.NzAddressDeliverable
                                                           .Where(w => w.full_address.Contains(addressFilter))
                                                           .Take(50)
                                                           .ToList();
            return new JsonResult(addresses);
        }
    }
}