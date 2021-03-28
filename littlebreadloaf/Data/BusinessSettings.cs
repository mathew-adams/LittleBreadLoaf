using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class BusinessSettings
    {
        [Key]
        public Guid BusinessSettingID { get; set; }
        
        [Display(Name = "Minimum delivery order amount", Prompt = "Enter a minimum delivery order amount", Description = "The minimum amount a delivery order can be")]
        [DataType(DataType.Currency)]
        public decimal MinimumDeliveryOrderAmount { get; set; }

        [Display(Name = "Minimum pickup order amount", Prompt = "Enter a minimum pickup order amount", Description = "The minimum amount a pickup order can be")]
        [DataType(DataType.Currency)]
        public decimal MinimumPickupOrderAmount { get; set; }

        [Display(Name = "Delivery enabled", Prompt = "Select if delivery is enabled", Description = "Delivery is currently enabled")]
        public bool DeliveryEnabled { get; set; }

        [Display(Name = "Pickup enabled", Prompt = "Select if Pickup is enabled", Description = "Pickup is currently enabled")]
        public bool PickupEnabled { get; set; }

        public bool PickupSunday { get; set; }
        public bool PickupMonday { get; set; }
        public bool PickupTuesday { get; set; }
        public bool PickupWednesday { get; set; }
        public bool PickupThursday { get; set; }
        public bool PickupFriday { get; set; }
        public bool PickupSaturday { get; set; }

        public bool DeliverSunday { get; set; }
        public bool DeliverMonday { get; set; }
        public bool DeliverTuesday { get; set; }
        public bool DeliverWednesday { get; set; }
        public bool DeliverThursday { get; set; }
        public bool DeliverFriday { get; set; }
        public bool DeliverSaturday { get; set; }

    }
}
