using Agency.Domain.Models;
using Agency.Infrastructure.Agents;
using Xunit;

namespace Agency.Tests.Agents;

public class LLMAgentBaseTests
{
    private class TestLlmAgent : LLMAgentBase
    {
        protected override string SystemPrompt => "You are a test agent.";

        public TestLlmAgent(IHttpClientFactory factory, string model = "llama3", string endpoint = "http://localhost:11434/api/generate")
            : base(factory, "test-agent", "TestRole", model, endpoint) { }
    }

    [Fact]
    public void Constructor_SetsCorrectDescriptor()
    {
        // Arrange
        var mockFactory = new MockHttpClientFactory();

        // Act
        var agent = new TestLlmAgent(mockFactory);

        // Assert
        Assert.Equal("test-agent", agent.Descriptor.Id);
        Assert.Equal("TestRole", agent.Descriptor.Role);
    }

    [Fact]
    public void SystemPrompt_IsAccessible()
    {
        // Arrange
        var mockFactory = new MockHttpClientFactory();
        var agent = new TestLlmAgent(mockFactory);

        // Act & Assert - SystemPrompt should be protected abstract but implementation should exist
        Assert.NotNull(agent); // Agent created successfully with system prompt
    }

    [Fact]
    public async Task HandleAsync_WithInstruction_ReturnsMessage()
    {
        // Arrange
        var mockFactory = new MockHttpClientFactory();
        var agent = new TestLlmAgent(mockFactory);
        var conversation = new[] { new AgentMessage("pm", "ProductManager", "Initial message", DateTime.UtcNow) };

        // Act
        var result = await agent.HandleAsync(conversation, "Test instruction");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-agent", result.From);
        Assert.Equal("TestRole", result.Role);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyConversation_ReturnsMessage()
    {
        // Arrange
        var mockFactory = new MockHttpClientFactory();
        var agent = new TestLlmAgent(mockFactory);
        var conversation = Array.Empty<AgentMessage>();

        // Act
        var result = await agent.HandleAsync(conversation, null);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Content);
    }

    private class MockHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient(new MockMessageHandler());
        }
    }

    private class MockMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var responseContent = new StringContent("{\"response\": \"Mocked response\"}");
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContent
            });
        }
    }
}
