using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateLibrary.Connector
{
    public class AWSConnector
    {
        public IAmazonS3 S3Client { get; }
        public IAmazonDynamoDB DynamoClient { get; }
        public DynamoDBContext Context { get; }


        public AWSConnector()
        {
            var credentials = FetchAWSCredentials();
            S3Client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            DynamoClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
            Context = new DynamoDBContext(DynamoClient);
            S3Client = new AmazonS3Client(RegionEndpoint.CACentral1);
            DynamoClient = new AmazonDynamoDBClient(RegionEndpoint.CACentral1);
            Context = new DynamoDBContext(DynamoClient);
        }
        private BasicAWSCredentials FetchAWSCredentials()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            string awsAccessKey = builder.Build().GetSection("AWS").GetSection("AccessKeyId").Value;
            string awsSecretKey = builder.Build().GetSection("AWS").GetSection("SecretAccessKey").Value;

            return new BasicAWSCredentials(awsAccessKey, awsSecretKey);
        }


        //Load content of a DynamoDb table.
        public Table LoadContentTable(string tableName)
        {
            return Table.LoadTable(DynamoClient, tableName);
        }
    }
}
