using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vastra.API.Entities
{
    public class Category
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }
        public Category? ParentCategory { get; set; }
        public int? ParentCategoryId { get; set; }

        public ICollection<Category> ChildCategories { get; set; } = new List<Category>();
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public Category(string categoryName)
        {
            CategoryName = categoryName;
        }

        public ICollection<Product> Products { get; set; } = new List<Product>();


    }
}
