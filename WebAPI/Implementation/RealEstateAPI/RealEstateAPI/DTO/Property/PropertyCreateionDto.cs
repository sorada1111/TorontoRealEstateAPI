namespace RealEstateAPI.DTO.Property
{
    public class PropertyCreateionDto
    {
        public string? PropertyName { get; set; }
        public string? PropertyDesc { get; set; }
        public decimal PropertyTax { get; set; }
        public decimal Price { get; set; }
        public string? Status { get; set; }
        public String? PropertyType { get; set; }

    }
}
