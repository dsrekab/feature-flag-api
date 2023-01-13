namespace FeatureFlagApi.Models
{
    public class SecretKeyValues
    {
        public string SecretName { get; set; }
        public Dictionary<string, string> SecretKeyValuePairs { get; set; } = new Dictionary<string, string>();
    }
}
