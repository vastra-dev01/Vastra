using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreation
{
    public class CategoryForCreationDto
    {
        [Required(ErrorMessage = "Please provide a category name")]
        [MaxLength(50)]
        public string CategoryName { get; set; }
    }
}
