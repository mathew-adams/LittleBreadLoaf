using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class Blog
    {
        [Key]
        public Guid? BlogID { get; set; }

        public Guid? UserID { get; set; }

        [Display(Name = "Title", Prompt = "Enter title", Description = "The title of the blog post")]
        [StringLength(255, MinimumLength = 2)]
        [Required(ErrorMessage = "A blog title is required.")]
        public string Title { get; set; }

        [Display(Name = "Description", Prompt = "Enter decription", Description = "The description of the blog post. This will display on the blog list.")]
        [StringLength(2000, MinimumLength = 2)]
        [Required(ErrorMessage = "A blog decription is required.")]
        public string Description { get; set; }

        [Display(Name = "Content", Prompt = "Enter content", Description = "The content of the blog post")]
        [Required(ErrorMessage = "Blog content is required.")]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime? Created { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime? Published { get; set; } 
    }
}
