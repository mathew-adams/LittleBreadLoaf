using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class BlogImage
    {
        [Key]
        public Guid BlogImageID { get; set; }

        public Guid BlogID { get; set; }

        [Display(Name = "Title", Prompt = "Enter image title", Description = "The title of the image.")]
        [StringLength(255, MinimumLength = 2)]
        [Required(ErrorMessage = "An image title is required.")]
        public string Title { get; set; }

        [Display(Name = "File Location", Description = "The file location of the image.")]
        [StringLength(1000, MinimumLength = 2)]
        public string FileLocation { get; set; }

        [Display(Name = "File Save Mode", Description = "Square, aspect, or banner")]
        public string Mode { get; set; }
    }
}
