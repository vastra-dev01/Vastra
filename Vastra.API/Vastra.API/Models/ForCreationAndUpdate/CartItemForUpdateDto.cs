using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class CartItemForUpdateDto
    {
        [Required(ErrorMessage = "Please provide quantity for cart item")]
        [RegularExpression(@"^[1-9]+[0-9]*$", ErrorMessage = "Quantity Invalid")]
        public int Quantity { get; set; }
    }
}
