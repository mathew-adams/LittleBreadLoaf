using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class UserProfile
    {
        [Key]
        public Guid? UserID { get; set; }

        [Display(Name = "First name", Prompt = "Enter first name", Description = "User first name")]
        [StringLength(50, MinimumLength = 2)]
        [Required(ErrorMessage = "Your first name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last name", Prompt = "Enter last name", Description = "User last name")]
        [StringLength(50, MinimumLength = 2)]
        [Required(ErrorMessage = "Your last name is required.")]
        public string LastName { get; set; }

        [Display(Name = "Instagram Display", Prompt = "Enter your Instrgram display text", Description = "Your Instagram display text")]
        [StringLength(50, MinimumLength = 2)]
        public string InstagramDisplay { get; set; }

        [Display(Name = "Instagram URL", Prompt = "Enter your Instrgram URL", Description = "Your Instagram URL")]
        [StringLength(150, MinimumLength = 2)]
        [DataType(DataType.Url)]
        public string InstagramURL { get; set; }

        [Display(Name = "Profile Image", Prompt = "Enter your profile image", Description = "Your profile image")]
        public string ProfileImageLocation { get; set; }
        
    }
}
