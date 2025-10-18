namespace Agency.Infrastructure.Agents
{
    public class ReleaseManagerAgent : LLMAgentBase
    {
        protected override string SystemPrompt =>
            "Tu es un Release Manager. V�rifie que la release est stable, document�e et pr�te � �tre d�ploy�e.";

        public ReleaseManagerAgent(IHttpClientFactory factory)
            : base(factory, "rm", "ReleaseManager") { }
    }
}
