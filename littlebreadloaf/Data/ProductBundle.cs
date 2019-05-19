using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace littlebreadloaf.Data
{
    public class ProductBundle
    {
        [Key]
        public Guid ProductBundleID { get; set; }

        [Display(Name = "Title", Prompt = "Enter title", Description = "The title of the product bundle")]
        [StringLength(50, MinimumLength = 2)]
        [Required(ErrorMessage = "A product bundle title is required.")]
        public string Title { get; set; }

        [Display(Name = "Description", Prompt = "Enter Description", Description = "The description of the product bundle")]
        [StringLength(50, MinimumLength = 2)]
        [Required(ErrorMessage = "A product bundle description is required.")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime Created { get; set; }
        
    }
}
