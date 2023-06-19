namespace Vastra.API.Models
{
    public class RoleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime DateAdded { get; }
        public DateTime DateModified { get; }
    }
}
