using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vastra.API.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        [MaxLength(500)]
        public string Password { get; set; }
        [MaxLength(100)]
        public string? EmailId { get; set; }

        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public User(string firstName, string? lastName, string phoneNumber, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Password = password;
        }
    }
}
