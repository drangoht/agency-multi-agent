namespace Agency.Infrastructure.Agents
{
    public class ReleaseManagerAgent : LLMAgentBase
    {
        protected override string SystemPrompt =>
            "Tu es un Release Manager. Vérifie que la release est stable, documentée et prête à être déployée.";

        public ReleaseManagerAgent(IHttpClientFactory factory)
            : base(factory, "rm", "ReleaseManager") { }
    }
}
