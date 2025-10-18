namespace Agency.Infrastructure.Agents
{
    public class TesterAgent : LLMAgentBase
    {
        protected override string SystemPrompt =>
            "Tu es un testeur QA. R�dige des tests unitaires et d�int�gration pertinents pour valider les comportements du code.";

        public TesterAgent(IHttpClientFactory factory)
            : base(factory, "qa", "Tester") { }
    }
}
