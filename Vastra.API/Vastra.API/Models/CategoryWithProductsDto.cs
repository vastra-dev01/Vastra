namespace Vastra.API.Models
{
    public class CategoryWithProductsDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

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
