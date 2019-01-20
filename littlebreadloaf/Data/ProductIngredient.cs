using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class ProductIngredient
    {
        [Key]
        public Guid? ProductIngredientID { get; set; }
        public Guid? ProductID { get; set; }

        [Display(Name = "Ingredient", Prompt = "Enter ingredient", Description = "The description of the ingredient")]
        [StringLength(255, MinimumLength = 2)]
        [Required(ErrorMessage = "A product ingredient is required.")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime? Created { get; set; }

    }
}
