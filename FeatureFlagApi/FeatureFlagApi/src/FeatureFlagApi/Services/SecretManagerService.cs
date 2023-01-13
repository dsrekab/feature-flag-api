using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using FeatureFlagApi.Exceptions;
using FeatureFlagApi.Models;
using FeatureFlagApi.Services.Interfaces;
using Newtonsoft.Json;

namespace FeatureFlagApi.Services
{
    public class SecretManagerService : ISecretManagerService
    {
        private List<SecretKeyValues> _secretKeyValues = new List<SecretKeyValues>();
        private readonly IConfiguration _configuration;
        private readonly ILogger<ISecretManagerService> _logger;

        public SecretManagerService(IConfiguration configuration, ILogger<ISecretManagerService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetSecret(string secretName)
        {
            string region = "us-east-1";

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT"
            };

            var response = await client.GetSecretValueAsync(request);

            return response.SecretString;

        }

        public async Task<string> GetSecretKeyValue(string region, string secretName, string key)
        {
            _logger.LogWarning("This is a test of log for {SecretName}", secretName);

            var retVal = _secretKeyValues.Where(s=>s.SecretName.ToLower() == secretName.ToLower()).FirstOrDefault()?.SecretKeyValuePairs[key];

            if (retVal == null)
            {
                //attempt to reload
                await CacheSecretKeys(region, secretName);

                //see if there is a value now
                retVal = _secretKeyValues.Where(s => s.SecretName.ToLower() == secretName.ToLower()).FirstOrDefault()?.SecretKeyValuePairs[key];

                if (retVal == null)
                {
                    throw new MissingSecretException($"Unable to find Secret {key}");
                }
            }

            return retVal;
        }

        public async Task CacheSecretKeys(string region, string secretName)
        {
            var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            var request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT"
            };

            try
            {
                var response = await client.GetSecretValueAsync(request);

                //remove the existing secret to reload it
                var existingSecret = _secretKeyValues.Where(k => k.SecretName.ToLower() == secretName.ToLower()).FirstOrDefault();

                if (existingSecret != null)
                {
                    _secretKeyValues.Remove(existingSecret);
                }

                //create the new secret's container
                _secretKeyValues.Add(
                    new SecretKeyValues
                    {
                        SecretName = secretName,
                        SecretKeyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.SecretString)
                    });
            }
            catch (Exception ex)
            {
                //log error
                throw;
            }
        }
    }
}
