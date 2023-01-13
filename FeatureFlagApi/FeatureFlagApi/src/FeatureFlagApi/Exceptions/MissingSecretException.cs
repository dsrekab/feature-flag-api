namespace FeatureFlagApi.Exceptions
{
    [Serializable]
    public class MissingSecretException : Exception
    {
        public MissingSecretException()
        {
        }

        public MissingSecretException(string message): base(message)
        {
        }

        public MissingSecretException(string message, Exception inner): base(message, inner)
        {
        }
    }
}
