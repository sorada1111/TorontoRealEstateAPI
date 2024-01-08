using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using RealEstateApp.Models;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace RealEstateApp.Service
{
    public class PropertyService : IPropertyService
    {
        private readonly HttpClient _httpClient;

        public PropertyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            //_httpClient.BaseAddress = new Uri("https://localhost:7270/");
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            var response = await _httpClient.GetAsync("api/Property"); // Appending the specific endpoint
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<IEnumerable<Property>>();

        }

        public async Task<Property> GetPropertyByIdAsync(string propertyId)
        {
            var response = await _httpClient.GetAsync($"api/Property/{propertyId}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            return await response.Content.ReadAsAsync<Property>();
        }
    
    //get property by city
    public async Task<IEnumerable<Property>> GetPropertyByCityAsync(string city)
        {
            var response = await _httpClient.GetAsync($"/api/Address/cityProperty/{city}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Property>>(jsonResponse);
        }

        //get property by search
        public async Task<IEnumerable<Property>> GetPropertyBySearchAsync(string? city, string? areaCode, string? propertyType, decimal? minPrice, decimal? maxPrice)
        {
            var tasks = new List<Task<HttpResponseMessage>>();

            if (!string.IsNullOrEmpty(city))
            {
                tasks.Add(_httpClient.GetAsync($"/api/Address/cityProperty/{city}"));
            }
            if (!string.IsNullOrEmpty(areaCode))
            {
                tasks.Add(_httpClient.GetAsync($"/api/Address/areaCodeProperty/{areaCode}"));
            }
            if (!string.IsNullOrEmpty(propertyType))
            {
                tasks.Add(_httpClient.GetAsync($"/api/Property/propertiesType/{propertyType}"));
            }
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                tasks.Add(_httpClient.GetAsync($"/api/Property/price-range/{minPrice}/{maxPrice}"));
            }

            // Wait for all the tasks to complete
            var responses = await Task.WhenAll(tasks);

            // Combine results from all responses
            var combinedResults = new List<Property>();
            foreach (var response in responses)
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                combinedResults.AddRange(JsonConvert.DeserializeObject<List<Property>>(jsonResponse));
            }

            return combinedResults.Distinct(new PropertyComparer()).ToList();
        }

        public class PropertyComparer : IEqualityComparer<Property>
        {
            public bool Equals(Property x, Property y)
            {
                if (Object.ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                // Assuming each property has a unique identifier, like an ID
                return x.PropertyId == y.PropertyId;
            }

            public int GetHashCode(Property obj)
            {
                // Use ID for hashing, but check for null
                return obj?.PropertyId.GetHashCode() ?? 0;
            }
        }




        public async Task<Property> CreatePropertyAsync(Property property)
        {
            var jsonContent = new StringContent(
            JsonConvert.SerializeObject(property),
            Encoding.UTF8,
            "application/json");

            var response = await _httpClient.PostAsync("api/Property", jsonContent);
            response.EnsureSuccessStatusCode();
            // Read the response content as a string
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response Content: " + responseBody);

            // Deserialize the response content as a dynamic object
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

            // Access the "property" object from the dynamic object
            Property responseProperty = jsonResponse.property.ToObject<Property>();
            Console.WriteLine("responseProperty: " + responseProperty);
            return responseProperty;
        }

        public async Task<bool> UploadPropertyImagesAsync(string propertyId, List<IFormFile> images)
        {
            using (var content = new MultipartFormDataContent())
            {
                foreach (var image in images)
                {
                    var imageContent = new StreamContent(image.OpenReadStream());
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                    content.Add(imageContent, "propertyImageUrls", image.FileName);
                }

                var response = await _httpClient.PostAsync($"api/Property/{propertyId}/images", content);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<HttpResponseMessage> UpdatePropertyAsync(Property property)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Property/{property.PropertyId}", property);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> PatchPropertyPriceAsync(PropertyPriceUpdateViewModel propertyPrice)
        {
            // Creating a patch document
            var patchDoc = new JsonPatchDocument();
            patchDoc.Replace("/Price", propertyPrice.Price);

            // Serializing the patch document
            var serializedPatchDoc = new StringContent(
                JsonConvert.SerializeObject(patchDoc),
                Encoding.UTF8,
                "application/json-patch+json");

            // Sending the PATCH request
            var response = await _httpClient.PatchAsync($"api/Property/{propertyPrice.PropertyId}", serializedPatchDoc);

            // Ensure the response is successful
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> DeletePropertyAsync(string propertyId)
        {
            var response = await _httpClient.DeleteAsync($"api/Property/{propertyId}");
            response.EnsureSuccessStatusCode(); 
            return response;
        }
    }
}
