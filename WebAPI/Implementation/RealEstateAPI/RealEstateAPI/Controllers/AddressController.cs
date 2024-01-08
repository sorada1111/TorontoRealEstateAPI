using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.DTO.Addresses;
using RealEstateAPI.DTO.Property;
using RealEstateAPI.Services;
using RealEstateLibrary.Models;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IRealEstateRespository _realEstateRespository;
        private readonly IAddressRepository _addressRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;


        public AddressController(IRealEstateRespository realEstateRespository, IAddressRepository addressRepository, IFileStorageService fileStorageService, IMapper mapper)
        {
            _realEstateRespository = realEstateRespository;
            _addressRepository = addressRepository;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        //Get all addresses
        [HttpGet]
        public async Task<ActionResult<Address>> GetAllAddresses()
        {
            var addresses = await _addressRepository.GetAddressesAsync();
            var results = _mapper.Map<IEnumerable<AddressDto>>(addresses);
            return Ok(results);
        }

        //get addresses in the city
        [HttpGet("cityAddress/{city}")]
        public async Task<ActionResult<Address>> GetAddressByCity(string city)
        {
            var addresses = await _addressRepository.GetAddressByCityAsync(city);
            if (addresses == null)
            {
                return NotFound();
            }
            var results = _mapper.Map<IEnumerable<AddressDto>>(addresses);
            return Ok(results);
        }


        //get all properties in the city
        [HttpGet("cityProperty/{city}")]
        public async Task<ActionResult<Property>> GetPropertyByCityAsync(string city)
        {
            var properties = await _addressRepository.GetPropertiesByCityAsync(city);
            if (properties == null)
            {
                return NotFound();
            }
            var results = _mapper.Map<IEnumerable<PropertyDto>>(properties);
            return Ok(results);
        }


        //get all properties in the areaCode
        [HttpGet("areaCodeProperty/{areaCode}")]
        public async Task<ActionResult<Property>> GetPropertyByAreaAsync(string areaCode)
        {
            var properties = await _addressRepository.GetPropertiesByAreaAsync(areaCode);
            if (properties == null)
            {
                return NotFound();
            }
            var results = _mapper.Map<IEnumerable<PropertyDto>>(properties);
            return Ok(results);
        }



        //add address to the property 
        [HttpPost("{propertyId}/address")]
        public async Task<IActionResult> AddAddressToProperty(string propertyId, [FromBody] AddressDto addressDto)
        {
            var address = _mapper.Map<AddressDto>(addressDto);

            var success = await _addressRepository.AddAddressToPropertyAsync(propertyId, address);

            if (!success)
            {
                return NotFound($"Property with ID {propertyId} not found.");
            }
            return Ok(new { Message = "Address created successfully.", AddressDto = addressDto });
        }

        //Update address
        [HttpPut("{propertyId}/updateAddress")]
        public async Task<IActionResult> UpdateAddress(string propertyId, [FromBody] AddressDto addressDto)
        {
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }
            var updatedAddress = _mapper.Map<Address>(addressDto);

            // Update the address in the repository
            var updateResult = await _addressRepository.UpdateAddress(propertyId, updatedAddress);

            if (updateResult)
                return Ok("Address updated successfully.");
            else
                return StatusCode(500, "An error occurred while updating the address.");
        }

        //Patch address
        [HttpPatch("{propertyId}/updateAddress")]
        public async Task<IActionResult> UpdateAddressPatch(string propertyId, [FromBody] JsonPatchDocument<Address> patchDoc)
        {
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            if (property.PropertyAddresses == null)
            {
                return BadRequest("No addresses available to update.");
            }
            var addressToUpdate = property.PropertyAddresses;
            if (addressToUpdate == null)
            {
                return BadRequest("No valid address found.");
            }

            // Apply the patch to this address
            patchDoc.ApplyTo(addressToUpdate, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updateResult = await _addressRepository.UpdateAddress(propertyId, addressToUpdate);

            if (updateResult)
                return Ok(new { Message = "Address updated successfully." });
            else
                return StatusCode(500, "An error occurred while updating the address.");
        }

        //Delete address in the property
        [HttpDelete("{propertyId}/deleteAddress")]
        public async Task<IActionResult> DeleteAddressFromProperty(string propertyId)
        {
            var property = await _realEstateRespository.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            var success = await _addressRepository.DeleteAddressFromPropertyAsync(propertyId);

            if (!success)
            {
                return NotFound($"Cannot delete address from the property ID: {propertyId}.");
            }

            return Ok(new { Message = "Address deleted from property successfully." });
        }


    }
}
