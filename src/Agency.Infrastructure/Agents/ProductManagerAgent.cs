using Agency.Infrastructure.Agents;
using Application.Interfaces;

namespace Agency.Infrastructure.Agents
{
    public class ProductManagerAgent : LLMAgentBase
    {
        protected override string SystemPrompt =>
            "You are a Product Manager in a web agency. " +
            "Analyze requirements, clarify project objectives and formulate clear tickets for developers.";

        public ProductManagerAgent(IHttpClientFactory factory)
            : base(factory, "pm", "ProductManager") { }
    }
}

