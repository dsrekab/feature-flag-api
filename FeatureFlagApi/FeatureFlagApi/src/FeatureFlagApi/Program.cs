using FeatureFlagApi;
using FeatureFlagApi.Exceptions;
using FeatureFlagApi.Models;
using FeatureFlagApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Logging.AddAWSProvider();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddFeatureFlagServices();

var app = builder.Build();

var featureFlagService = app.Services.GetService<IFeatureFlagService>();

if (featureFlagService==null)
{
    throw new FeatureFlagException("Unable to find IFeatureFlagService");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Use CreateFeatureFlag, GetFeatureFlags, or IsEnabled");

app.MapPost("/CreateFeatureFlag", async (FeatureFlag newFeatureFlag) => await featureFlagService.CreateFeatureFlag(newFeatureFlag));
app.MapGet("/GetFeatureFlags", async (string? serviceName, string? flagName) => await featureFlagService.GetFeatureFlags(serviceName, flagName));
app.MapGet("/isenabled", async (string serviceName, string flagName) => await featureFlagService.FeatureIsEnabled(serviceName, flagName));

app.Run();
