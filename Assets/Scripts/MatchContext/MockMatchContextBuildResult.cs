namespace OFIS.MatchContext
{
    public sealed class MockMatchContextBuildResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }
        public MockMatchContext Context { get; }

        private MockMatchContextBuildResult(
            bool success,
            string errorMessage,
            MockMatchContext context)
        {
            Success = success;
            ErrorMessage = errorMessage;
            Context = context;
        }

        public static MockMatchContextBuildResult Failed(string errorMessage)
        {
            return new MockMatchContextBuildResult(false, errorMessage, null);
        }

        public static MockMatchContextBuildResult Completed(MockMatchContext context)
        {
            return new MockMatchContextBuildResult(true, string.Empty, context);
        }
    }
}