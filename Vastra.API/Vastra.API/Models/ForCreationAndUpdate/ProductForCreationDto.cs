using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class ProductForCreationDto
    {
        [Required(ErrorMessage = "Please provide Product Name")]
        [RegularExpression(@"^\w+( \w+)*(-\w+)*$",
            ErrorMessage = "Product Name should contain only alphanumeric values separated by " +
            "- or space")]
        [MaxLength(50, ErrorMessage = "Product Name too large")]
        [MinLength(8, ErrorMessage = "Product Name too small")]
        public string Name { get; set; }


        [RegularExpression(@"^(.|\s)*[a-zA-Z]+(.|\s)*$",
            ErrorMessage = "Description can contain alphabets, numbers, special characters " +
            "but can't contain only special characters, space or numbers.")]
        [MaxLength(200, ErrorMessage = "Description too large")]
        [MinLength(20, ErrorMessage = "Description too small")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please provide Size")]
        [RegularExpression(@"^(M|S|L|XS|XL|XXL|XXXL)$", 
            ErrorMessage = "Size invalid(Only M|S|L|XS|XL|XXL|XXXL allowed)")]
        public string Size { get; set; }


        [RegularExpression(@"^[a-zA-Z]+( [a-zA-Z]+)*(-[a-zA-Z]+)*$", 
            ErrorMessage = "Colour should contain only alphabets separated by - or space")]
        [MaxLength(50, ErrorMessage = "Colour too large")]
        [MinLength(2, ErrorMessage = "Colour too small")]
        public string Colour { get; set; }


        [Required(ErrorMessage = "Pleae provide SKU")]
        [RegularExpression(@"^[A-Z]{4}[0-9]{4}$", 
            ErrorMessage = "SKU invalid. 8 characters long " +
            " value with 4 capital alphabets and 4 numbers is allowed ex: ABCD1234.")]
        public string SKU { get; set; }


        [Required(ErrorMessage = "Please provide Price")]
        [RegularExpression(@"^[1-9]+\d*[\.]{0,1}\d+$", ErrorMessage = "Price Invalid")]
        public float Price { get; set; }

        //[RegularExpression(@"^/?[\w-]+(?:/[\w-]+)*\.(png|jpg|jpeg)$", 
        //    ErrorMessage = "Image path Invalid. Valid image name/path required ex: " +
        //    "'image.png|jpg|jpeg'")]
        //[MaxLength(100, ErrorMessage = "Image path too large")]
        //public string Image { get; set; }


        [Required(ErrorMessage = "Please provide quantity for product")]
        [RegularExpression(@"^[1-9]+[0-9]*$", ErrorMessage = "Quantity Invalid")]
        public int Quantity { get; set; }



        //public DateTime Created { get; set; } = DateTime.Now;
        //public DateTime Modified { get; set; } = DateTime.Now;
    }
}
