using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class CartItemForCreationDto
    {
        [Required(ErrorMessage = "Please provide product id of product to be added as cart item")]
        [RegularExpression(@"^[0-9]{1, 10}$", ErrorMessage = "ProductId Invalid")]
        public int ProductId { get; set; }


        [Required(ErrorMessage = "Please provide quantity for cart item")]
        [RegularExpression(@"^[1-9]{1, 10}$", ErrorMessage = "Quantity Invalid")]
        public int Quantity { get; set; }
    }
}
