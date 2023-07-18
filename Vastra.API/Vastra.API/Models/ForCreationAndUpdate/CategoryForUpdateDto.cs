using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class CategoryForUpdateDto
    {
        [Required(ErrorMessage = "Please provide a category name")]
        [MaxLength(50)]
        public string CategoryName { get; set; }
    }
}
