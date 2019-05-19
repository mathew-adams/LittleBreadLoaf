using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class ProductBundleItem
    {
        [Key]
        public Guid ProductBundleItemID { get; set; }
       
        public Guid ProductBundleID { get; set; }

        public Guid ProductID { get; set; }

    }
}
