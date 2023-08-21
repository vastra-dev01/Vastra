using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class AddressForUpdateDto
    {
        [Required(ErrorMessage = "Please provide location details")]
        [MaxLength(100)]
        public string Location { get; set; }
        [Required(ErrorMessage = "Please provide city name")]
        [MaxLength(50)]
        public string City { get; set; }
        [Required(ErrorMessage = "Please provide state name")]
        [MaxLength(50)]
        public string State { get; set; }
        [Required(ErrorMessage = "Please provide pincode")]
        public int PinCode { get; set; }
        [Required(ErrorMessage = "Please provide country name")]
        [MaxLength(50)]
        public string Country { get; set; }
        [MaxLength(50)]
        public string? Tag { get; set; }
        public int? Type { get; set; }
    }
}
