using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class ProductOrder
    {
        [Key]
        public Guid? OrderID { get; set; }
        public Guid? CartID { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime? Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date", Prompt = "Date", Description = "The time you would like your order delivered")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DeliveryDate { get; set; }
        
        [StringLength(45)]
        [Display(Name = "Time", Prompt = "Time", Description = "The time you would like your order delivered")]
        [Required(ErrorMessage = "A delivery time is required.")]
        public string DeliveryTime { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime? Confirmed { get; set; }

        [Display(Name = "Would you like to schedule a pickup?", Prompt = "Schedule for pickup?", Description = "Schedule for pickup?")]
        public Boolean Pickup { get; set; }

        [Display(Name = "Do you need an invoice?", Prompt = "Do you need an invoice?", Description = "Do you need an invoice?")]
        public Boolean Invoice { get; set; }

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

        [Display(Name = "Delivery address", Prompt = "Enter your delivery address", Description = "Your delivery address")]
        public int? ContactAddress { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime? Payment { get; set; }

        [Display(Name = "Payment method", Prompt = "Select your payment method", Description = "Your choice of payment")]
        [StringLength(12, MinimumLength = 2)]
        [Required(ErrorMessage = "A payment method is required.")]
        public string PaymentMethod { get; set; }

        [StringLength(6)]
        [Display(Name = "Confirmation code", Description = "Confirmation code")]
        public string ConfirmationCode { get; set; }
    }
}
