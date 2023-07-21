namespace Vastra.API.Models
{
    public class CategoryWithProductsDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

        public ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int NumberOfProducts
        {
            get
            {
                return Products.Count;
            }
        }
    }
}
