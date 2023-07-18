using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vastra.API.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int Value { get; set; }

        public String PaymentStatus { get; set; } 

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public Order(int value, string paymentStatus)
        {
            Value = value;
            PaymentStatus = paymentStatus;
        }
    }
}
