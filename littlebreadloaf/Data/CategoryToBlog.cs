using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class CategoryToBlog
    {
        [Key]
        public Guid? CategoryToBlogID { get; set; }

        public Guid? BlogCategoryID { get; set; }
        public Guid? BlogID { get; set; }
    }
}
