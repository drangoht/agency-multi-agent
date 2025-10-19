using Agency.Application.Interfaces;
using Agency.Domain.Models;
using Agency.Infrastructure.Agents;
using Xunit;

namespace Agency.Tests.Agents;

public class AgentBaseTests
{
    private class TestAgent : AgentBase
    {
        public TestAgent(string id, string role, string? parent = null)
            : base(id, role, parent) { }

        public override Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default)
        {
            var msg = new AgentMessage(Descriptor.Id, Descriptor.Role, "Test response", DateTime.UtcNow);
            return Task.FromResult<AgentMessage?>(msg);
        }
    }

    [Fact]
    public void Constructor_SetsDescriptor_WithIdAndRole()
    {
        // Arrange & Act
        var agent = new TestAgent("agent1", "Developer");

        // Assert
        Assert.NotNull(agent.Descriptor);
        Assert.Equal("agent1", agent.Descriptor.Id);
        Assert.Equal("Developer", agent.Descriptor.Role);
        Assert.Null(agent.Descriptor.ParentId);
    }

    [Fact]
    public void Constructor_SetsDescriptor_WithParentId()
    {
        // Arrange & Act
        var agent = new TestAgent("agent1", "Developer", "parent1");

        // Assert
        Assert.NotNull(agent.Descriptor);
        Assert.Equal("agent1", agent.Descriptor.Id);
        Assert.Equal("Developer", agent.Descriptor.Role);
        Assert.Equal("parent1", agent.Descriptor.ParentId);
    }

    [Fact]
    public void Descriptor_IsAccessible()
    {
        // Arrange
        var agent = new TestAgent("dev", "Developer");

        // Act
        var descriptor = agent.Descriptor;

        // Assert
        Assert.IsAssignableFrom<AgentDescriptor>(descriptor);
        Assert.Equal("dev", descriptor.Id);
    }

    [Fact]
    public async Task HandleAsync_ImplementsIAgent()
    {
        // Arrange
        var agent = new TestAgent("dev", "Developer");
        var conversation = new AgentMessage[]
        {
            new("pm", "ProductManager", "Requirement: add greeting", DateTime.UtcNow)
        };

        // Act
        var result = await agent.HandleAsync(conversation);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("dev", result.From);
        Assert.Equal("Developer", result.Role);
        Assert.Equal("Test response", result.Content);
    }
}
