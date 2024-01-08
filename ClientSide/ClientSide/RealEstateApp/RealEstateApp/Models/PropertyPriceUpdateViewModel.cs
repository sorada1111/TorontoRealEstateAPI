using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Models
{
    public class PropertyPriceUpdateViewModel
    {
        public string? PropertyId { get; set; }
        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }
    }
}
