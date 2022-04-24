using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace littlebreadloaf.Data
{
    public class InvoiceModel
    {
        public string Status { get; set; }
        public decimal Balance { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string BankNumber { get; set; }
        public string Phone { get; set; }
        public bool HasAddress { get; set; }
        public ProductOrder ProductOrder { get; set; }
        public Invoice Invoice { get; set; }
        public NzAddressDeliverable NzAddressDeliverable { get; set; }
        public List<InvoiceTransaction> InvoiceTransactions { get; set; }
        public string GetLogoUrl(IHostingEnvironment env)
        {
            return Path.Combine(env.ContentRootPath,
                                "wwwroot",
                                "images",
                                "little-bread-loaf-logo-blue.png");
        }
    }
}
