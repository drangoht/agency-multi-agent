using Agency.Application.Interfaces;
using Agency.Application.Services;
using Agency.Infrastructure.Agents;
using Agency.Infrastructure.Orchestrator;
using Agency.Domain.Models;
using Xunit;

namespace Agency.Tests;
public class OrchestratorTests
{
    [Fact]
    public async Task Orchestrator_ProducesConversation()
    {
        var store = new InMemoryConversationStore();
        // Use lightweight stub agents for the unit test to avoid requiring external dependencies.
        var agents = new IAgent[]
        {
            new StubAgent("pm", "ProductManager", "pm message"),
            new StubAgent("dev", "Developer", "dev message"),
            new StubAgent("tester", "Tester", "tester message"),
            new StubAgent("rm", "ReleaseManager", "rm message")
        };
        var orchestrator = new SimpleOrchestrator(agents, store);
        await orchestrator.StartConversationAsync("Initial feature: greeting");
        var conv = orchestrator.GetConversation();
        Assert.NotEmpty(conv);
        Assert.Contains(conv, m => m.From == "pm");
        Assert.Contains(conv, m => m.From == "dev");
    }

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
}
