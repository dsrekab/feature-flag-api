namespace FeatureFlagApi.Exceptions
{
    [Serializable]
    public class FeatureFlagException: Exception
    {
        public FeatureFlagException()
        {
        }

        public FeatureFlagException(string message) : base(message)
        {
        }

        public FeatureFlagException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
