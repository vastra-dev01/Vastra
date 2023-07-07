using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class UserForCreationDto
    {
        [Required(ErrorMessage = "Please provide a first name")]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        [Phone(ErrorMessage = "This does not appear to be a valid phone number")]
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        [EmailAddress(ErrorMessage = "This does not appear to be a valid email address")]
        public string? EmailId { get; set; }
    }
}
