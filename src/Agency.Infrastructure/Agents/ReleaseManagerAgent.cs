namespace Agency.Infrastructure.Agents
{
    public class ReleaseManagerAgent : LLMAgentBase
    {
        protected override string SystemPrompt =>
            "You are a Release Manager. Verify that the release is stable, documented and ready to be deployed.";

        public ReleaseManagerAgent(IHttpClientFactory factory)
            : base(factory, "rm", "ReleaseManager") { }
    }
}
