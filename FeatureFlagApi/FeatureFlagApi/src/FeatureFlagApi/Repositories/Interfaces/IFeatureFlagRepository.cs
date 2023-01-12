using FeatureFlagApi.Models;

namespace FeatureFlagApi.Repositories.Interfaces
{
    public interface IFeatureFlagRepository
    {
        Task<FeatureFlagRepoItem> GetFeatureFlag(string serviceName, string flagName);
        Task<List<FeatureFlagRepoItem>> GetAllFeatureFlagsByService(string serviceName);
    }
}
