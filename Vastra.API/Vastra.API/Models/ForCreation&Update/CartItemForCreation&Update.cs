using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreation
{
    public class CartItemForCreationDto
    {
        [Required(ErrorMessage = "Please provide quantity for cart item")]
        public int Quantity { get; set; }
    }
}
