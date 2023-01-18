using FeatureFlagApi.Models;

namespace FeatureFlagApi.Repositories.Interfaces
{
    public interface IFeatureFlagRepository
    {
        Task<FeatureFlag> CreateFeatureFlag(FeatureFlag newFeatureFlag);
        Task<List<FeatureFlag>> GetFeatureFlags(string? serviceName, string? flagName);
    }
}
