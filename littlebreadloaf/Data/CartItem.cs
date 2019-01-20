using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class CartItem
    {
        [Key]
        public Guid? CartItemID { get; set; }

        public Guid? CartID { get; set; }

        public Guid? ProductID { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }

        public DateTime? Created { get; set; }
    }
}
