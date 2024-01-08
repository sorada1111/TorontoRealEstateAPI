using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper.Features;
using RealEstateAPI.Connector;
using RealEstateAPI.DTO.Addresses;
using RealEstateAPI.DTO.Features;
using RealEstateLibrary.Models;

namespace RealEstateAPI.Services
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly AWSConnector _awsConnector;
        private readonly string _tableName = "Property";
        private readonly Table _table;

        public FeatureRepository(AWSConnector awsConnector)
        {
            _awsConnector = awsConnector;
            _table = _awsConnector.LoadContentTable(_tableName);
        }

        public async Task<IEnumerable<Feature>> GetFeaturesAsync()
        {
            DynamoDBContext context = _awsConnector.Context;

            // Perform a scan operation to retrieve the Property items
            var properties = await context.ScanAsync<Property>(new List<ScanCondition>()).GetRemainingAsync();

            // Extract the Feature attribute from each Property item
            var features = properties
             .Where(property => property.Features != null)
             .Select(property => property.Features)
             .ToList();

            return features;
        }

        public async Task<IEnumerable<Feature>> GetFeaturesBySizeAsync(string size)
        {
            DynamoDBContext context = _awsConnector.Context;

            // Perform a scan operation to retrieve the Property items
            var properties = await context.ScanAsync<Property>(new List<ScanCondition>()).GetRemainingAsync();

            // Extract the Feature attribute from each Property item and filter by size
            var FeaturesWithSize = properties
                .Where(property => property.Features != null)
                .Select(property => property.Features)
                .Where(feature => feature.Size.Equals(size, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return FeaturesWithSize;
        }

        public async Task<IEnumerable<Property>> GetPropertiesBySizeAsync(string size)
        {
            DynamoDBContext context = _awsConnector.Context;

            // Perform a scan operation to retrieve the Property items
            var allProperties = await context.ScanAsync<Property>(new List<ScanCondition>()).GetRemainingAsync();

            // Filter properties by size
            var propertiesWithSize = allProperties
                .Where(property => property.Features != null &&
                                   property.Features.Size.Equals(size, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return propertiesWithSize;
        }

        public async Task<bool> AddFeatureToPropertyAsync(string propertyId, FeatureDto featureDto)
        {
            DynamoDBContext context = _awsConnector.Context;
            var property = await context.LoadAsync<Property>(propertyId);
            if (property == null) return false;

            if (property.Features == null)
            {
                property.Features = new Feature(); 
            }

            var newFeature = new Feature
            {
                Size = featureDto.Size,
                Rooms = featureDto.Rooms,
                Bathrooms = featureDto.Bathrooms,
                HasParking = featureDto.HasParking,
                WalkScore = featureDto.WalkScore,
                TransitScore = featureDto.TransitScore,
                BikeScore = featureDto.BikeScore
            };

            property.Features = newFeature;

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

        public async Task<bool> UpdateFeature(string propertyId, Feature updatedFeature)
        {
            try
            {
                DynamoDBContext context = _awsConnector.Context;

                Property property = await context.LoadAsync<Property>(propertyId);
                if (property == null) return false;

                // Check if there are feature to update
                if (property.Features == null)
                {
                    
                    property.Features = new Feature();
                }
                else
                {       
                        property.Features = updatedFeature;                   
                    
                }
                await context.SaveAsync(property);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while updating feature: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteFeatureFromPropertyAsync(string propertyId)
        {
            DynamoDBContext context = _awsConnector.Context;

            // Retrieve the property by PropertyId
            var property = await context.LoadAsync<Property>(propertyId);
            if (property == null)
            {
                return false; // Property not found
            }

            // Clear the feature list
            property.Features =null;

            try
            {
                // Save the updated property
                await context.SaveAsync(property);
                return true; // Deletion of feature successful
            }
            catch (Exception ex)
            {
                return false; // Deletion failed
            }
        }


    }
}
