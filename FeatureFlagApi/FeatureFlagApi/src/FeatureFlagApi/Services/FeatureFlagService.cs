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

        public bool FeatureIsEnabled(string serviceName, string flagName)
        {
            var flag = _featureFlagRepository.GetFeatureFlag(serviceName, flagName);

            if (flag == null || flag.Enabled==null)
            {
                return false;
            }
            else
            {
                return flag.Enabled.Value;
            }
        }


        public FeatureFlag GetFeatureFlag(string serviceName, string flagName)
            => _featureFlagRepository.GetFeatureFlag(serviceName, flagName);
    }
}
