using Agency.Infrastructure.Agents;
using Application.Interfaces;

namespace Agency.Infrastructure.Agents
{
    public class ProductManagerAgent : LLMAgentBase
    {
        protected override string SystemPrompt =>
            "Tu es un Product Manager dans une agence web. " +
            "Analyse les besoins, clarifie les objectifs du projet et formule des tickets clairs pour les d�veloppeurs.";

        public ProductManagerAgent(IHttpClientFactory factory)
            : base(factory, "pm", "ProductManager") { }
    }
}

