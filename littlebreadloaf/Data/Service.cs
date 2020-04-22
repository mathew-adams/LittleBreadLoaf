using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class Service
    {
        public Guid ServiceID { get; set; }

        [Display(Name = "Type", Prompt = "Enter type", Description = "The type of service")]
        [StringLength(12, MinimumLength = 2)]
        [Required(ErrorMessage = "A service type is required.")]
        public string Type { get; set; }

        [Display(Name = "Service address", Prompt = "Enter your address", Description = "Your address")]
        public int? ServiceAddress { get; set; }

        [Display(Name = "Parking available", Prompt = "Is parking available?", Description = "Service address has parking available")]
        public bool ParkingAvailable { get; set; }

        [Display(Name = "Employees", Prompt = "Estimated number of employees", Description = "An estimated number of employees")]
        public int Employees { get; set; }
        
        [Display(Name = "Description", Prompt = "Enter description", Description = "The description of the product")]
        [StringLength(255, MinimumLength = 2)]
        [Required(ErrorMessage = "A product description is required.")]
        public string Description { get; set; }

        [StringLength(255, MinimumLength = 2)]
        [Display(Name = "Name", Prompt = "Name", Description = "Your name")]
        [Required(ErrorMessage = "Your name is required.")]
        public string ContactName { get; set; }

        [EmailAddress]
        [Display(Name = "Email address", Prompt = "Email", Description = "Your email address")]
        [StringLength(255, MinimumLength = 2)]
        [Required(ErrorMessage = "Your email address is required.")]
        public string ContactEmail { get; set; }

        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "Phone number", Prompt = "Phone", Description = "Your phone number")]
        [Required(ErrorMessage = "Your phone number is required.")]
        public string ContactPhone { get; set; }

        [Display(Name = "Vegan options", Prompt = "Vegan options", Description = "Vegan options")]
        public bool VeganOptions { get; set; }

        [Display(Name = "Vegitarian options", Prompt = "Vegitarian options", Description = "Vegitarian options")]
        public bool VegitarianOptions { get; set; }

        [StringLength(45, MinimumLength = 2)]
        [Display(Name = "Service time of day", Prompt = "Time of day", Description = "Time of day")]
        [Required(ErrorMessage = "Time of day is required.")]
        public string TimeOfDay { get; set; }

        [StringLength(10, MinimumLength = 2)]
        [Display(Name = "Service day of week", Prompt = "Day of week", Description = "What day of week?")]
        [Required(ErrorMessage = "Your phone number is required.")]
        public string DayOfWeek { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime Created { get; set; }

    }
}
