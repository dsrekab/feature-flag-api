using FeatureFlagApi.Models;
using FeatureFlagApi.Repositories.Interfaces;
using FeatureFlagApi.Services.Interfaces;

namespace FeatureFlagApi.Services
{
    public class FeatureFlagService : IFeatureFlagService
    {
        private readonly IFeatureFlagRepository _featureFlagRepository;

        public FeatureFlagService(IFeatureFlagRepository featureFlagRepository)
        {
            _featureFlagRepository=featureFlagRepository;
        }

        public async Task<FeatureFlag> CreateFeatureFlag(FeatureFlag featureFlag)
        {
            var existingFlag = (await _featureFlagRepository.GetFeatureFlags(featureFlag.ServiceName, featureFlag.FlagName)).FirstOrDefault();

            if (existingFlag?.FlagName!=null)
            {
                throw new Exception($"FeatureFlag {featureFlag.FlagName} in Service {featureFlag.ServiceName} already exists");
            }

            var newFlag = await _featureFlagRepository.CreateFeatureFlag(featureFlag);

            if (string.IsNullOrEmpty(newFlag?.FlagName))
            {
                throw new Exception($"Unable to create Feature Flag {featureFlag.FlagName} in Service {featureFlag.ServiceName}");
            }

            return new FeatureFlag
            {
                ServiceName = newFlag.ServiceName,
                FlagName = newFlag.FlagName,
                Enabled = newFlag.Enabled
            };
        }

        public async Task<List<FeatureFlagRepoItem>> GetFeatureFlags(string? serviceName, string? flagName)
        {
            var flags = await _featureFlagRepository.GetFeatureFlags(serviceName, flagName);

            if (!flags.Any())
            {
                if (serviceName!=null && flagName !=null)
                {
                    return new List<FeatureFlagRepoItem>
                    {
                        new FeatureFlagRepoItem
                        {
                            ServiceName=serviceName,
                            FlagName=flagName,
                            Enabled=false
                        }
                    };
                }
            }

            return flags;
        }


        public async Task<bool> FeatureIsEnabled(string serviceName, string flagName)
        {
            var flags = await GetFeatureFlags(serviceName, flagName);
            
            return flags.FirstOrDefault()?.Enabled == true;
        }

        private FeatureFlag GetFeatureFlag(FeatureFlagRepoItem repoItem)
            => new FeatureFlag
            {
                ServiceName = repoItem.ServiceName,
                FlagName = repoItem.FlagName,
                Enabled = repoItem.Enabled
            };
    }
}
