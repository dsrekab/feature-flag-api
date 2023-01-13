using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using FeatureFlagApi.Models;
using FeatureFlagApi.Repositories.Interfaces;
using FeatureFlagApi.Services.Interfaces;

namespace FeatureFlagApi.Repositories
{
    public class FeatureFlagRepository : IFeatureFlagRepository
    {
        private AmazonDynamoDBClient? _dynamoDbClient;
        private DynamoDBContext? _dynamoDbContext;
        private readonly ISecretManagerService _secretManagerService;
        private readonly IConfiguration _configuration;
                
        public FeatureFlagRepository(ISecretManagerService secretManagerService, IConfiguration configuration)
        {
            _secretManagerService = secretManagerService;
            _configuration= configuration;
        }

        public async Task<List<FeatureFlagRepoItem>> GetAllFeatureFlagsByService(string serviceName)
        {
            await InitDbContext();

            var search = _dynamoDbContext.ScanAsync<FeatureFlagRepoItem>
              (
                new[] {
                    new ScanCondition
                      (
                        nameof(FeatureFlagRepoItem.ServiceName),
                        ScanOperator.Equal,
                        serviceName.ToLower()
                      )
                }
              );

            var result = await search.GetRemainingAsync();
            return result;
        }

        public async Task<FeatureFlagRepoItem?> GetFeatureFlag(string serviceName, string flagName)
        {
            await InitDbContext();

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

        private async Task InitDbContext()
        {
            if (_dynamoDbContext==null)
            {
                var secretKeyName = _configuration["SecretManagerKeys:AccessKeySecretName"];
                var region = _configuration["SecretManagerKeys:AccessKeyRegion"];

                var accessKey = await _secretManagerService.GetSecretKeyValue(region, secretKeyName, "accesskey");
                var secretKey = await _secretManagerService.GetSecretKeyValue(region, secretKeyName, "secretkey");

                var clientConfig = new AmazonDynamoDBConfig();
                _dynamoDbClient = new AmazonDynamoDBClient(accessKey, secretKey, clientConfig);
                _dynamoDbContext = new DynamoDBContext(_dynamoDbClient);
            }
        }
    }
}
