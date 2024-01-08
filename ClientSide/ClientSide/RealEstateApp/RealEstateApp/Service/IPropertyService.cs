using Microsoft.AspNetCore.Mvc.ApplicationModels;
using RealEstateApp.Models;

namespace RealEstateApp.Service
{
    public interface IPropertyService
    {
        Task<IEnumerable<Property>> GetAllPropertiesAsync();  //get all properties
        Task<Property> GetPropertyByIdAsync(string propertyId); //get properties by Id
        Task<IEnumerable<Property>> GetPropertyByCityAsync(string city); // get property by city
        Task<IEnumerable<Property>> GetPropertyBySearchAsync(string? city, string? areaCode, string? propertyType, decimal? minPrice, decimal? maxPrice); // get property by search
        Task<Property> CreatePropertyAsync(Property property); //create new property 

        Task<bool> UploadPropertyImagesAsync(string propertyId, List<IFormFile> images); // upload new property image

        Task<HttpResponseMessage> UpdatePropertyAsync(Property property); // update property 

        Task<HttpResponseMessage> PatchPropertyPriceAsync(PropertyPriceUpdateViewModel propertyPrice); // patch method for property price
        Task<HttpResponseMessage> DeletePropertyAsync(string propertyId); //Delete Property 
    }
}
