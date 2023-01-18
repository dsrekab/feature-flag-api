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

            try
            {
                return await _featureFlagRepository.CreateFeatureFlag(featureFlag);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create Feature Flag {featureFlag.FlagName} in Service {featureFlag.ServiceName}.", ex);
            }
        }

        public async Task<List<FeatureFlag>> GetFeatureFlags(string? serviceName, string? flagName)
        {
            var flags = await _featureFlagRepository.GetFeatureFlags(serviceName, flagName);

            if (!flags.Any())
            {
                if (serviceName!=null && flagName !=null)
                {
                    return new List<FeatureFlag>
                    {
                        new FeatureFlag
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
    }
}
