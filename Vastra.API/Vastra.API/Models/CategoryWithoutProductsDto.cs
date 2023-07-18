namespace Vastra.API.Models
{
    public class CategoryWithoutProductsDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

        ICollection<CategoryDto> ChildCategories { get; set; } = new List<CategoryDto>();
    }
}
