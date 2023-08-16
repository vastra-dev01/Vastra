using System.ComponentModel.DataAnnotations.Schema;
using Vastra.API.Entities;

namespace Vastra.API.Models
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
    }
}
