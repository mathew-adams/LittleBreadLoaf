using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class ProductImage
    {
        [Key]
        public Guid? ProductImageID { get; set; }

        public Guid? ProductID { get; set; }

        [Display(Name = "Title", Prompt = "Enter image title", Description = "The title of the image.")]
        [StringLength(255, MinimumLength = 2)]
        [Required(ErrorMessage = "An image title is required.")]
        public string Title { get; set; }

        [Display(Name = "File Location", Description = "The file location of the product image.")]
        [StringLength(1000, MinimumLength = 2)]
        public string FileLocation { get; set; }

        [Display(Name = "Primary Image", Description = "The primary image shown on the list of products.")]
        public bool? PrimaryImage { get; set; }
        
    }
}
