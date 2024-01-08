using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.DTO.Addresses;
using RealEstateAPI.DTO.Features;
using RealEstateAPI.DTO.Property;
using RealEstateAPI.Services;
using RealEstateLibrary.Models;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        private readonly IRealEstateRespository _realEstateRespository;
        private readonly IFeatureRepository _featureRepository;
        private readonly IMapper _mapper;


        public FeatureController(IRealEstateRespository realEstateRespository, IFeatureRepository featureRepository, IMapper mapper)
        {
            _realEstateRespository = realEstateRespository;
            _featureRepository = featureRepository;
            _mapper = mapper;
        }

        //Get all Features
        [HttpGet]
        public async Task<ActionResult<Feature>> GetAllFeatures()
        {
            var features = await _featureRepository.GetFeaturesAsync();
            var results = _mapper.Map<IEnumerable<FeatureDto>>(features);
            return Ok(results);
        }

        //get feature with size
        [HttpGet("sizeFeature/{size}")]
        public async Task<ActionResult<Feature>> GetFeatureWithSize(string size)
        {
            var features = await _featureRepository.GetFeaturesBySizeAsync(size);
            if (features == null)
            {
                return NotFound();
            }
            var results = _mapper.Map<IEnumerable<FeatureDto>>(features);
            return Ok(results);
        }


        //get all properties with size
        [HttpGet("sizeProperty/{size}")]
        public async Task<ActionResult<Property>> GetPropertyWithSize(string size)
        {
            var properties = await _featureRepository.GetPropertiesBySizeAsync(size);
            if (properties == null)
            {
                return NotFound();
            }
            var results = _mapper.Map<IEnumerable<PropertyDto>>(properties);
            return Ok(results);
        }

        //add feature to the property 
        [HttpPost("{propertyId}/feature")]
        public async Task<IActionResult> AddFeatureToProperty(string propertyId, [FromBody] FeatureDto featureDto)
        {
            var feature = _mapper.Map<FeatureDto>(featureDto);

            var success = await _featureRepository.AddFeatureToPropertyAsync(propertyId, feature);

            if (!success)
            {
                return NotFound($"Property with ID {propertyId} not found.");
            }
            return Ok(new { Message = "Feature created successfully.", FeatureDto = featureDto });
        }

        //Update feature
        [HttpPut("{propertyId}/updateFeature")]
        public async Task<IActionResult> UpdateFeature(string propertyId, [FromBody] FeatureDto featureDto)
        {
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }
            var updatedFeature = _mapper.Map<Feature>(featureDto);

            // Update the feature in the repository
            var updateResult = await _featureRepository.UpdateFeature(propertyId, updatedFeature);

            if (updateResult)
                return Ok("Feature updated successfully.");
            else
                return StatusCode(500, "An error occurred while updating the feature.");
        }

        //Patch Category
        [HttpPatch("{propertyId}/updateFeature")]
        public async Task<IActionResult> UpdateFeaturePatch(string propertyId, [FromBody] JsonPatchDocument<Feature> patchDoc)
        {
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            if (property.Features == null)
            {
                return BadRequest("No feature available to update.");
            }

            var featureToUpdate = property.Features;

            patchDoc.ApplyTo(featureToUpdate, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updateResult = await _featureRepository.UpdateFeature(propertyId, featureToUpdate);

            if (updateResult)
                return Ok(new { Message = "Feature updated successfully." });
            else
                return StatusCode(500, "An error occurred while updating the feature.");
        }

        //Delete feature in the property
        [HttpDelete("{propertyId}/deleteFeature")]
        public async Task<IActionResult> DeleteFeatureFromProperty(string propertyId)
        {
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            var success = await _featureRepository.DeleteFeatureFromPropertyAsync(propertyId);

            if (!success)
            {
                return NotFound($"Cannot delete feature from the property ID: {propertyId}.");
            }

            return Ok(new { Message = "Feature deleted from property successfully." });
        }


    }
}
