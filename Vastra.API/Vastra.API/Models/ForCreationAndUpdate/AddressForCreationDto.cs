using System.ComponentModel.DataAnnotations;
using System.Resources;
using Vastra.API.Services;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class AddressForCreationDto
    {
        [Required(ErrorMessage = "Please provide Location")]
        [MinLength(10, ErrorMessage = "Provided Location details not enough")]
        [RegularExpression(@"^\w+( \w+)*(-\w+)*$", ErrorMessage = "Location should contain only alphabets, numbers, -, and space")]
        [MaxLength(100, ErrorMessage = "Location details provided too large")]
        public string Location { get; set; }


        [Required(ErrorMessage = "Please provide City name")]
        [MinLength(3, ErrorMessage ="City name too small")]
        [MaxLength(50, ErrorMessage = "City name too large")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "City name invalid")]
        public string City { get; set; }


        [Required(ErrorMessage = "Please provide State name")]
        [MinLength(3, ErrorMessage = "State name too small")]
        [MaxLength(50, ErrorMessage = "State name too large")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "State name invalid")]
        public string State { get; set; }


        [Required(ErrorMessage = "Please provide Pincode ")]
        [RegularExpression(@"^[1-9]{1}[0-9]{2}[0-9]{3}$", ErrorMessage = "Pincode invalid")]
        public int PinCode { get; set; }


        [Required(ErrorMessage = "Please provide Country name")]
        [MinLength(3, ErrorMessage = "Country name too small")]
        [MaxLength(50, ErrorMessage = "Country name too large")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Country name invalid")]
        public string Country { get; set; }


        [MinLength(3, ErrorMessage = "Tag too small")]
        [MaxLength(50, ErrorMessage = "Tag too large")]
        [RegularExpression(@"^[-\w]+$", ErrorMessage = "Tag name should contain only alphabets, numbers, -, and _")]
        public string? Tag { get; set; }

        [RegularExpression(@"^(0|1)$", ErrorMessage = "Type can only be 0(Primary) or 1(Secondary)")]
        public int? Type { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    //Type can only be 0 or 1
        //    if(Type != 0 && Type != 1)
        //    {
        //        yield return new ValidationResult(
        //            "Type can only be 0(Primary) or 1(Secondary).",
        //            new[] { "Type" });
        //    }
            
        //}
    }
}
