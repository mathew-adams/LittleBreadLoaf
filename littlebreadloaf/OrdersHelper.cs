using littlebreadloaf.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace littlebreadloaf
{
    public class ActiveOrders
    {
        public DateTime? Created { get; set; }
        public DateTime? Confirmed { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryInstructions { get; set; }
        public DateTime? PickupDate { get; set; }
        public string PickupTime { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ConfirmationCode { get; set; }
        public string full_address { get; set; }
        public string suburb_locality { get; set; }
        public string town_city { get; set; }
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? ProductSum { get; set; }
        public Guid? OrderID { get; set; }
    }

    public class OrdersHelper
    {
        public static async Task<List<ActiveOrders>> GetActiveOrders(ProductContext context)
        {
            var orders = await context.ProductOrder.AsNoTracking()
                                        .Join(context.CartItem.AsNoTracking(),
                                                po => po.CartID,
                                                ci => ci.CartID, (po, ci) => new
                                                {
                                                    po.Created,
                                                    po.Confirmed,
                                                    po.DeliveryDate,
                                                    po.DeliveryTime,
                                                    po.DeliveryInstructions,
                                                    po.PickupDate,
                                                    po.ContactAddress,
                                                    po.PickupTime,
                                                    po.ContactName,
                                                    po.ContactEmail,
                                                    po.ContactPhone,
                                                    po.ConfirmationCode,
                                                    po.Payment,
                                                    po.OrderID,
                                                    ci.ProductID,
                                                    ci.Quantity,
                                                    ci.Price
                                                })
                                        .Join(context.Product.AsNoTracking(),
                                                ci => ci.ProductID,
                                                p => p.ProductID, (ci, p) => new
                                                {
                                                    ci.Created,
                                                    ci.Confirmed,
                                                    ci.DeliveryDate,
                                                    ci.DeliveryTime,
                                                    ci.DeliveryInstructions,
                                                    ci.PickupDate,
                                                    ci.ContactAddress,
                                                    ci.PickupTime,
                                                    ci.ContactName,
                                                    ci.ContactEmail,
                                                    ci.ContactPhone,
                                                    ci.ConfirmationCode,
                                                    ci.Payment,
                                                    ci.OrderID,
                                                    p.Name,
                                                    ci.Quantity,
                                                    ci.Price
                                                })
                                        .GroupJoin(context.NzAddressDeliverable.AsNoTracking(),
                                                    ci => ci.ContactAddress,
                                                    ad => ad.address_id,
                                                    (ci, ad) => new { ad, ci })
                                        .SelectMany(ad => ad.ad.DefaultIfEmpty(),
                                                    (x, y) => new { y, x.ci })
                                        .Where(a => a.ci.Payment == new DateTime(9999, 12, 31) && a.ci.Confirmed < new DateTime(9999, 12, 31))
                                        .Select(a => new ActiveOrders
                                        {
                                            Created = a.ci.Created,
                                            Confirmed = a.ci.Confirmed,
                                            DeliveryDate = a.ci.DeliveryDate,
                                            DeliveryTime = a.ci.DeliveryTime,
                                            DeliveryInstructions = a.ci.DeliveryInstructions,
                                            PickupDate = a.ci.PickupDate,
                                            PickupTime = a.ci.PickupTime,
                                            ContactName = a.ci.ContactName,
                                            ContactEmail = a.ci.ContactEmail,
                                            ContactPhone = a.ci.ContactPhone,
                                            ConfirmationCode = a.ci.ConfirmationCode,
                                            full_address = a.y.full_address,
                                            suburb_locality = a.y.suburb_locality,
                                            town_city = a.y.town_city,
                                            Name = a.ci.Name,
                                            Quantity = a.ci.Quantity,
                                            Price = a.ci.Price,
                                            ProductSum = a.ci.Quantity * a.ci.Price,
                                            OrderID = a.ci.OrderID
                                        }).ToListAsync();

            return orders;
        }
    }
}
