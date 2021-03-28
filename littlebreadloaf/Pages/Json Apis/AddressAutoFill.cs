using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Json_Apis
{
    [Route("api/address-auto-fill")]
    [ApiController]
    [Produces("application/json")]
    public class AddressAutoFill : ControllerBase
    {
        private readonly ProductContext _context;
        public AddressAutoFill(ProductContext context)
        {
            _context = context;
        }

        [HttpGet]
        public JsonResult Get(string addressFilter)
        {
            var addresses = _context.NzAddressDeliverable
                                    .Where(w => w.full_address.Contains(addressFilter, StringComparison.OrdinalIgnoreCase))
                                    .Take(10)
                                    .Select(s => new
                                    {
                                        s.address_id,
                                        s.full_address
                                    })
                                    .ToList();
            return new JsonResult(addresses);
        }
    }
}
