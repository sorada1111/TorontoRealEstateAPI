using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Models
{
    public class Agent
    {
        public string? AgentId { get; set; }
        [Required(ErrorMessage = "Agent Name is required.")]
        public string? AgentName { get; set; }
        [Required(ErrorMessage = "Agent Company Name is required.")]
        public string? AgentCompanyName { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [StringLength(12, ErrorMessage = "Phone Number cannot be longer than 12 characters.")]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Phone Number must be in the format xxx-xxx-xxxx.")]
        public string? AgentPhone { get; set; }
    }
}
