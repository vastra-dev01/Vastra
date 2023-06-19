namespace Vastra.API.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Size { get; set; }
        public float Price { get; set;}
        public string Colour { get; set; }
        public string Image { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public DateTime DateAdded { get;}
        public DateTime DateModified { get;}
    }
}
