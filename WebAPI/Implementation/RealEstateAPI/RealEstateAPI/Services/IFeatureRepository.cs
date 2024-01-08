using RealEstateAPI.DTO.Addresses;
using RealEstateAPI.DTO.Features;
using RealEstateLibrary.Models;

namespace RealEstateAPI.Services
{
    public interface IFeatureRepository
    {
        Task<IEnumerable<Feature>> GetFeaturesAsync();
        Task<IEnumerable<Feature>> GetFeaturesBySizeAsync(string size);
        Task<IEnumerable<Property>> GetPropertiesBySizeAsync(string size);
        Task<bool> AddFeatureToPropertyAsync(string propertyId, FeatureDto featureDto);
        Task<bool> UpdateFeature(string propertyId, Feature updatedFeature);
        Task<bool> DeleteFeatureFromPropertyAsync(string propertyId);
    }
}
