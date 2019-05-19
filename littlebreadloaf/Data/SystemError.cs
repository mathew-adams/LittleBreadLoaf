using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class SystemError
    {
        [Key]
        public Guid ErrorID { get; set; }

        [StringLength(255)]
        public string RequestID { get; set; }

        [StringLength(255)]
        public string Path { get; set; }

        public string Error { get; set; }

        public DateTime Occurred { get; set; }
    }
}
