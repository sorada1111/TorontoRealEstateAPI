using Newtonsoft.Json;
using RealEstateApp.Models;
using System.Text;

namespace RealEstateApp.Service
{
    public class FeatureService : IFeatureService
    {
        private readonly HttpClient _httpClient;

        public FeatureService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<bool> AddFeaturesToPropertyAsync(string propertyId, Feature features)
        {
            var jsonContent = new StringContent(
            JsonConvert.SerializeObject(features),
            Encoding.UTF8,
            "application/json");
           
            var response = await _httpClient.PostAsync($"api/Feature/{propertyId}/feature", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }

        //edit address
        public async Task<HttpResponseMessage> EditFeaturesAsync(string propertyId, Feature features)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Feature/{propertyId}/updateFeature", features);
            response.EnsureSuccessStatusCode();
            return response;
        }

        //Delete Address
        public async Task<HttpResponseMessage> DeleteFeaturesAsync(string propertyId)
        {
            var response = await _httpClient.DeleteAsync($"api/Feature/{propertyId}/deleteFeature");
            response.EnsureSuccessStatusCode();
            return response;
        }


    }
}
