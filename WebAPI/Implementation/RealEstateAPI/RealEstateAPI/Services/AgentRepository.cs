using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RealEstateAPI.Connector;
using RealEstateAPI.DTO.Agencies;
using RealEstateLibrary.Models;

namespace RealEstateAPI.Services
{
    public class AgentRepository : IAgentRepository
    {
        private readonly AWSConnector _awsConnector;
        private readonly string _tableName = "Property";
        private readonly Table _table;

        public AgentRepository(AWSConnector awsConnector)
        {
            _awsConnector = awsConnector;
            _table = _awsConnector.LoadContentTable(_tableName);
        }
        public async Task<IEnumerable<Agent>> GetAgentsAsync()
        {
            DynamoDBContext context = _awsConnector.Context;

            // Perform a scan operation to retrieve the Property items
            var properties = await context.ScanAsync<Property>(new List<ScanCondition>()).GetRemainingAsync();

            // Extract the Agent attribute from each Property item
            var agents = properties
             .Where(property => property.Agents != null)
             .Select(property => property.Agents)
             .ToList();

            return agents;
        }
        public async Task<Agent> GetAgentByIdAsync(string agentId)
        {
            DynamoDBContext context = _awsConnector.Context;

            // Perform a scan operation to retrieve the Property items
            var properties = await context.ScanAsync<Property>(new List<ScanCondition>()).GetRemainingAsync();

            // Extract the Agent attribute from each Property item
            var agents = properties
                .Where(property => property.Agents != null)
                .Select(property => property.Agents)
                .ToList();

            // Find the agent by AgentId
            var agent = agents.FirstOrDefault(c => c.AgentId == agentId);

            return agent;
        }
        public async Task<bool> AddAgentToPropertyAsync(string propertyId, AgentCreationDto agentDto)
        {
            DynamoDBContext context = _awsConnector.Context;
            var property = await context.LoadAsync<Property>(propertyId);
            if (property == null) return false;

            if (property.Agents == null)
            {
                property.Agents = new Agent();  
            }

           
            var newAgent = new Agent
            {
                AgentId = Guid.NewGuid().ToString(),
                AgentName = agentDto.AgentName,
                AgentCompanyName = agentDto.AgentCompanyName,
                AgentPhone = agentDto.AgentPhone
            };

            property.Agents = newAgent;

            try
            {
                await context.SaveAsync(property);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAgent(string propertyId, Agent updatedAgent)
        {
            try
            {
                DynamoDBContext context = _awsConnector.Context;
                Property property = await context.LoadAsync<Property>(propertyId);
                if (property == null) return false;

                // Check if Agents dictionary is initialized
                if (property.Agents == null)
                {
                    property.Agents = new Agent();
                }

                // Check if the agent exists
                if (property.Agents != null)
                {
                    // Update the existing agent
                    property.Agents = updatedAgent;
                }
                else
                {
                    // Add the new agent to the dictionary
                    property.Agents = updatedAgent;
                }

                // Save the updated property item
                await context.SaveAsync(property);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while updating agent: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAgentFromPropertyAsync(string propertyId, string agentId)
        {
            DynamoDBContext context = _awsConnector.Context;

            // Retrieve the property by PropertyId
            var property = await context.LoadAsync<Property>(propertyId);

            if (property == null || property.Agents == null)
            {
                return false; // Property not found or has no agents
            }

            // Check if the agent exists in the property's agents
            if (property.Agents.AgentId == agentId)
            {
                // Remove the agent reference from the property
                property.Agents = null;


                try
                {
                    // Save the updated property
                    await context.SaveAsync(property);
                    return true; // Deletion successful
                }
                catch (Exception ex)
                {
                    return false; // Deletion failed
                }
            }
            else
            {
                return false; // Agent not found in property
            }
        }


    }
}
