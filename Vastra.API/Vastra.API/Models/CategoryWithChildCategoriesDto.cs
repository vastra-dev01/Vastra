namespace Vastra.API.Models
{
    public class CategoryWithChildCategoriesDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public ICollection<CategoryDto> ChildCategories { get; set; } = new List<CategoryDto>();
        public int NumberOfChildCategories
        {
            get
            {
                return ChildCategories.Count;
            }
        }

    }
}
