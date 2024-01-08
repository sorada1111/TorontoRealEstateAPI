using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Models
{
    public class AgentPhoneNumberUpdateViewModel
    {
        public string? PropertyId { get; set; }
        public string? AgentId { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [StringLength(12, ErrorMessage = "Phone Number cannot be longer than 12 characters.")]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Phone Number must be in the format xxx-xxx-xxxx.")]
        public string? AgentPhone { get; set; }
    }
}
