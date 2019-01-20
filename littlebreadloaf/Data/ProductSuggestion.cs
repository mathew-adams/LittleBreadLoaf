using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class ProductSuggestion
    {
        [Key]
        public Guid? ProductSuggestionID { get; set; }
        public Guid? ProductID { get; set; }

        [Display(Name = "Suggestion", Prompt = "Enter suggestion", Description = "A product suggestion")]
        [StringLength(255, MinimumLength = 2)]
        [Required(ErrorMessage = "A product suggestion is required.")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime? Created { get; set; }

    }
}
