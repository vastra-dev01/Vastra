using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreation
{
    public class RoleForCreationDto
    {
        [Required(ErrorMessage = "Please provide a role name")]
        [MaxLength(50)]
        public string RoleName { get; set; }
    }
}
