using FeatureFlagApi.Models;

namespace FeatureFlagApi.Services.Interfaces
{
    public interface IFeatureFlagService
    {
        Task<FeatureFlag> GetFeatureFlag(string serviceName, string flagName);
        Task<bool> FeatureIsEnabled(string serviceName, string flagName);
    }
}
