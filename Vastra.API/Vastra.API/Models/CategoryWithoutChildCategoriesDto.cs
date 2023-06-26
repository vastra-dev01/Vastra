namespace Vastra.API.Models
{
    public class CategoryWithoutChildCategoriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
