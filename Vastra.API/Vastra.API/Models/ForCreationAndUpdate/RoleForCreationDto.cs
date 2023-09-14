using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class RoleForCreationDto
    {
        [Required(ErrorMessage = "Please provide a Role Name")]
        [MaxLength(50, ErrorMessage = "Role Name too large")]
        [MinLength(3, ErrorMessage = "Role Name too small")]
        [RegularExpression(@"^[a-zA-Z]+( [a-zA-Z]+)*(-[a-zA-Z]+)*$",
            ErrorMessage = "Role Name should only contain alphabets, space or -")]
        public string RoleName { get; set; }
    }
}
