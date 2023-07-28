namespace Vastra.API.Models
{
    public class UserWithOrdersDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string? LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string? EmailId { get; set; }

        public ICollection<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public int NumberOfOrders
        {
            get
            {
                return Orders.Count;
            }
        }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
    }
}
