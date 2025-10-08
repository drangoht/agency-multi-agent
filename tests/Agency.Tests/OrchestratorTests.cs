using Agency.Application.Interfaces;
using Agency.Application.Services;
using Agency.Infrastructure.Agents;
using Agency.Infrastructure.Orchestrator;
using Xunit;

namespace Agency.Tests;
public class OrchestratorTests
{
    [Fact]
    public async Task Orchestrator_ProducesConversation()
    {
        var store = new InMemoryConversationStore();
        var agents = new IAgent[] { new ProductManagerAgent(), new DeveloperAgent(), new TesterAgent(), new ReleaseManagerAgent() };
        var orchestrator = new SimpleOrchestrator(agents, store);
        await orchestrator.StartConversationAsync("Initial feature: greeting");
        var conv = orchestrator.GetConversation();
        Assert.NotEmpty(conv);
        Assert.Contains(conv, m => m.From == "pm");
        Assert.Contains(conv, m => m.From == "dev");
    }
}
