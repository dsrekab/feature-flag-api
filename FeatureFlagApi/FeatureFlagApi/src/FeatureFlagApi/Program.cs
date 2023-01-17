using FeatureFlagApi;
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Hello World");
app.MapGet("/getallfeatureflagsbyservice", (string serviceName) => featureFlagService?.GetAllFeatureFlagsByService(serviceName));
app.MapGet("/getfeatureflag", (string serviceName, string flagName) => featureFlagService?.GetFeatureFlag(serviceName, flagName));
app.MapGet("/isenabled", (string serviceName, string flagName) => featureFlagService?.FeatureIsEnabled(serviceName, flagName));

app.Run();
