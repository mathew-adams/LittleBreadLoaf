using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;

namespace littlebreadloaf.Pages.Json_Apis
{
    [Route("api/dates")]
    [ApiController]
    [Produces("application/json")]
    public class Dates : ControllerBase
    {
        private readonly ProductContext _context;
        public Dates(ProductContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<JsonResult> Get(string type)
        {

            var settings = await _context.BusinessSettings.AsNoTracking().FirstOrDefaultAsync();
            var dates = new List<BusinessDate>();

            if (settings == null)
                return new JsonResult(dates);

            var pickup = new List<string>();
            if (settings.PickupSunday) pickup.Add("Sunday");
            if (settings.PickupMonday) pickup.Add("Monday");
            if (settings.PickupTuesday) pickup.Add("Tuesday");
            if (settings.PickupWednesday) pickup.Add("Wednesday");
            if (settings.PickupThursday) pickup.Add("Thursday");
            if (settings.PickupFriday) pickup.Add("Friday");
            if (settings.PickupSaturday) pickup.Add("Saturday");

            var delivery = new List<string>();
            if (settings.DeliverSunday) delivery.Add("Sunday");
            if (settings.DeliverMonday) delivery.Add("Monday");
            if (settings.DeliverTuesday) delivery.Add("Tuesday");
            if (settings.DeliverWednesday) delivery.Add("Wednesday");
            if (settings.DeliverThursday) delivery.Add("Thursday");
            if (settings.DeliverFriday) delivery.Add("Friday");
            if (settings.DeliverSaturday) delivery.Add("Saturday");

            if(!string.IsNullOrEmpty(type))
            {
                var preOrders = await _context.PreOrderSource
                                                .AsNoTracking()
                                                .Where(w => w.Source == type)
                                                .ToListAsync();

                foreach (var preOrder in preOrders)
                {
                    var preOrderDates = new List<string>();
                    if (preOrder.Sunday) preOrderDates.Add("Sunday");
                    if (preOrder.Monday) preOrderDates.Add("Monday");
                    if (preOrder.Tuesday) preOrderDates.Add("Tuesday");
                    if (preOrder.Wednesday) preOrderDates.Add("Wednesday");
                    if (preOrder.Thursday) preOrderDates.Add("Thursday");
                    if (preOrder.Friday) preOrderDates.Add("Friday");
                    if (preOrder.Saturday) preOrderDates.Add("Saturday");
                    dates.Add(new BusinessDate() { type = preOrder.Source, dates = preOrderDates });
                }
            }

            dates.Add(new BusinessDate() { type = "PICKUP", dates = pickup });
            dates.Add(new BusinessDate() { type = "DELIVERY", dates = delivery });

            return new JsonResult(dates);
        }

    }
    public class BusinessDate
    {
        public string type { get; set; }
        public List<string> dates { get; set; }
    }
}
