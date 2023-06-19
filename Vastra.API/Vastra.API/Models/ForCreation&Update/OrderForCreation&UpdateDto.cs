using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreation
{
    public class OrderForCreationDto
    {
        [Required(ErrorMessage = "Please provide order value")]
        public int Value { get; set; }

        public String PaymentStatus { get; set; }
    }
}
