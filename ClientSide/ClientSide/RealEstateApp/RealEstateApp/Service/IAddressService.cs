using RealEstateApp.Models;

namespace RealEstateApp.Service
{
    public interface IAddressService
    {
        Task<bool> AddAddressToPropertyAsync(string propertyId, Address address); // Add address
        Task<List<Address>> GetCityAddressAsync(string city); // get address by city 
        Task<HttpResponseMessage> EditAddressAsync(string propertyId, Address address);// Edit Address use Put
        Task<HttpResponseMessage> PatchAddressPostalCodeAsync(AddresspostalCodeUpdateViewModel addresssPostalCode); // Patch Postal Code
        Task<HttpResponseMessage> DeleteAddressAsync(string propertyId); //Delete address
    }
}
