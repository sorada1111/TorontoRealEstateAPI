using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RealEstateAPI.Connector;
using RealEstateAPI.DTO.Addresses;
using RealEstateLibrary.Models;
using System.Diagnostics.Metrics;

namespace RealEstateAPI.Services
{
    public class AddressRepository: IAddressRepository
    {
        private readonly AWSConnector _awsConnector;
        private readonly string _tableName = "Property";
        private readonly Table _table;

        public AddressRepository(AWSConnector awsConnector)
        {
            _awsConnector = awsConnector;
            _table = _awsConnector.LoadContentTable(_tableName);
        }

        public async Task<IEnumerable<Address>> GetAddressesAsync()
        {
            DynamoDBContext context = _awsConnector.Context;

            // Perform a scan operation to retrieve the Property items
            var properties = await context.ScanAsync<Property>(new List<ScanCondition>()).GetRemainingAsync();

            // Extract the Address attribute from each Property item
            var addresses = properties
             .Where(property => property.PropertyAddresses != null)
             .Select(property => property.PropertyAddresses)
             .ToList();

            return addresses;
        }
        public async Task<IEnumerable<Address>> GetAddressByCityAsync(string city)
        {
            DynamoDBContext context = _awsConnector.Context;

            // Perform a scan operation to retrieve the Property items
            var properties = await context.ScanAsync<Property>(new List<ScanCondition>()).GetRemainingAsync();

            // Extract the Address attribute from each Property item and filter by city
            var addressesInCity = properties
                .Where(property => property.PropertyAddresses != null)
                .Select(property => property.PropertyAddresses)
                .Where(address => address.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return addressesInCity;
        }



        public async Task<IEnumerable<Property>> GetPropertiesByCityAsync(string city)
        {
            DynamoDBContext context = _awsConnector.Context;

            // Perform a scan operation to retrieve the Property items
            var allProperties = await context.ScanAsync<Property>(new List<ScanCondition>()).GetRemainingAsync();

            // Filter properties by city
            var propertiesInCity = allProperties
                .Where(property => property.PropertyAddresses != null &&
                                   property.PropertyAddresses.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return propertiesInCity;
        }


        public async Task<IEnumerable<Property>> GetPropertiesByAreaAsync(string areaCode)
        {
            DynamoDBContext context = _awsConnector.Context;

            // Perform a scan operation to retrieve the Property items
            var allProperties = await context.ScanAsync<Property>(new List<ScanCondition>()).GetRemainingAsync();

            // Filter properties by areacode
            var propertiesInAreaCode = allProperties
                .Where(property => property.PropertyAddresses != null &&
                                   property.PropertyAddresses.AreaCode.Equals(areaCode, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return propertiesInAreaCode;
        }


        public async Task<bool> UpdateAddress(string propertyId, Address updatedAddress)
        {
            try
            {
                DynamoDBContext context = _awsConnector.Context;

                Property property = await context.LoadAsync<Property>(propertyId);
                if (property == null) return false;

                // Check if there are addresses to update
                if (property.PropertyAddresses == null)
                {
                    property.PropertyAddresses = new Address();
                    property.PropertyAddresses.AreaCode = updatedAddress.PostalCode?.Substring(0, 3);
                }
                else
                {
                   
                   property.PropertyAddresses = updatedAddress;
                   property.PropertyAddresses.AreaCode = updatedAddress.PostalCode?.Substring(0, 3);

                }
                await context.SaveAsync(property);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while updating address: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddAddressToPropertyAsync(string propertyId, AddressDto addressDto)
        {
            DynamoDBContext context = _awsConnector.Context;
            var property = await context.LoadAsync<Property>(propertyId);
            if (property == null) return false;

            if (property.PropertyAddresses == null)
            {
                property.PropertyAddresses = new Address();
            }

            var newAddress= new Address
            {
                StreetAddress = addressDto.StreetAddress,
                City = addressDto.City,
                State =addressDto.State,
                PostalCode= addressDto.PostalCode,
                Country = addressDto.Country,
                AreaCode = addressDto.PostalCode?.Substring(0, 3)
        };

            property.PropertyAddresses = newAddress;

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

        public async Task<bool> DeleteAddressFromPropertyAsync(string propertyId)
        {
            DynamoDBContext context = _awsConnector.Context;

            // Retrieve the property by PropertyId
            var property = await context.LoadAsync<Property>(propertyId);
            if (property == null)
            {
                return false; // Property not found
            }

            // Clear the address list
            property.PropertyAddresses = null;

            try
            {
                // Save the updated property
                await context.SaveAsync(property);
                return true; // Deletion of all addresses successful
            }
            catch (Exception ex)
            {
                return false; // Deletion failed
            }
        }


    }
}
