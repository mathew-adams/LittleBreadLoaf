using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.ConfigurationProvider
{
    public class LittleBreadLoafSystem
    {
        [Key]
        [StringLength(255)]
        public string Key { get; set; }

        [StringLength(1000)]
        public string Value { get; set; }
    }
}
