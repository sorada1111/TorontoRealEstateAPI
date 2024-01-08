using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Models;
using RealEstateApp.Service;
using System.Diagnostics;

namespace RealEstateApp.Controllers
{
    public class AgentController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IAgentService _agentService;

        public AgentController(IPropertyService propertyService, IAgentService agentService)
        {
            _propertyService = propertyService;
            _agentService = agentService;
        }

        // GET: List all agent
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var properties = await _propertyService.GetAllPropertiesAsync();
                if (properties == null || !properties.Any())
                {
                    return View("NoProperties");
                }
                return View(properties);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AgentDetails(string agentId)
        {
            try
            {
                var agents = await _agentService.GetAgentByIDAsync(agentId);
                var agent = agents.FirstOrDefault();
                if (agent == null)
                {
                    return NotFound();
                }

                return View(agent);
            }
            catch (HttpRequestException ex)
            {
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


        //Add agent to the property 
        [HttpGet]
        public IActionResult AddAgent(string propertyId)
        {
            ViewBag.PropertyId = propertyId;
            return View(new Agent());
        }

        [HttpPost]
        public async Task<IActionResult> AddAgent(string propertyId, Agent agent)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PropertyId = propertyId;
                return View(agent);
            }

            bool isAdded = await _agentService.AddAgentToPropertyAsync(propertyId, agent);
            if (isAdded)
            {
                return RedirectToAction("Index", "Agent");
            }
            else
            {
                ViewBag.PropertyId = propertyId;
                ModelState.AddModelError("", "Failed to add agent");
                return View(agent);
            }
        }



        //Put Agent / Edit Agent
        [HttpGet]
        public async Task<IActionResult> EditAgent(string propertyId, string agentId)
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property?.Agents == null)
            {
                return NotFound();
            }

            var agentToEdit = property.Agents;

            ViewBag.PropertyId = propertyId;
            return View("EditAgent", agentToEdit);
        }


        [HttpPost]
        public async Task<IActionResult> EditAgent(string propertyId, string agentId, Agent agent)
        {
            if (!ModelState.IsValid)
            {
                return View(agent);
            }

            var response = await _agentService.EditAgentAsync(propertyId, agentId, agent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the address.");
                return View(agent);
            }
        }


        // GET: /api/Agent/{propertyId}/agents/{agentId}
        [HttpGet]
        public async Task<IActionResult> EditAgentPhoneNumber(string propertyId, string agentId, AgentPhoneNumberUpdateViewModel agentPhoneNumber)
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property?.Agents == null)
            {
                return NotFound();
            }

            var agentToEdit = property.Agents;

            ViewBag.PropertyId = propertyId;

            var model = new AgentPhoneNumberUpdateViewModel
            {
                PropertyId = propertyId,
                AgentId = agentId,
                AgentPhone = agentToEdit.AgentPhone 
            };

            return View("EditAgentPhoneNumber", model);
        }


        // POST: /api/Agent/{propertyId}/agents/{agentId}
        [HttpPost]
        public async Task<IActionResult> EditAgentPhoneNumber(AgentPhoneNumberUpdateViewModel agentPhoneNumber)
        {
            if (ModelState.IsValid)
            {
                var response = await _agentService.PatchAgentPhoneNumberAsync(agentPhoneNumber);
                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }

            }
            return View(agentPhoneNumber);
        }


        //Delete /api/Agent/{propertyId}/agent/{agentId}
        [HttpPost]
        [Route("Agent/Delete/{propertyId}/{agentId}")]
        public async Task<IActionResult> Delete(string propertyId, string agentId)
        {
            try
            {
                await _agentService.DeleteAgentAsync(propertyId, agentId);
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                return View("Error");
            }
        }



    }
}
