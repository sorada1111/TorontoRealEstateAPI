using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3.Transfer;
using Amazon.S3;
using RealEstateAPI.Connector;
using RealEstateLibrary.Models;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;

namespace RealEstateAPI.Services
{
    public class RealEstateRespository : IRealEstateRespository
    {
        private readonly AWSConnector _awsConnector;
        private readonly string _tableName = "Property";
        private readonly Table _table;

        public RealEstateRespository(AWSConnector awsConnector)
        {
            _awsConnector = awsConnector;
            _table = _awsConnector.LoadContentTable(_tableName);
        }
        public async Task<IEnumerable<Property>> GetPropertiesAsync()
        {
            DynamoDBContext context = _awsConnector.Context;
            var properties = await context.ScanAsync<Property>(new List<ScanCondition>()).GetRemainingAsync();
            return properties;
        }

        public async Task<Property> GetPropertyByIdAsync(string propertyId)
        {
            DynamoDBContext context = _awsConnector.Context;
            var property = await context.LoadAsync<Property>(propertyId);
            return property;
        }
        public async Task<Property> GetPropertyByNameAsync(string propertyName)
        {
            DynamoDBContext context = _awsConnector.Context;
            var conditions = new List<ScanCondition>
            {
                new ScanCondition("PropertyName", ScanOperator.Equal, propertyName)
            };
            var search = context.ScanAsync<Property>(conditions);
            var properties = await search.GetRemainingAsync();      
            return properties.FirstOrDefault();
        }

        public async Task<IEnumerable<Property>> GetPropertyByTypeAsync(string propertyType) 
        {
            DynamoDBContext context = _awsConnector.Context;
            var conditions = new List<ScanCondition>
                {
                    new ScanCondition("PropertyType", ScanOperator.Equal, propertyType)
                };

            var searchResults = await context.ScanAsync<Property>(conditions, new DynamoDBOperationConfig
            {
                IndexName = "PropertyType-index", // GSI name 
            }).GetRemainingAsync();

            return searchResults.ToList();
        }

        public async Task<IEnumerable<Property>> GetPropertyByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            DynamoDBContext context = _awsConnector.Context;
            var conditions = new List<ScanCondition>
                {
                    new ScanCondition("Price", ScanOperator.Between, minPrice, maxPrice)
                };

            var searchResults = await context.ScanAsync<Property>(conditions, new DynamoDBOperationConfig
            {
                IndexName = "Price-index", // GSI name
            }).GetRemainingAsync();

            return searchResults.ToList();
        }

    

        public async Task<IEnumerable<Property>> GetPropertyByStatusAsync(string status)
        {
            DynamoDBContext context = _awsConnector.Context;
            var conditions = new List<ScanCondition>
            {
                new ScanCondition("Status", ScanOperator.Equal, status)
            };
            var search = context.ScanAsync<Property>(conditions);
            var properties = await search.GetRemainingAsync();
            return properties;
        }

        public async Task<Property> AddProperty(Property property)
        {
            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("yyyy-MM-dd");
         
            property.PropertyId = Guid.NewGuid().ToString("N");
            property.LastUpdate = formattedDate;
            property.DateListed = formattedDate;

            var document = new Document();
            document["PropertyId"] = property.PropertyId;
            document["PropertyName"] = property.PropertyName;
            document["PropertyDesc"] = property.PropertyDesc;
            document["PropertyTax"] = property.PropertyTax;
            document["LastUpdate"] = property.LastUpdate;
            document["DateListed"] = property.DateListed;
            document["Price"] = property.Price;
            document["Status"] = property.Status;
            document["PropertyType"] = property.PropertyType;

            await _table.PutItemAsync(document);
            return property;

        }

        //update
        public async Task<bool> UpdateProperty(Property property)
        {
            try
            {
                DynamoDBContext context = _awsConnector.Context;
                await context.SaveAsync(property);

                return true; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while updating property: {ex.Message}");
                return false;
            }
        }

        //Delete Property
        public async Task<bool> DeletePropertyAsync(string propertyId)
        {
            DynamoDBContext context = _awsConnector.Context;
            try
            {               
                await context.DeleteAsync<Property>(propertyId);
                return true; // Deletion successful
            }
            catch (AmazonDynamoDBException dbEx)
            {
                return false; // Deletion failed due to a DynamoDB exception
                throw new Exception($"DynamoDB error: {dbEx.ErrorCode} - {dbEx.Message}", dbEx);         
            }
            catch (Exception ex)
            {
                return false; // Deletion failed due to a general exception
                throw new Exception($"General error: {ex.Message}", ex);               
            }
        }

    }
}
