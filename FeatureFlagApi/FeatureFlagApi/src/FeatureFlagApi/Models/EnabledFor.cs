namespace FeatureFlagApi.Models
{
    public class EnabledFor
    {
        public bool? All { get; set; }
        public Rollout? Rollout { get; set; }
    }
}
