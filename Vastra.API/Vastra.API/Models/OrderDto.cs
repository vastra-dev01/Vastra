using Vastra.API.Entities;

namespace Vastra.API.Models
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int Value { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

        public ICollection<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
        public int NumberOfCartItems { 
            get { 
                return CartItems.Count;
            } 
        }
    }
}
