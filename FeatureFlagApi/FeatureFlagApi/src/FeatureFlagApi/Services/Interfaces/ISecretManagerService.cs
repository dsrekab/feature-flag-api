namespace FeatureFlagApi.Services.Interfaces
{
    public interface ISecretManagerService
    {
        Task<string> GetSecret(string secretName);
        Task<string> GetSecretKeyValue(string region, string secretName, string key);
        Task CacheSecretKeys(string region, string secretName);
    }
}
