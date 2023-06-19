using System.ComponentModel.DataAnnotations.Schema;
using Vastra.API.Entities;

namespace Vastra.API.Models
{
    public class AddressDto
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public string City { get; set; }

        public string State { get; set; }
        public int PinCode { get; set; }

        public string Country { get; set; }

        public string? Tag { get; set; }

        public int? Type { get; set; }

        public DateTime DateAdded { get; }
        public DateTime DateModified { get; }
    }
}
