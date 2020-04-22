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
        [Display(Name = "Deliver date", Prompt = "Deliver date", Description = "The day you would like your order delivered")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DeliveryDate { get; set; }
        
        [StringLength(45)]
        [DisplayFormat(NullDisplayText = "", ConvertEmptyStringToNull = false)]
        [Display(Name = "Deliver time", Prompt = "Deliver time", Description = "The time you would like your order delivered")]
        public string DeliveryTime { get; set; }

        [StringLength(2000)]
        [DisplayFormat(NullDisplayText = "", ConvertEmptyStringToNull = false)]
        [Display(Name = "Delivery instructions", Prompt = "Delivery instructions, voucher number, or any comments", Description = "Any additional delivery instructions you have")]
        public string DeliveryInstructions { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Pickup date", Prompt = "Pickup date", Description = "The day you would like to pick up your order")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? PickupDate { get; set; }

        [StringLength(45)]
        [DisplayFormat(NullDisplayText = "", ConvertEmptyStringToNull = false)]
        [Display(Name = "Pickup time", Prompt = "Pickup time", Description = "The time you would like to pick up your order")]
        public string PickupTime { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime? Confirmed { get; set; }

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
