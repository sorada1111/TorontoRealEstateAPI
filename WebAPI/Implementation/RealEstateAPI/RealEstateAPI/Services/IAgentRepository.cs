using RealEstateAPI.DTO.Agencies;
using RealEstateLibrary.Models;

namespace RealEstateAPI.Services
{
    public interface IAgentRepository
    {
        Task<IEnumerable<Agent>> GetAgentsAsync();
        Task<Agent> GetAgentByIdAsync(string agentId);
        Task<bool> AddAgentToPropertyAsync(string propertyId, AgentCreationDto agentDto);
        Task<bool> UpdateAgent(string propertyId, Agent agent);
        Task<bool> DeleteAgentFromPropertyAsync(string propertyId, string agentId);
    }
}
