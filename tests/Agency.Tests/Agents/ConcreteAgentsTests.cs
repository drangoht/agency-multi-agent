using Agency.Domain.Models;
using Agency.Infrastructure.Agents;
using Application.Interfaces;
using Xunit;

namespace Agency.Tests.Agents;

public class ConcreteAgentsTests
{
    private class MockOllamaClient : IOllamaClient
    {
        private readonly string _response;

        public MockOllamaClient(string response = "Mocked response")
        {
            _response = response;
        }

        public Task<string> GenerateAsync(string prompt, string model = null)
        {
            return Task.FromResult(_response);
        }
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
            var responseContent = new StringContent("{\"response\": \"Mocked LLM response\"}");
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = responseContent
            });
        }
    }

    [Fact]
    public void DeveloperAgent_HasCorrectDescriptor()
    {
        // Arrange
        var mockClient = new MockOllamaClient();
        var agent = new DeveloperAgent(mockClient);

        // Act & Assert
        Assert.Equal("dev", agent.Descriptor.Id);
        Assert.Equal("Developer", agent.Descriptor.Role);
    }

    [Fact]
    public async Task DeveloperAgent_HandleAsync_ReturnsMessage()
    {
        // Arrange
        var mockClient = new MockOllamaClient("Implemented feature");
        var agent = new DeveloperAgent(mockClient);
        var conversation = new[]
        {
            new AgentMessage("pm", "ProductManager", "Add login feature", DateTime.UtcNow)
        };

        // Act
        var result = await agent.HandleAsync(conversation, "Add login");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("dev", result.From);
        Assert.Equal("Developer", result.Role);
        Assert.Equal("Implemented feature", result.Content);
    }

    [Fact]
    public async Task DeveloperAgent_WithoutInstruction_UsesDefaultPrompt()
    {
        // Arrange
        var mockClient = new MockOllamaClient("Implementation");
        var agent = new DeveloperAgent(mockClient);
        var conversation = Array.Empty<AgentMessage>();

        // Act
        var result = await agent.HandleAsync(conversation);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Content);
    }

    [Fact]
    public void ProductManagerAgent_HasCorrectDescriptor()
    {
        // Arrange
        var mockFactory = new MockHttpClientFactory();
        var agent = new ProductManagerAgent(mockFactory);

        // Act & Assert
        Assert.Equal("pm", agent.Descriptor.Id);
        Assert.Equal("ProductManager", agent.Descriptor.Role);
    }

    [Fact]
    public async Task ProductManagerAgent_HandleAsync_ReturnsMessage()
    {
        // Arrange
        var mockFactory = new MockHttpClientFactory();
        var agent = new ProductManagerAgent(mockFactory);
        var conversation = Array.Empty<AgentMessage>();

        // Act
        var result = await agent.HandleAsync(conversation, "Add greeting feature");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("pm", result.From);
        Assert.Equal("ProductManager", result.Role);
        Assert.NotEmpty(result.Content);
    }

    [Fact]
    public void TesterAgent_HasCorrectDescriptor()
    {
        // Arrange
        var mockFactory = new MockHttpClientFactory();
        var agent = new TesterAgent(mockFactory);

        // Act & Assert
        Assert.Equal("qa", agent.Descriptor.Id);
        Assert.Equal("Tester", agent.Descriptor.Role);
    }

    [Fact]
    public async Task TesterAgent_HandleAsync_ReturnsMessage()
    {
        // Arrange
        var mockFactory = new MockHttpClientFactory();
        var agent = new TesterAgent(mockFactory);
        var conversation = new[]
        {
            new AgentMessage("pm", "ProductManager", "Add feature", DateTime.UtcNow),
            new AgentMessage("dev", "Developer", "Feature implemented", DateTime.UtcNow.AddSeconds(1))
        };

        // Act
        var result = await agent.HandleAsync(conversation);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("qa", result.From);
        Assert.Equal("Tester", result.Role);
        Assert.NotEmpty(result.Content);
    }

    [Fact]
    public void ReleaseManagerAgent_HasCorrectDescriptor()
    {
        // Arrange
        var mockFactory = new MockHttpClientFactory();
        var agent = new ReleaseManagerAgent(mockFactory);

        // Act & Assert
        Assert.Equal("rm", agent.Descriptor.Id);
        Assert.Equal("ReleaseManager", agent.Descriptor.Role);
    }

    [Fact]
    public async Task ReleaseManagerAgent_HandleAsync_ReturnsMessage()
    {
        // Arrange
        var mockFactory = new MockHttpClientFactory();
        var agent = new ReleaseManagerAgent(mockFactory);
        var conversation = new[]
        {
            new AgentMessage("pm", "ProductManager", "Release needed", DateTime.UtcNow),
            new AgentMessage("dev", "Developer", "Code ready", DateTime.UtcNow.AddSeconds(1)),
            new AgentMessage("qa", "Tester", "Tests passed", DateTime.UtcNow.AddSeconds(2))
        };

        // Act
        var result = await agent.HandleAsync(conversation);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("rm", result.From);
        Assert.Equal("ReleaseManager", result.Role);
        Assert.NotEmpty(result.Content);
    }
}
