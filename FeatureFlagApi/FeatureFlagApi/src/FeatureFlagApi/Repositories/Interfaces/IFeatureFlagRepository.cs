using FeatureFlagApi.Models;

namespace FeatureFlagApi.Repositories.Interfaces
{
    public interface IFeatureFlagRepository
    {
        public Task<FeatureFlagRepoItem> GetFeatureFlag(string serviceName, string flagName);
    }
}
