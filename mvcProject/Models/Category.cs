using System.ComponentModel.DataAnnotations;

namespace mvcProject.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MinLength(3, ErrorMessage = "Product name must be at least 3 characters.")]
        [Required(ErrorMessage = "Product name is requiered.")]
        [MaxLength(100, ErrorMessage = "Product name can't exceed 100 characters.")]
        public string Name { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
