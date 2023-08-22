using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Models.ForCreationAndUpdate
{
    public class CategoryForUpdateDto
    {
        [Required(ErrorMessage = "Please provide Category Name")]
        [RegularExpression(@"^\w+( \w+)*(-\w+)*$", ErrorMessage = "Category name should contain only alphabets separated by - or space")]
        [MaxLength(50, ErrorMessage = "Category Name too large")]
        public string CategoryName { get; set; }
    }
}
