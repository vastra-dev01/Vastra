namespace Vastra.API.Models
{
    public class RoleDto
    {
        public int RoleId { get; set; }

        public string Name { get; set; }
        public DateTime DateAdded { get; }
        public DateTime DateModified { get; }
    }
}
