using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Vastra.API.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public string Size { get; set; }

        public string Colour { get; set; }

        public string SKU { get; set; }

        public float Price { get; set; }

        public string Image { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public int CategoryId
        {
            get; set;
        }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public Product(string name, string size, string colour, string sKU, float price, string image, int quantity)
        {
            Name = name;
            Size = size;
            Colour = colour;
            SKU = sKU;
            Price = price;
            Image = image;
            Quantity = quantity;
        }
    }
}
