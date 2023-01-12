using FeatureFlagApi.Models;
using FeatureFlagApi.Repositories.Interfaces;

namespace FeatureFlagApi.Repositories
{
    public class FeatureFlagRepository : IFeatureFlagRepository
    {
        public FeatureFlag GetFeatureFlag(string serviceName, string flagName)
        {
            return new FeatureFlag
            {
                ServiceName = serviceName,
                FlagName = flagName,
                Enabled = true
            };
        }
    }
}
