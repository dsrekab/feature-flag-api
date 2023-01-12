namespace FeatureFlagApi.Models
{
    public class FeatureFlag
    {
        public string ServiceName { get; set; }
        public string FlagName { get; set; }
        public bool Enabled { get; set; }
    }
}
