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

            if (flag == null || flag.EnabledFor==null)
            {
                return false;
            }
            else
            {
                if (flag.EnabledFor.All==null)
                {
                    //more logic here later
                    return false;
                }
                else
                {
                    return flag.EnabledFor.All.Value;
                }
            }
        }


        public FeatureFlag GetFeatureFlag(string serviceName, string flagName)
            => _featureFlagRepository.GetFeatureFlag(serviceName, flagName);
    }
}
