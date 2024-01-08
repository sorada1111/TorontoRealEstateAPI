using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using RealEstateApp.Models;
using System.Net.Http.Json;
using System.Text;

namespace RealEstateApp.Service
{
    public class AddressService: IAddressService
    {
        private readonly HttpClient _httpClient;

        public AddressService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> AddAddressToPropertyAsync(string propertyId, Address address)
        {
            var jsonContent = new StringContent(
            JsonConvert.SerializeObject(address),
            Encoding.UTF8,
            "application/json");

            var response = await _httpClient.PostAsync($"api/Address/{propertyId}/address", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }


        //get address by city
        public async Task<List<Address>> GetCityAddressAsync(string city)
        {
            var response = await _httpClient.GetAsync($"/api/Address/cityAddress/{city}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Address>>(jsonResponse);
        }

        //edit address
        public async Task<HttpResponseMessage> EditAddressAsync(string propertyId, Address address)
        {      
            var response = await _httpClient.PutAsJsonAsync($"api/Address/{propertyId}/updateAddress", address);
            response.EnsureSuccessStatusCode();
            return response;
        }

        //patch postal code
        public async Task<HttpResponseMessage> PatchAddressPostalCodeAsync(AddresspostalCodeUpdateViewModel addresssPostalCode)
        {
            // Creating a patch document
            var patchDoc = new JsonPatchDocument();
            patchDoc.Replace("/PostalCode", addresssPostalCode.PostalCode);

            // Serializing the patch document
            var serializedPatchDoc = new StringContent(
                JsonConvert.SerializeObject(patchDoc),
                Encoding.UTF8,
                "application/json-patch+json");

            // Sending the PATCH request
            var response = await _httpClient.PatchAsync($"api/Address/{addresssPostalCode.PropertyId}/updateAddress", serializedPatchDoc);

            // Ensure the response is successful
            response.EnsureSuccessStatusCode();

            return response;
        }

        //Delete Address
        public async Task<HttpResponseMessage> DeleteAddressAsync(string propertyId)
        {
            var response = await _httpClient.DeleteAsync($"api/Address/{propertyId}/deleteAddress");
            response.EnsureSuccessStatusCode();
            return response;
        }


    }
}
