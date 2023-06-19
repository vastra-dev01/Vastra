using Vastra.API.Entities;

namespace Vastra.API.Models
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime DateAdded { get; }
        public DateTime DateModified { get; }

        ICollection<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
    }
}
