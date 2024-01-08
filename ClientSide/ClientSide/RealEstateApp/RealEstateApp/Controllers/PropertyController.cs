using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using RealEstateApp.Models;
using RealEstateApp.Service;

namespace RealEstateApp.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        // GET: Properties
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


        // GET: Properties/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);

            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }




        //search 
        [HttpGet]
        public async Task<IActionResult> SearchProperty(string? city, string? areaCode, string? propertyType, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                var searchResults = await _propertyService.GetPropertyBySearchAsync(city, areaCode, propertyType, minPrice, maxPrice);

                if (searchResults == null || !searchResults.Any())
                {
                    ViewData["Message"] = "No properties found matching the criteria.";
                }
               
                // Pass the search results to the view in both cases
                return View("Index", searchResults ?? new List<Property>());
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewData["Message"] = "No properties found matching the criteria.";
                return View("Index", new List<Property>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                ViewData["Message"] = "No properties found matching the criteria.";
                return View("Index", new List<Property>());
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
                var addressesInCity = await _propertyService.GetPropertyByCityAsync(city);
                if (addressesInCity == null)
                {
                    ViewData["Message"] = "No properties found for the specified city.";
                    return View("Index", new List<Property>()); 
                }
                return View("Index", addressesInCity); 
            }
            catch (HttpRequestException e)
            {
                return View("Error");
            }
        }



        [HttpGet]
        public async Task<IActionResult> CreateProperty()
        {
            return View();
        }
        // Post: /api/Property
       [HttpPost]
       public async Task<IActionResult> CreateProperty(Property property)
       {
           if (ModelState.IsValid)
           {
               try
               {
                   var createdProperty = await _propertyService.CreatePropertyAsync(property);
                   Console.WriteLine("Property ID:" + createdProperty.PropertyId);
                   return Json(new { success = true, propertyId = createdProperty.PropertyId });
               }
               catch (Exception ex)
               {
                   ModelState.AddModelError("", "Error creating the property: " + ex.Message);
               }
           }

           // If model state is not valid, or if an exception occurred
           return Json(new { success = false, message = "Error creating the property" });
       }

        //upload property images
        [HttpPost]
        public async Task<IActionResult> UploadImages(string propertyId, List<IFormFile> PropertyImages)
        {
            var uploadSuccess = await _propertyService.UploadPropertyImagesAsync(propertyId, PropertyImages);
            if (uploadSuccess)
            {
                return Json(new { success = true, message = "Images uploaded successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Error uploading images." });
            }
        }


        // GET: /Property/EditProperty/{propertyId}
        [HttpGet]
        public async Task<IActionResult> EditProperty(string id)
        {           
            var property = await _propertyService.GetPropertyByIdAsync(id);

            if (property == null)
            {           
                return NotFound();
            }
            return View(property);
        }
        // POST: /Property/EditProperty
        [HttpPost]
        public async Task<IActionResult> EditProperty(Property property)
        {
            if (ModelState.IsValid)
            {
                var response = await _propertyService.UpdatePropertyAsync(property);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index"); 
                }
            }
            return View(property);
        }

        // GET: /Property/EditPropertyPrice/{id}
        [HttpGet]
        public IActionResult EditPropertyPrice(string id)
        {
            var model = new PropertyPriceUpdateViewModel { PropertyId = id };
            return View(model);
        }

        // POST: /Property/EditPropertyPrice
        [HttpPost]
        public async Task<IActionResult> EditPropertyPrice(PropertyPriceUpdateViewModel propertyPrice)
        {
            if (ModelState.IsValid)
            {
                var response = await _propertyService.PatchPropertyPriceAsync(propertyPrice);
                if (response.IsSuccessStatusCode)
                {
                    
                    return RedirectToAction("Index"); 
                }
              
            }
            return View(propertyPrice);
        }
        //Delete /api/Property/{propertyId}
        [HttpPost]
        public async Task<IActionResult> Delete(string propertyId)
        {
            try
            {
                await _propertyService.DeletePropertyAsync(propertyId);            
                return RedirectToAction("Index"); 
            }
            catch (HttpRequestException ex)
            {
                return View("Error");
            }
        }

        

    }
}
