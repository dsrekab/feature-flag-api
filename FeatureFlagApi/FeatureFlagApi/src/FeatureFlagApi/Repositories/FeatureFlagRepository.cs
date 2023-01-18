using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
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
        private const string _featureFlagTableName = "FeatureFlags";

        public FeatureFlagRepository(ISecretManagerService secretManagerService, IConfiguration configuration)
        {
            _secretManagerService = secretManagerService;
            _configuration= configuration;
        }

        public async Task<FeatureFlag> CreateFeatureFlag(FeatureFlag newFeatureFlag)
        {
            await InitDbContext();

            var request = BuildPutRequest(newFeatureFlag);

            await _dynamoDbClient.PutItemAsync(request);

            var createdFlag =  (await GetFeatureFlags(newFeatureFlag.ServiceName, newFeatureFlag.FlagName)).FirstOrDefault(); 

            if (createdFlag == null)
            {
                //DynamoDb is Eventually Consistent for this project, so if the read doesn't work, just return the requested flag
                return newFeatureFlag;
            }

            return createdFlag;
        }

        public async Task<List<FeatureFlag>> GetFeatureFlags(string? serviceName, string? flagName)
        {
            await InitDbContext();
            var request = BuildGetRequest(serviceName, flagName);

            var result = await _dynamoDbClient.ScanAsync(request);

            return result.Items.Select(Map).ToList();
        }

        private FeatureFlag Map(Dictionary<string, AttributeValue> valuesToMap)
        {
            return new FeatureFlag
            {
                FeatureFlagId = Guid.Parse(valuesToMap["FeatureFlagId"].S),
                ServiceName = valuesToMap["ServiceName"].S,
                FlagName = valuesToMap["FlagName"].S,
                Enabled = valuesToMap["Enabled"].BOOL,
                LastUpdated = DateTimeOffset.Parse(valuesToMap["LastUpdated"].S)
            };
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

        private PutItemRequest BuildPutRequest(FeatureFlag putFeatureFlag)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                {"FeatureFlagId", new AttributeValue { S = Guid.NewGuid().ToString()} },
                {"ServiceName", new AttributeValue{ S = putFeatureFlag.ServiceName} },
                {"FlagName", new AttributeValue{ S = putFeatureFlag.FlagName} },
                {"Enabled", new AttributeValue{BOOL = putFeatureFlag.Enabled??false } },
                {"LastUpdated", new AttributeValue{S = DateTimeOffset.UtcNow.ToString() } }
            };

            return new PutItemRequest
            {
                TableName = _featureFlagTableName,
                Item = item
            };
        }

        private ScanRequest BuildGetRequest (string? serviceName, string? flagName)
        {
            var retVal = new ScanRequest
            {
                TableName = _featureFlagTableName,
                ProjectionExpression = "FeatureFlagId, ServiceName, FlagName, Enabled, LastUpdated"
            };

            if (!string.IsNullOrEmpty(serviceName))
            {
                if (!string.IsNullOrEmpty(flagName))
                {
                    //Filter on serviceName and FlagName
                    retVal.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {":v_serviceName", new AttributeValue { S = serviceName} },
                        {":v_flagName", new AttributeValue { S = flagName} },
                    };

                    retVal.FilterExpression = "ServiceName = :v_serviceName AND FlagName = :v_flagName";
                    
                }
                else
                {
                    //Filter on just ServiceName
                    retVal.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":v_serviceName", new AttributeValue { S = serviceName} }
                };

                    retVal.FilterExpression = "ServiceName = :v_serviceName";
                }
            }


            return retVal;
        }
    }
}
