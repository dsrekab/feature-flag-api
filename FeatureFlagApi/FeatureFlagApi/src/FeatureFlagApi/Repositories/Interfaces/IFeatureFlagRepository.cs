using FeatureFlagApi.Models;

namespace FeatureFlagApi.Repositories.Interfaces
{
    public interface IFeatureFlagRepository
    {
        Task<FeatureFlagRepoItem> CreateFeatureFlag(FeatureFlag newFeatureFlag);
        Task<List<FeatureFlagRepoItem>> GetFeatureFlags(string? serviceName, string? flagName);
    }
}
