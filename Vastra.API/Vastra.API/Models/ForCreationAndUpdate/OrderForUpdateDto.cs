using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class OrderForUpdateDto
    {
        [Required(ErrorMessage = "Please provide order value")]
        public int Value { get; set; }

        //public String PaymentStatus { get; set; }
    }
}
