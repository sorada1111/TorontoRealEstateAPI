using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RealEstateApp.Models;
using RealEstateApp.Service;

namespace RealEstateApp.Controllers
{
    public class AddressController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IAddressService _addressService;

        public AddressController(IPropertyService propertyService, IAddressService addressService)
        {
            _propertyService = propertyService;
            _addressService = addressService;
        }

        // GET: List all address
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

        //Add Address to the property 
        [HttpGet]
        public IActionResult AddAddress(string propertyId)
        {
            ViewBag.PropertyId = propertyId;
            return View(new Address());
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(string propertyId, Address address)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PropertyId = propertyId; // Reset propertyId in ViewBag in case of error
                return View(address);
            }

            bool isAdded = await _addressService.AddAddressToPropertyAsync(propertyId, address);
            if (isAdded)
            {
                return RedirectToAction("Index","Address");
            }
            else
            {
                ViewBag.PropertyId = propertyId;
                ModelState.AddModelError("", "Failed to add address");
                return View(address);
            }
        }

        //Get by city 
        [HttpGet]
        public async Task<IActionResult> SearchByCity(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return RedirectToAction("Index"); 
            }
            try
            {
                var allProperties = await _propertyService.GetAllPropertiesAsync();

                var addressesInCity = await _addressService.GetCityAddressAsync(city);
                var propertiesWithCityAddress = allProperties
                .Where(property => addressesInCity
                .Any(cityAddress => cityAddress.StreetAddress == property.PropertyAddresses?.StreetAddress &&
                            cityAddress.City == property.PropertyAddresses?.City &&
                            cityAddress.State == property.PropertyAddresses?.State &&
                            cityAddress.PostalCode == property.PropertyAddresses?.PostalCode))
                .ToList();


                return View("Index", propertiesWithCityAddress);
            }
            catch (HttpRequestException e)
            {
                return View("Error");
            }
        }

        //Put Address / Edit Address
        [HttpGet]
        public async Task<IActionResult> EditAddress(string propertyId)
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property?.PropertyAddresses == null)
            {
                return NotFound();
            }

            var addressToEdit = property.PropertyAddresses; 

            ViewBag.PropertyId = propertyId;
            return View("EditAddress", addressToEdit); 
        }


        [HttpPost]
        public async Task<IActionResult> EditAddress(string propertyId, Address address)
        {
            if (!ModelState.IsValid)
            {
                return View(address); 
            }

            var response = await _addressService.EditAddressAsync(propertyId, address);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the address.");
                return View(address);
            }
        }


        // GET: /api/Address/{propertyId}/editPostalCode
        [HttpGet]
        public async Task<IActionResult> EditAddressPostalCode(string propertyId)
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property?.PropertyAddresses == null)
            {
                return NotFound();
            }

            var addressToEdit = property.PropertyAddresses;

            ViewBag.PropertyId = propertyId;

            var model = new AddresspostalCodeUpdateViewModel
            {
                PropertyId = propertyId,
                PostalCode = addressToEdit.PostalCode 
            };

            return View("EditAddressPostalCode", model);
        }


        // POST: /api/Address/{propertyId}/editPostalCode
        [HttpPost]
        public async Task<IActionResult> EditAddressPostalCode(AddresspostalCodeUpdateViewModel addressPostalCode)
        {
            if (ModelState.IsValid)
            {
                var response = await _addressService.PatchAddressPostalCodeAsync(addressPostalCode);
                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }

            }
            return View(addressPostalCode);
        }

        //Delete /api/Address/{propertyId}/deleteAddress
        [HttpPost]
        public async Task<IActionResult> Delete(string propertyId)
        {
            try
            {
                await _addressService.DeleteAddressAsync(propertyId);
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                return View("Error");
            }
        }


    }
}
