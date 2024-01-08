using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.DTO.Agencies;
using RealEstateAPI.Services;
using RealEstateLibrary.Models;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly IRealEstateRespository _realEstateRespository;
        private readonly IAgentRepository _agentRepository;
        private readonly IMapper _mapper;

        public AgentController(IRealEstateRespository realEstateRespository, IAgentRepository agentRepository, IMapper mapper)
        {
            _realEstateRespository = realEstateRespository;
            _agentRepository = agentRepository;
            _mapper = mapper;
        }

        //Get all agents
        [HttpGet]
        public async Task<ActionResult<Agent>> GetAllAgents()
        {
            var agents = await _agentRepository.GetAgentsAsync();
            var results = _mapper.Map<IEnumerable<AgentDto>>(agents);
            return Ok(results);
        }

        //Get Agent By Id
        [HttpGet("{agentId}")]
        public async Task<ActionResult<Agent>> GetAgentById(string agentId)
        {
            var agent = await _agentRepository.GetAgentByIdAsync(agentId);
            if (agent == null)
            {
                return NotFound();
            }
            var results = _mapper.Map<AgentDto>(agent);
            return Ok(results);
        }

        //add agent
        [HttpPost("{propertyId}/agent")]
        public async Task<IActionResult> AddAgentToProperty(string propertyId, [FromBody] AgentCreationDto agentDto)
        {
            var agent = _mapper.Map<AgentCreationDto>(agentDto);

            var success = await _agentRepository.AddAgentToPropertyAsync(propertyId, agent);

            if (!success)
            {
                return NotFound($"Property with ID {propertyId} not found.");
            }
            return Ok(new { Message = "Agent created successfully.", AgentCreationDto = agentDto });
        }

        //Update Agent
        [HttpPut("{propertyId}/updateAgent/{agentId}")]
        public async Task<IActionResult> UpdateAgent(string propertyId, string agentId, [FromBody] AgentUpdateDto agentUpdateDto)
        {
            var agentToUpdate = await _agentRepository.GetAgentByIdAsync(agentId);
            if (agentToUpdate == null)
            {
                return NotFound();
            }

            _mapper.Map(agentUpdateDto, agentToUpdate);

            var updateResult = await _agentRepository.UpdateAgent(propertyId, agentToUpdate);

            if (updateResult)
                return Ok("Agent updated successfully."); 
            else
                return StatusCode(500, "An error occurred while updating the agent.");
        }

        //Patch Agent
        [HttpPatch("{propertyId}/agents/{agentId}")]
        public async Task<IActionResult> UpdateAgentPatch(string propertyId, string agentId, [FromBody] JsonPatchDocument<Agent> patchDoc)
        {
            var agentToUpdate = await _agentRepository.GetAgentByIdAsync(agentId);
            if (agentToUpdate == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(agentToUpdate, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updateResult = await _agentRepository.UpdateAgent(propertyId, agentToUpdate);

            if (updateResult)
                return Ok(new { Message = "Agent updated successfully." });
            else
                return StatusCode(500, "An error occurred while updating the agent.");
        }

        //Delete agent in the property
        [HttpDelete("{propertyId}/agent/{agentId}")]
        public async Task<IActionResult> DeleteAgentFromProperty(string propertyId, string agentId)
        {
            var success = await _agentRepository.DeleteAgentFromPropertyAsync(propertyId, agentId);

            if (!success)
            {
                return NotFound($"Property with ID {propertyId} not found or agent with ID {agentId} not found in the property.");
            }

            return Ok(new { Message = "Agent deleted from property successfully." });
        }

    }
}
