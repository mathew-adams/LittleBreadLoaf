using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class BlogCategory
    {
        [Key]
        public Guid? BlogCategoryID { get; set; }

        [Display(Name = "Category", Prompt = "Enter blog category", Description = "A category to organize blog posts")]
        [StringLength(50, MinimumLength = 2)]
        [Required(ErrorMessage = "A category is required.")]
        public string Name { get; set; }
    }
}
