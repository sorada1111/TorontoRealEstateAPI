using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RealEstateApp.Models;
using System.Net;
using System.Text;

namespace RealEstateApp.Service
{
    public class AgentService : IAgentService
    {
        private readonly HttpClient _httpClient;

        public AgentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    
        public async Task<List<Agent>> GetAgentByIDAsync(string agentId)
        {
            var response = await _httpClient.GetAsync($"/api/Agent/{agentId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Deserialize as a single Agent object
            var agent = JsonConvert.DeserializeObject<Agent>(jsonResponse);

            // Wrap the single agent in a list
            var agents = new List<Agent>();
            if (agent != null)
            {
                agents.Add(agent);
            }

            return agents;
        }

        //add agent
        public async Task<bool> AddAgentToPropertyAsync(string propertyId, Agent agent)
        {
            var jsonContent = new StringContent(
            JsonConvert.SerializeObject(agent),
            Encoding.UTF8,
            "application/json");

            var response = await _httpClient.PostAsync($"api/Agent/{propertyId}/agent", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }


        //edit agent
        public async Task<HttpResponseMessage> EditAgentAsync(string propertyId, string agentId, Agent agent)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Agent/{propertyId}/updateAgent/{agentId}", agent);
            response.EnsureSuccessStatusCode();
            return response;
        }

        //Patch phone number
        public async Task<HttpResponseMessage> PatchAgentPhoneNumberAsync(AgentPhoneNumberUpdateViewModel agentPhoneNumber)
        {
            // Creating a patch document
            var patchDoc = new JsonPatchDocument();
            patchDoc.Replace("/AgentPhone", agentPhoneNumber.AgentPhone);

            // Serializing the patch document
            var serializedPatchDoc = new StringContent(
                JsonConvert.SerializeObject(patchDoc),
                Encoding.UTF8,
                "application/json-patch+json");
 
            // Sending the PATCH request
            var response = await _httpClient.PatchAsync($"api/Agent/{agentPhoneNumber.PropertyId}/agents/{agentPhoneNumber.AgentId}", serializedPatchDoc);

            // Ensure the response is successful
            response.EnsureSuccessStatusCode();
            return response;
        }


        //Delete Agent
        public async Task<HttpResponseMessage> DeleteAgentAsync(string propertyId, string agentId)
        {
            var response = await _httpClient.DeleteAsync($"api/Agent/{propertyId}/agent/{agentId}");
            response.EnsureSuccessStatusCode();
            return response;
        }

    }
}
