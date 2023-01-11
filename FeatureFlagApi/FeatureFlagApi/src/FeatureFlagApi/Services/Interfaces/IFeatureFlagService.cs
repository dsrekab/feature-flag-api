using FeatureFlagApi.Models;

namespace FeatureFlagApi.Services.Interfaces
{
    public interface IFeatureFlagService
    {
        FeatureFlag GetFeatureFlag(string serviceName, string flagName);
        bool FeatureIsEnabled(string serviceName, string flagName);
    }
}
