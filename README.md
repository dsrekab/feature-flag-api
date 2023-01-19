# feature-flag-api
Feature Flag Api in .net for AWS Lambda

## Description
Feature Flag Api is used to check and set Feature Flags to allow production releases with features turned off as well as quick rollback of erroring releases.

Flags are stored in DynamoDB and API is released with AWS CloudFormation templates to the FeatureFlagApiStack stack in AWS.

##Usage
To Add a Feature Flags, call /CreateFeatureFlag POST endpoint with the ServiceName, FlagName, and Enabled Value in JSON

```json
{
    "ServiceName": "VerificationService",
    "FlagName": "allowVerification",
    "Enabled": false
}
```

To Get Feature Flags, call /GetFeatureFlags GET endpoint with the following parameter options:

| GET Endpoint                                     | Result                                                  |
|--------------------------------------------------|---------------------------------------------------------|
| /GetFeatureFlags                                 | All Feature Flags                                       |
| /GetFeatureFlag?serviceName=[foo]                | All Feature Flags for service [Foo], useful for caching |
| /GetFeatureFlag?serviceName=[foo]&flagName=[bar] | The [Bar] feature flag for service [Foo]                |


If a feature flag is requested but not found, the result will be a feature flag object with the same serviceName and flagName and an Enabled value of False.


##RoadMap
Need to create a UI to allow creating, updating, and deleting flags without needing to create the requests manually