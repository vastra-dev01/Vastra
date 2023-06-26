using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class OrderForCreationAndUpdateDto
    {
        [Required(ErrorMessage = "Please provide order value")]
        public int Value { get; set; }

        public String PaymentStatus { get; set; }
    }
}
