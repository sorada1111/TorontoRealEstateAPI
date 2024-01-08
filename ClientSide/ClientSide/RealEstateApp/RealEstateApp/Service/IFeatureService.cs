using RealEstateApp.Models;

namespace RealEstateApp.Service
{
    public interface IFeatureService
    {
        Task<bool> AddFeaturesToPropertyAsync(string propertyId, Feature features); // add features to the property
        Task<HttpResponseMessage> EditFeaturesAsync(string propertyId, Feature features); //edit features
        Task<HttpResponseMessage> DeleteFeaturesAsync(string propertyId); //delete features
    }
}
