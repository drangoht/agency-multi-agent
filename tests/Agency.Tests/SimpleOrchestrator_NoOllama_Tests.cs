using System.Threading;
using Agency.Application.Interfaces;
using Agency.Domain.Models;
using Agency.Infrastructure.Orchestrator;
using Xunit;

namespace Agency.Tests
{
    public class SimpleOrchestrator_NoOllama_Tests
    {
        private class StubAgent : IAgent
        {
            public AgentDescriptor Descriptor { get; }
            private readonly string _content;

            public StubAgent(string id, string role, string content)
            {
                Descriptor = new AgentDescriptor(id, role);
                _content = content;
            }

            public Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default)
            {
                var content = _content;
                if (!string.IsNullOrWhiteSpace(instruction)) content += " - " + instruction;
                var msg = new AgentMessage(Descriptor.Id, Descriptor.Role, content, DateTime.UtcNow);
                return Task.FromResult<AgentMessage?>(msg);
            }
        }

        [Fact]
        public async Task Orchestrator_Works_Without_OllamaGuy()
        {
            var store = new Application.Services.InMemoryConversationStore();

            // Create only PM, Dev, Tester, ReleaseManager — no OllamaGuy
            var agents = new IAgent[]
            {
                new StubAgent("pm", "ProductManager", "pm initial"),
                new StubAgent("dev", "Developer", "dev response"),
                new StubAgent("tester", "Tester", "tests produced"),
                new StubAgent("rm", "ReleaseManager", "release notes")
            };

            var orchestrator = new SimpleOrchestrator(agents, store);

            await orchestrator.StartConversationAsync("Initial prompt: hello");

            var conv = orchestrator.GetConversation().ToList();

            Assert.NotEmpty(conv);
            Assert.Contains(conv, m => m.From == "pm");
            Assert.Contains(conv, m => m.From == "dev");
        }
    }
}
