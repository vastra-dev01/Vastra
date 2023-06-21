using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class CartItemForCreationAndUpdateDto
    {
        [Required(ErrorMessage = "Please provide quantity for cart item")]
        public int Quantity { get; set; }
    }
}
