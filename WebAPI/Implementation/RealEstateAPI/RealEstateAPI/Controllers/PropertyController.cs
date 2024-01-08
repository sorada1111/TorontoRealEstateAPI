using Amazon.S3.Transfer;
using Amazon.S3;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Services;
using RealEstateLibrary.Models;
using RealEstateAPI.DTO.Property;
using Microsoft.AspNetCore.JsonPatch;
using static System.Net.Mime.MediaTypeNames;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IRealEstateRespository _realEstateRespository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        
        public PropertyController(IRealEstateRespository realEstateRespository, IFileStorageService fileStorageService, IMapper mapper)
        {
            _realEstateRespository = realEstateRespository;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Property>> GetAllProperties() 
        {
            var properties = await _realEstateRespository.GetPropertiesAsync();
            var results = _mapper.Map<IEnumerable<PropertyDto>>(properties);
            return Ok(results);
        }

        [HttpGet("{propertyId}")]
        public async Task<ActionResult<Property>> GetPropertyById(string propertyId)
        {
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }
            var results = _mapper.Map<PropertyDto>(property);
            return Ok(results);
        }


        [HttpGet("propertiesType/{propertyType}")]
        public async Task<ActionResult<IEnumerable<Property>>> GetPropertyByTypeAsync(string propertyType)
        {
            var properties = await _realEstateRespository.GetPropertyByTypeAsync(propertyType);
            if (properties == null || !properties.Any())
            {
                return NotFound();
            }
            var results = _mapper.Map<IEnumerable<PropertyWithoutOthersAttributeDto>>(properties);
            return Ok(results);
        }

        [HttpGet("price-range/{minPrice}/{maxPrice}")]
        public async Task<ActionResult<IEnumerable<Property>>> GetPropertiesByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var properties = await _realEstateRespository.GetPropertyByPriceRangeAsync(minPrice, maxPrice);

            // Check if the returned collection is empty, not just null
            if (properties == null || !properties.Any())
            {
                return NotFound();
            }

            var results = _mapper.Map<IEnumerable<PropertyWithoutOthersAttributeDto>>(properties);
            return Ok(results);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<Property>> GetPropertyByStatus(string status)
        {
            var properties = await _realEstateRespository.GetPropertyByStatusAsync(status);
            if (properties == null)
            {
                return NotFound();
            }
            var results = _mapper.Map<IEnumerable<PropertyWithoutOthersAttributeDto>>(properties);
            return Ok(results);
        }


        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromBody] PropertyCreateionDto propertyDto)
        {
            if (propertyDto == null)
            {
                return BadRequest("Property data is required.");
            }

            try
            {
                // Use AutoMapper to map DTO to Entity
                var property = _mapper.Map<Property>(propertyDto);

                // Add the property using the repository
                var createdProperty = await _realEstateRespository.AddProperty(property);

                return Ok(new { Message = "Property created successfully.", Property = createdProperty });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the property.");
            }

        }

        //Add New Images to the Existing Property
        [HttpPost("{propertyId}/images")]
        public async Task<IActionResult> UploadImages(string propertyId, [FromForm] IEnumerable<IFormFile> propertyImageUrls)
        {
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            if (propertyImageUrls == null || !propertyImageUrls.Any())
            {
                return BadRequest("No files were selected for upload.");
            }
            try
            {
                // Use AutoMapper to map DTO to Entity
                //var property = _mapper.Map<Property>(propertyDto);

                //// Add the property using the repository
                //var createdProperty = await _realEstateRespository.AddProperty(property, PropertyImageUrls);
                var imageUrls = await _fileStorageService.SaveImagesAsync(propertyImageUrls);

                // Update property with new image URLs
                property.PropertyImageUrls = (List<string>?)imageUrls; // This can be an append or replace operation, depending on your logic

                var updateResult = await _realEstateRespository.UpdateProperty(property);
                if (!updateResult)
                {
                    return StatusCode(500, "Error updating property images in the database.");
                }

                return Ok(new { Message = "Property created successfully.", Property = property });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the property.");
            }

        }

        //Update exisiting property 
        [HttpPut("{propertyId}")]
        public async Task<IActionResult> UpdateProperty(string propertyId, [FromBody] PropertyUpdateDto propertyUpdateDto)
        {
            var propertyToUpdate = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (propertyToUpdate == null)
            {
                return NotFound();
            }

            _mapper.Map(propertyUpdateDto, propertyToUpdate);// Map the updated values

            var updateResult = await _realEstateRespository.UpdateProperty(propertyToUpdate);

            if (updateResult)
                return NoContent(); // Indicate success with NoContent (204)
            else
                return StatusCode(500, "An error occurred while updating the property.");
        }

        //replace the specific propertyImageUrl
        [HttpPut("{propertyId}/imageReplace")]
        public async Task<IActionResult> ReplacePropertyImage(string propertyId,string existingImageUrl,IFormFile newImage)
        {
            // Validate property exists
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            // Validate the existing image URL
            if (string.IsNullOrWhiteSpace(existingImageUrl))
            {
                return BadRequest("Existing image URL is required.");
            }

            // Validate the new image file
            if (newImage == null || newImage.Length == 0)
            {
                return BadRequest("New image file is required.");
            }

            try
            {
                // Save the new image file and get its URL
                var newImageUrl = await _fileStorageService.SaveSingleImageAsync(newImage);

                // Find the existing image in the property's image list and replace it
                var imageIndex = property.PropertyImageUrls.IndexOf(existingImageUrl);
                if (imageIndex != -1)
                {
                    property.PropertyImageUrls[imageIndex] = newImageUrl;
                }
                else
                {
                    return BadRequest("The specified existing image URL does not match any images of the property.");
                }

                // Update the property in the repository
                var updateResult = await _realEstateRespository.UpdateProperty(property);
                if (!updateResult)
                {
                    return StatusCode(500, "Error updating the property image in the database.");
                }

                return Ok(new { Message = "Property image replaced successfully.", NewImageUrl = newImageUrl });
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                return StatusCode(500, "An error occurred while replacing the property image.");
            }
        }


        //Patch
        [HttpPatch("{propertyId}")]
        public async Task<IActionResult> UpdatePropertyPatch(string propertyId, [FromBody] JsonPatchDocument<Property> patchDoc)
        {
            var propertyToUpdate = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (propertyToUpdate == null)
            {
                return NotFound();
            }

            // Apply the patch document to the property entity
            patchDoc.ApplyTo(propertyToUpdate, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Update the property in the repository
            var updateResult = await _realEstateRespository.UpdateProperty(propertyToUpdate);

            if (updateResult)
                return Ok(new { Message = "Property updated successfully."});
            else
                return StatusCode(500, "An error occurred while updating the property.");
        }


        //Delete image
        [HttpDelete("{propertyId}/images")]
        public async Task<IActionResult> DeleteImage(string propertyId, [FromBody] IEnumerable<string> imageUrls)
        {
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound($"Property with ID {propertyId} not found.");
            }

            if (imageUrls == null || !imageUrls.Any())
            {
                return BadRequest("No image URLs provided.");
            }

            var imagesToRemove = property.PropertyImageUrls.Where(url => imageUrls.Contains(url)).ToList();
            if (!imagesToRemove.Any())
            {
                return BadRequest("None of the provided image URLs match the property's images.");
            }

            foreach (var imageUrl in imagesToRemove)
            {
                // Logic to delete the image from storage
                await _fileStorageService.DeleteImageAsync(imageUrl);

                // Remove the image URL from the property's image list
                property.PropertyImageUrls.Remove(imageUrl);
            }

            var updateResult = await _realEstateRespository.UpdateProperty(property);
            if (!updateResult)
            {
                return StatusCode(500, "An error occurred while updating the property.");
            }

            return Ok($"Images successfully deleted from property {propertyId}.");
        }


        //Delete property 
        [HttpDelete("{propertyId}")]
        public async Task<IActionResult> DeleteProperty(string propertyId)
        {
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound($"Property with ID {propertyId} not found.");
            }

            
            if (property.PropertyImageUrls != null)
            {
                //Delete related data, such as images
                foreach (var imageUrl in property.PropertyImageUrls)
                {
                    await _fileStorageService.DeleteImageAsync(imageUrl);
                }
            }

            // Delete the property from the database
            bool deletionResult = await _realEstateRespository.DeletePropertyAsync(propertyId);

            if (deletionResult)
            {
                return Ok($"Property with ID {propertyId} has been successfully deleted.");
            }
            else
            {
                return NotFound($"Property with ID {propertyId} not found.");
            }
        }

    }

}

