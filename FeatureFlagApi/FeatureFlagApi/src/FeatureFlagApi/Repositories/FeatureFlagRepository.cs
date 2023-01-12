using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using FeatureFlagApi.Models;
using FeatureFlagApi.Repositories.Interfaces;

namespace FeatureFlagApi.Repositories
{
    public class FeatureFlagRepository : IFeatureFlagRepository
    {
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private readonly DynamoDBContext _dynamoDbContext;

        public FeatureFlagRepository()
        {
            var clientConfig = new AmazonDynamoDBConfig();
            _dynamoDbClient = new AmazonDynamoDBClient("access", "secret", clientConfig);
            _dynamoDbContext = new DynamoDBContext(_dynamoDbClient);
        }

        public async Task<FeatureFlagRepoItem?> GetFeatureFlag(string serviceName, string flagName)
        {
            var search = _dynamoDbContext.ScanAsync<FeatureFlagRepoItem>
              (
                new[] {
                    new ScanCondition
                      (
                        nameof(FeatureFlagRepoItem.ServiceName),
                        ScanOperator.Equal,
                        serviceName.ToLower()
                      ),
                    new ScanCondition
                      (
                        nameof(FeatureFlagRepoItem.FlagName),
                        ScanOperator.Equal,
                        flagName.ToLower()
                      )
                }
              );

            var result = await search.GetRemainingAsync();
            return result?.FirstOrDefault();
        }
    }
}
