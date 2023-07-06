using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vastra.API.Entities
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        public string Location { get; set; }

        public string City { get; set; }

        public string State { get; set; }
        public int PinCode { get; set; }

        public string Country { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string? Tag { get; set; }

        public int? Type { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

        public Address(string location, string city, string state, int pinCode, string country)
        {
            Location = location;
            City = city;
            State = state;
            PinCode = pinCode;
            Country = country;
        }
    }
}
