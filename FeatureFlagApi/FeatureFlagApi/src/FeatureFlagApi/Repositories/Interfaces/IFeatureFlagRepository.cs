using FeatureFlagApi.Models;

namespace FeatureFlagApi.Repositories.Interfaces
{
    public interface IFeatureFlagRepository
    {
        public FeatureFlag GetFeatureFlag(string serviceName, string flagName);
    }
}
