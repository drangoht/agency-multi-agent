namespace Agency.Infrastructure.Agents
{
    public class TesterAgent : LLMAgentBase
    {
        protected override string SystemPrompt =>
            "Tu es un testeur QA. Rédige des tests unitaires et d’intégration pertinents pour valider les comportements du code.";

        public TesterAgent(IHttpClientFactory factory)
            : base(factory, "qa", "Tester") { }
    }
}
