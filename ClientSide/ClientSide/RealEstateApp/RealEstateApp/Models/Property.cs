using System.ComponentModel.DataAnnotations;
using System.Net;

namespace RealEstateApp.Models
{
    public class Property
    {
        public string? PropertyId { get; set; }

        [Required(ErrorMessage = "Property Name is required.")]
        public string? PropertyName { get; set; }
        [Required(ErrorMessage = "Property Name is required.")]
        public string? PropertyDesc { get; set; }

        [Required(ErrorMessage = "Property Tax is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Property Tax must be a positive number.")]
        public decimal PropertyTax { get; set; }
        public string? LastUpdate { get; set; }
        public string? DateListed { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Property Status is required.")]
        public string? Status { get; set; }
        [Required(ErrorMessage = "Property Type is required.")]
        public String? PropertyType { get; set; }
        public List<string>? PropertyImageUrls { get; set; }
        public Address? PropertyAddresses { get; set; }
        public Agent? Agents { get; set; }
        public Feature? Features { get; set; }
    }
}
