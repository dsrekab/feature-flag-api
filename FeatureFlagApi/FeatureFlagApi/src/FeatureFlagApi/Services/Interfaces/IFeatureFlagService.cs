using FeatureFlagApi.Models;

namespace FeatureFlagApi.Services.Interfaces
{
    public interface IFeatureFlagService
    {
        Task<FeatureFlag> CreateFeatureFlag(FeatureFlag featureFlag);
        Task<List<FeatureFlagRepoItem>> GetFeatureFlags(string? serviceName, string? flagName);
        Task<bool> FeatureIsEnabled(string serviceName, string flagName);
    }
}
