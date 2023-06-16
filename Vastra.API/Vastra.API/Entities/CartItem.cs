using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vastra.API.Entities
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        public int Quantity { get; set; }

        
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public DateTime DateAdded { get; private set; }
        public DateTime DateModified { get; private set; }
        public CartItem(int quantity)
        {
            Quantity = quantity;
        }
    }
}
