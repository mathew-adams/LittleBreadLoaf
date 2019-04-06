using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using littlebreadloaf.Data;

namespace littlebreadloaf.ViewComponents
{
    public class OrderViewComponent : ViewComponent
    {
        private readonly ProductContext _context;
        public OrderViewComponent(ProductContext context)
        {
            _context = context;
        }
        
        public class ProductOrderAndAddress
        {
            public ProductOrder ProductOrder { get; set; }
            public NzAddressDeliverable NzAddressDeliverable { get; set; }
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid orderID)
        {
            var order = await _context.ProductOrder.FirstOrDefaultAsync(w => w.OrderID == orderID);

            var address = await _context.NzAddressDeliverable.FirstOrDefaultAsync(f => f.address_id == order.ContactAddress);

            return View(new ProductOrderAndAddress()
            {
                ProductOrder = order,
                NzAddressDeliverable = address
            });
        }
    }
}