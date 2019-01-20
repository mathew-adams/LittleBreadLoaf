using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class ProductBadge
    {
        [Key]
        public Guid? ProductBadgeID { get; set; }
        public Guid? ProductID { get; set; }

        [Display(Name = "Badge", Prompt = "Example: V", Description = "A short title to display on below the title")]
        [Required(ErrorMessage = "A badge title is required.")]
        public string Title { get; set; }

        [Display(Name = "Popup Display", Prompt = "Example: Vegan", Description = "The text that will display when you hover over the badge")]
        [Required(ErrorMessage = "A badge description is required.")]
        public string Description { get; set; }

    }
}
