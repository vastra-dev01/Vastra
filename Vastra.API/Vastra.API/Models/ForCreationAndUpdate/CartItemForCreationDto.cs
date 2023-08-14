using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class CartItemForCreationDto
    {
        [Required(ErrorMessage = "Please provide product id of product to be added as cart item")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Please provide quantity for cart item")]
        public int Quantity { get; set; }
    }
}
