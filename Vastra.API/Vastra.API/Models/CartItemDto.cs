using System.ComponentModel.DataAnnotations.Schema;
using Vastra.API.Entities;

namespace Vastra.API.Models
{
    public class CartItemDto
    {
        public int Id { get; set; }

        public int Quantity { get; set; }
        public DateTime DateAdded { get; }
        public DateTime DateModified { get; }
    }
}
