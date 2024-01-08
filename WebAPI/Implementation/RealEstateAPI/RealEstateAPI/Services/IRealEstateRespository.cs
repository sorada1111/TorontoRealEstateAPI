using RealEstateLibrary.Models;
using System.Runtime;

namespace RealEstateAPI.Services
{
    public interface IRealEstateRespository
    {
        Task<IEnumerable<Property>> GetPropertiesAsync();
        Task<Property> GetPropertyByIdAsync(string propertyId);
        Task<IEnumerable<Property>> GetPropertyByTypeAsync(string propertyType);
        Task<IEnumerable<Property>> GetPropertyByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Property>> GetPropertyByStatusAsync(string status);
        Task<Property> AddProperty(Property property);
        Task<bool> UpdateProperty(Property property);
        Task<bool> DeletePropertyAsync(string propertyId);
    }
}
