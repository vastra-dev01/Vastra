namespace Vastra.API.Models
{
    public class CartItemWithProductDto
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public float Value { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public ProductDto Product { get; set; } = new ProductDto();
    }
}
