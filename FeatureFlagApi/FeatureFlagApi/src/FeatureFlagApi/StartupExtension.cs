using FeatureFlagApi.Repositories;
using FeatureFlagApi.Repositories.Interfaces;
using FeatureFlagApi.Services;
using FeatureFlagApi.Services.Interfaces;

namespace FeatureFlagApi
{
    public static class StartupExtension
    {
        public static void AddFeatureFlagServices(this IServiceCollection services)
        {
            services.AddTransient<IFeatureFlagService, FeatureFlagService>();            
            services.AddTransient<ISecretManagerService, SecretManagerService>();

            services.AddTransient<IFeatureFlagRepository, FeatureFlagRepository>();
        }
    }
}
