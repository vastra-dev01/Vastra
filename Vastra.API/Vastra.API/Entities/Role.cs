using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Entities
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}
