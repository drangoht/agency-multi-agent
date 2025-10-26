using Agency.Application.Interfaces;
using Application.Interfaces;
using Agency.Domain.Models;
using System.Text;

namespace Agency.Infrastructure.Agents
{
    public class DeveloperAgent : AgentBase
    {
        private readonly IOllamaClient _ollama;

        public DeveloperAgent(IOllamaClient ollama)
            : base("dev", "Developer")
        {
            _ollama = ollama;
        }

        public override async Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(instruction))
            {
                sb.AppendLine(instruction);
            }

            var prompt = sb.ToString();
            if (string.IsNullOrWhiteSpace(prompt)) prompt = "Implement the requested functionality.";

            var result = await _ollama.GenerateAsync(prompt);

            return new AgentMessage(Descriptor.Id, Descriptor.Role, result ?? string.Empty, DateTime.UtcNow);
        }
    }
}
