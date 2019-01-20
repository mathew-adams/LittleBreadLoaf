using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class MovieUser
    {
        
        [Key]
        public Guid? UserID { get; set; }

        [Display(Name = "Username", Prompt = "(e.g. user123)", Description = "User name you will log in with")]
        [Required]
        public string UserName { get; set; }

        [Display(Name = "Email address", Prompt = "(e.g. mail@domain.com)", Description = "Your email address")]
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Full name", Prompt = "(e.g. John Doe)", Description = "Your full name, first name and last name")]
        public string FullName { get; set; }
        
        public bool Active { get; set; }
    }
}
