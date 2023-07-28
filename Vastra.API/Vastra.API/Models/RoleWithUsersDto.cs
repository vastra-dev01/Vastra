namespace Vastra.API.Models
{
    public class RoleWithUsersDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public ICollection<UserDto> Users { get; set; } = new List<UserDto>();
        public int NumberOfUsers
        {
            get { return Users.Count; }
        }
    }
}
