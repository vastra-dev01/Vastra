using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreation
{
    public class ProductForCreationDto
    {
        [Required(ErrorMessage = "Please provide a product name")]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
        [MaxLength(10)]
        public string Size { get; set; }
        [MaxLength(50)]
        public string Colour { get; set; }
        [MaxLength(50)]
        public string SKU { get; set; }

        public float Price { get; set; }
        [MaxLength(100)]
        public string Image { get; set; }

        public int Quantity { get; set; }
    }
}
