using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class PreOrderSource
    {
        [Key]
        public Guid PreOrderID { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Source", Prompt = "Pre-order source (e.g. XERO)", Description = "The name of who is pre-ordering")]
        [Required(ErrorMessage = "A pre-order source is required.")]
        public string Source { get; set; }

        public int AddressID { get; set; }
        public DateTime Created { get; set; }
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Active { get; set; }
    }
}
