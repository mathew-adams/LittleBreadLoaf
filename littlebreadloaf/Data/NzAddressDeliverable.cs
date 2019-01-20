using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class NzAddressDeliverable
    {
        [Key]
        public int address_id { get; set; }

        public int address_number { get; set; }

        public string address_number_high { get; set; }

        public string address_number_suffix { get; set; }

        public string address_type { get; set; }

        public string full_address { get; set; }

        public string full_address_number { get; set; }

        public string full_road_name { get; set; }

        public string suburb_locality { get; set; }

        public string town_city { get; set; }

        public string unit_value { get; set; }

    }
}
