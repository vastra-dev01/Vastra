using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class RoleForUpdateDto
    {
        [Required(ErrorMessage = "Please provide a role name")]
        [MaxLength(50)]
        public string RoleName { get; set; }
    }
}
