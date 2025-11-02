using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace mvcProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Product name is requiered.")]
        [MaxLength(100, ErrorMessage="Product name can't exceed 100 characters.")]
        [MinLength(3, ErrorMessage="Product name must be at least 3 characters.")]
        public string Name { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage="Price must be greater than zero.")]
        [Required(ErrorMessage="Product price is requiered.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage="Product description is requiered.")]
        [MinLength(20, ErrorMessage="Product description must be at least 20 characters.")]
        public string Description { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; } 
        [Range(0, int.MaxValue, ErrorMessage="Quantity cannot be negative.")]
        [Required(ErrorMessage="Product quantity is requiered.")]
        public int Quantity { get; set; }
        [Range(1, 5, ErrorMessage="Rate must be between 1 and 5.")]
        public int Rate { get; set; }
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
    }
}
