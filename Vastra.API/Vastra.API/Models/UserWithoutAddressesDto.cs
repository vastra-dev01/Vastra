namespace Vastra.API.Models
{
    public class UserWithoutAddressesDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string? LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string? EmailId { get; set; }

        public RoleDto Role { get; set; }

        public ICollection<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public int NumberOfOrders
        {
            get
            {
                return Orders.Count;
            }
        }
        public DateTime DateAdded { get; }
        public DateTime DateModified { get; }
    }
}
