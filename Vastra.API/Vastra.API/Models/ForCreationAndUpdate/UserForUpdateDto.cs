using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class UserForUpdateDto
    {
        [Required(ErrorMessage = "Please provide a First Name")]
        [MaxLength(50, ErrorMessage = "First name too large")]
        [MinLength(3, ErrorMessage = "First Name too small")]
        [RegularExpression(@"^[a-zA-z]+$", ErrorMessage = "First Name should be a single word")]
        public string FirstName { get; set; }


        [MaxLength(50, ErrorMessage = "Last name too large")]
        [MinLength(3, ErrorMessage = "Last Name too small")]
        [RegularExpression(@"^[a-zA-z]+$", ErrorMessage = "Last Name should be a single word")]
        public string? LastName { get; set; }


        [Phone(ErrorMessage = "This does not appear to be a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please provide password")]
        [RegularExpression(@"^(?=[^a-z]*[a-z])(?=[^A-Z]*[A-Z])(?=\D*\d)(?=[^!@#%]*[!@#%])[A-Za-z0-9!#@%]{5,15}$",
         ErrorMessage = "Password should be between 5-15 characters containing at least one special character(!,# or %)")]
        public string Password { get; set; }


        [EmailAddress(ErrorMessage = "This does not appear to be a valid email address")]
        public string? EmailId { get; set; }
    }
}
