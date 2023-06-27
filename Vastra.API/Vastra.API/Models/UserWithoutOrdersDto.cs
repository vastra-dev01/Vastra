namespace Vastra.API.Models
{
    public class UserWithoutOrdersDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string? LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string? EmailId { get; set; }

        public RoleDto Role { get; set; }

        public ICollection<AddressDto> Addresses { get; set; } = new List<AddressDto>();
        public int NumberOfAddresses
        {
            get
            {
                return Addresses.Count;
            }
        }
        
        public DateTime DateAdded { get; }
        public DateTime DateModified { get; }
    }
}
