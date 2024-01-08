using RealEstateApp.Models;

namespace RealEstateApp.Service
{
    public interface IAgentService
    {
        Task<List<Agent>> GetAgentByIDAsync(string agentId); //get agent by Id
        Task<bool> AddAgentToPropertyAsync(string propertyId, Agent agent); // add agent 
        Task<HttpResponseMessage> EditAgentAsync(string propertyId, string agentId, Agent agent); // edit agent
        Task<HttpResponseMessage> PatchAgentPhoneNumberAsync(AgentPhoneNumberUpdateViewModel agentPhoneNumber); //Patch Phone Number
        Task<HttpResponseMessage> DeleteAgentAsync(string propertyId, string agentId); // Delete Agent 
    }
}
