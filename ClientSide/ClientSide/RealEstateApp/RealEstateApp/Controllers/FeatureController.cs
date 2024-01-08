using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Models;
using RealEstateApp.Service;

namespace RealEstateApp.Controllers
{
    public class FeatureController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IFeatureService _featureService;

        public FeatureController(IPropertyService propertyService, IFeatureService featureService)
        {
            _propertyService = propertyService;
            _featureService = featureService;
        }

        // GET: List all features
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

        //Add Feature to the property 
        [HttpGet]
        public IActionResult AddFeatures(string propertyId)
        {
            ViewBag.PropertyId = propertyId;
            return View(new Feature());
        }

        [HttpPost]
        public async Task<IActionResult> AddFeatures(string propertyId, Feature features)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PropertyId = propertyId; // Reset propertyId in ViewBag in case of error
                return View(features);
            }

            bool isAdded = await _featureService.AddFeaturesToPropertyAsync(propertyId, features);
            if (isAdded)
            {
                return RedirectToAction("Index", "Feature");
            }
            else
            {
                ViewBag.PropertyId = propertyId;
                ModelState.AddModelError("", "Failed to add Feature");
                return View(features);
            }
        }


        //Put Features / Edit Features
        [HttpGet]
        public async Task<IActionResult> EditFeatures(string propertyId)
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property?.Features == null)
            {
                return NotFound();
            }

            var featuresToEdit = property.Features;

            ViewBag.PropertyId = propertyId;
            return View("EditFeatures", featuresToEdit);
        }


        [HttpPost]
        public async Task<IActionResult> EditFeatures(string propertyId, Feature features)
        {
            if (!ModelState.IsValid)
            {
                return View(features);
            }

            var response = await _featureService.EditFeaturesAsync(propertyId, features);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the features.");
                return View(features);
            }
        }

        //Delete features
        [HttpPost]
        public async Task<IActionResult> Delete(string propertyId)
        {
            try
            {
                await _featureService.DeleteFeaturesAsync(propertyId);
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                return View("Error");
            }
        }



    }
}
