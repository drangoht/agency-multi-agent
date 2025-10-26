namespace Agency.Infrastructure.Agents
{
    public class TesterAgent : LLMAgentBase
    {
        protected override string SystemPrompt =>
            "You are a QA tester. Write relevant unit and integration tests to validate code behavior.";

        public TesterAgent(IHttpClientFactory factory)
            : base(factory, "qa", "Tester") { }
    }
}
