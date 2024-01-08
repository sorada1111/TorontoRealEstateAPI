using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Amazon.S3;
using System;
using System.IO;

namespace RealEstateAPI.Connector
{
    public class AWSConnector
    {
        public IAmazonS3 S3Client { get; }
        public IAmazonDynamoDB DynamoClient { get; }
        public DynamoDBContext Context { get; }

        public AWSConnector()
        {
            if (IsRunningLocally())
            {
                // Load AWS credentials from a configuration file or environment variables
                var credentials = FetchAWSCredentialsLocally();
                S3Client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
                DynamoClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
                Context = new DynamoDBContext(DynamoClient);
            }
            else
            {
                // Use instance profile credentials for running on AWS resources
                S3Client = new AmazonS3Client(RegionEndpoint.CACentral1);
                DynamoClient = new AmazonDynamoDBClient(RegionEndpoint.CACentral1);
                Context = new DynamoDBContext(DynamoClient);
            }
        }

        private BasicAWSCredentials FetchAWSCredentialsLocally()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            string awsAccessKey = builder.Build().GetSection("AWS").GetSection("AccessKeyId").Value;
            string awsSecretKey = builder.Build().GetSection("AWS").GetSection("SecretAccessKey").Value;

            return new BasicAWSCredentials(awsAccessKey, awsSecretKey);
        }

        private bool IsRunningLocally()
        {
            // Implement logic to determine if the code is running locally
            // For example, check for environment variables or configuration settings
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        }

        // Load content of a DynamoDb table.
        public Table LoadContentTable(string tableName)
        {
            return Table.LoadTable(DynamoClient, tableName);
        }
    }
}
