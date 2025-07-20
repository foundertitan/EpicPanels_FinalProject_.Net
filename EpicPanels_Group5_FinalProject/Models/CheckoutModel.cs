using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EpicPanels_Group5_FinalProject.Models
{
    public class CheckoutModel
    {
        [Required, MaxLength(50)]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required, MaxLength(6), MinLength(6)]
        public string PostalCode { get; set; }

        [Required, MaxLength(16), MinLength(16)]
        public string CardNumber { get; set; }

        [Required]
        public string CardHolderName { get; set; }

        [Required, RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Invalid expiry date format.")]
        public string ExpiryDate { get; set; }

        [Required, MaxLength(3), MinLength(3)]
        public string CVV { get; set; }

        public List<CartItem> OrderItems { get; set; } = new List<CartItem>();
    }

}
