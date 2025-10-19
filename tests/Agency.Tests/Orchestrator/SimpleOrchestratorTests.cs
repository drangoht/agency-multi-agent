using Agency.Application.Interfaces;
using Agency.Application.Services;
using Agency.Infrastructure.Agents;
using Agency.Infrastructure.Orchestrator;
using Agency.Domain.Models;
using Xunit;

namespace Agency.Tests.Orchestrator;

public class SimpleOrchestratorTests
{
    private class TestAgent : IAgent
    {
        public AgentDescriptor Descriptor { get; }
        private readonly string _content;

        public TestAgent(string id, string role, string content)
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

    private static SimpleOrchestrator CreateOrchestratorWithAllAgents()
    {
        var store = new InMemoryConversationStore();
        var agents = new IAgent[]
        {
            new TestAgent("pm", "ProductManager", "Feature required: greeting"),
            new TestAgent("dev", "Developer", "Code implemented"),
            new TestAgent("qa", "Tester", "Tests written"),
            new TestAgent("rm", "ReleaseManager", "Release prepared")
        };
        return new SimpleOrchestrator(agents, store);
    }



    [Fact]
    public void Constructor_StoresAgentsAndStore()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var agents = new IAgent[]
        {
            new TestAgent("pm", "ProductManager", "msg"),
            new TestAgent("dev", "Developer", "msg")
        };

        // Act
        var orchestrator = new SimpleOrchestrator(agents, store);

        // Assert
        Assert.NotNull(orchestrator);
    }

    [Fact]
    public void GetAgents_ReturnsAllAgentDescriptors()
    {
        // Arrange
        var orchestrator = CreateOrchestratorWithAllAgents();

        // Act
        var agents = orchestrator.GetAgents();

        // Assert
        Assert.NotNull(agents);
        var agentList = agents.ToList();
        Assert.Equal(4, agentList.Count);
        Assert.Single(agentList.Where(a => a.Role == "ProductManager"));
        Assert.Single(agentList.Where(a => a.Role == "Developer"));
        Assert.Single(agentList.Where(a => a.Role == "Tester"));
        Assert.Single(agentList.Where(a => a.Role == "ReleaseManager"));
    }

    [Fact]
    public void GetConversation_ReturnsEmptyInitially()
    {
        // Arrange
        var orchestrator = CreateOrchestratorWithAllAgents();

        // Act
        var conversation = orchestrator.GetConversation();

        // Assert
        Assert.Empty(conversation);
    }

    [Fact]
    public async Task StartConversationAsync_AddsProductManagerMessage()
    {
        // Arrange
        var orchestrator = CreateOrchestratorWithAllAgents();
        var prompt = "Add greeting feature";

        // Act
        await orchestrator.StartConversationAsync(prompt);

        // Assert
        var conversation = orchestrator.GetConversation();
        Assert.NotEmpty(conversation);
        Assert.Single(conversation.Where(m => m.From == "pm"));
    }

    [Fact]
    public async Task StartConversationAsync_AddsDeveloperMessage()
    {
        // Arrange
        var orchestrator = CreateOrchestratorWithAllAgents();

        // Act
        await orchestrator.StartConversationAsync("Add feature");

        // Assert
        var conversation = orchestrator.GetConversation();
        Assert.Single(conversation.Where(m => m.From == "dev"));
    }

    [Fact]
    public async Task StartConversationAsync_AddsTesterMessage()
    {
        // Arrange
        var orchestrator = CreateOrchestratorWithAllAgents();

        // Act
        await orchestrator.StartConversationAsync("Add feature");

        // Assert
        var conversation = orchestrator.GetConversation();
        Assert.Single(conversation.Where(m => m.From == "qa"));
    }

    [Fact]
    public async Task StartConversationAsync_AddsReleaseManagerMessage()
    {
        // Arrange
        var orchestrator = CreateOrchestratorWithAllAgents();

        // Act
        await orchestrator.StartConversationAsync("Add feature");

        // Assert
        var conversation = orchestrator.GetConversation();
        Assert.Single(conversation.Where(m => m.From == "rm"));
    }

    [Fact]
    public async Task StartConversationAsync_MessagesInCorrectOrder()
    {
        // Arrange
        var orchestrator = CreateOrchestratorWithAllAgents();

        // Act
        await orchestrator.StartConversationAsync("Add feature");

        // Assert
        var conversation = orchestrator.GetConversation().ToList();
        Assert.Equal("pm", conversation[0].From);
        Assert.Equal("dev", conversation[1].From);
        Assert.Equal("qa", conversation[2].From);
        Assert.Equal("rm", conversation[3].From);
    }

    [Fact]
    public async Task StartConversationAsync_WithoutOllamaAgent_StillWorks()
    {
        // Arrange
        var orchestrator = CreateOrchestratorWithAllAgents();

        // Act & Assert - should not throw
        await orchestrator.StartConversationAsync("Add feature");
        var conversation = orchestrator.GetConversation();
        Assert.NotEmpty(conversation);
    }

    [Fact]
    public async Task StartConversationAsync_WithCancellationToken()
    {
        // Arrange
        var orchestrator = CreateOrchestratorWithAllAgents();
        using var cts = new CancellationTokenSource();

        // Act
        await orchestrator.StartConversationAsync("Add feature", cts.Token);

        // Assert
        var conversation = orchestrator.GetConversation();
        Assert.NotEmpty(conversation);
    }

    [Fact]
    public async Task StartConversationAsync_PassesInstructionToDeveloper()
    {
        // Arrange
        var orchestrator = CreateOrchestratorWithAllAgents();
        var prompt = "Add greeting";

        // Act
        await orchestrator.StartConversationAsync(prompt);

        // Assert
        var conversation = orchestrator.GetConversation();
        var devMsg = conversation.First(m => m.From == "dev");
        // Developer should receive PM's instruction in its content
        Assert.NotEmpty(devMsg.Content);
    }

    [Fact]
    public async Task StartConversationAsync_SkipsNullReturnFromAgent()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var nullAgent = new NullReturnAgent("pm", "ProductManager");
        var devAgent = new TestAgent("dev", "Developer", "Dev response");
        var testerAgent = new TestAgent("qa", "Tester", "Test response");
        var rmAgent = new TestAgent("rm", "ReleaseManager", "Release response");

        var agents = new IAgent[] { nullAgent, devAgent, testerAgent, rmAgent };
        var orchestrator = new SimpleOrchestrator(agents, store);

        // Act
        await orchestrator.StartConversationAsync("Add feature");

        // Assert - PM message should not be in conversation since it returns null
        var conversation = orchestrator.GetConversation();
        Assert.DoesNotContain(conversation, m => m.From == "pm");
    }

    private class NullReturnAgent : IAgent
    {
        public AgentDescriptor Descriptor { get; }

        public NullReturnAgent(string id, string role)
        {
            Descriptor = new AgentDescriptor(id, role);
        }

        public Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<AgentMessage?>(null);
        }
    }
}
