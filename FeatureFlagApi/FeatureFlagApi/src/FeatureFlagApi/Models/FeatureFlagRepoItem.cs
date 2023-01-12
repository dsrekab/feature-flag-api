using Amazon.DynamoDBv2.DataModel;

namespace FeatureFlagApi.Models
{
    [DynamoDBTable("FeatureFlags")]
    public class FeatureFlagRepoItem
    {
        [DynamoDBHashKey]
        private Guid FeatureFlagId { get; set; }
        public string ServiceName { get; set; }
        public string FlagName { get; set; }
        public bool Enabled { get; set; }
        public string LastUpdated { get; set; }
    }
}
