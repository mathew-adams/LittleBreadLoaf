using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class InvoiceTransaction
    {

        [Key]
        public Guid InvoiceTransactionID { get; set; }

        public Guid InvoiceID { get; set; }
        
        [StringLength(12)]
        public string Type { get; set; }

        [StringLength(12)]
        public string Category { get; set; }

        [StringLength(45)]
        public string Name { get; set; }
        
        [StringLength(45)]
        public string Description { get; set; }
        
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Posted { get; set; }
        
    }
}
