namespace RealEstateAPI.DTO.Property
{
    public class PropertyWithoutOthersAttributeDto
    {
        public string? PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyDesc { get; set; }
        public decimal PropertyTax { get; set; }
        public string? LastUpdate { get; set; }
        public string? DateListed { get; set; }
        public decimal Price { get; set; }
        public string? Status { get; set; }
        public String? PropertyType { get; set; }
        public List<string>? PropertyImageUrls { get; set; }
    }
}
