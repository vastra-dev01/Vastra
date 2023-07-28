namespace Vastra.API.Models
{
    public class UserWithAddressesDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string? LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string? EmailId { get; set; }

        public ICollection<AddressDto> Addresses { get; set; } = new List<AddressDto>();
        public int NumberOfAddresses
        {
            get
            {
                return Addresses.Count;
            }
        }
        
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
    }
}
