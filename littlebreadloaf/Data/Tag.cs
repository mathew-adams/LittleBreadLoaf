using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class Tag
    {
        [Key]
        public Guid? TagID { get; set; }

        [Display(Name = "Tag", Prompt = "Enter tag", Description = "A tag to display")]
        [StringLength(255, MinimumLength = 2)]
        [Required(ErrorMessage = "A tag is required.")]
        public string Name { get; set; }
    }
}
