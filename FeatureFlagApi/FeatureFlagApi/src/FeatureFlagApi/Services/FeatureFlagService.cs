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

        public async Task<bool> FeatureIsEnabled(string serviceName, string flagName)
        {
            var flag = await _featureFlagRepository.GetFeatureFlag(serviceName, flagName);

            return flag?.Enabled == true;
        }

        public async Task<List<FeatureFlag>> GetAllFeatureFlagsByService(string serviceName)
        {
            var serviceFlags = await _featureFlagRepository.GetAllFeatureFlagsByService(serviceName);
            var retVal = new List<FeatureFlag>();

            serviceFlags?.ForEach(f => retVal.Add(new FeatureFlag { ServiceName = f.ServiceName, FlagName = f.FlagName, Enabled = f.Enabled }));

            return retVal;
        }

        public async Task<FeatureFlag> GetFeatureFlag(string serviceName, string flagName)
        {
            var featureFlagRepoItem = await _featureFlagRepository.GetFeatureFlag(serviceName, flagName);
            
            return new FeatureFlag
            {
                ServiceName = featureFlagRepoItem?.ServiceName ?? serviceName,
                FlagName = featureFlagRepoItem?.FlagName ?? flagName,
                Enabled = featureFlagRepoItem?.Enabled ?? false
            };
        }
    }
}
