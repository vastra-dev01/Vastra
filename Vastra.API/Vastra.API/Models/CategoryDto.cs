using Vastra.API.Entities;

namespace Vastra.API.Models
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

        ICollection<CategoryDto> ChildCategories { get; set; } = new List<CategoryDto>();
        public int NumberOfChildCategories {
            get
            {
                return ChildCategories.Count;
            }
        }
        ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int NumberOfProducts
        {
            get
            {
                return Products.Count;
            }
        }
    }
}
