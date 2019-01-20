using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class Cart
    {
        [Key]
        public Guid? CartID { get; set; }

        public Guid? UserID { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? CheckedOut { get; set; }
    }
}
