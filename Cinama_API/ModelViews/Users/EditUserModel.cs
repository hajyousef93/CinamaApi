using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cinama_API.ModelViews.Users
{
    public class EditUserModel
    {
        [Required]
        public string Id { get; set; }

        [StringLength(256), Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "ConfirmEmail")]
        public bool EmailConfirmed { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string Country { get; set; }

        [StringLength(256), Required, DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
