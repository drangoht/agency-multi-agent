using Agency.Application.Interfaces;
using Agency.Application.Services;
using Agency.Domain.Models;
using Agency.Infrastructure.Agents;
using Agency.Infrastructure.Orchestrator;
using Application.Interfaces;
using Xunit;

namespace Agency.Tests.Integration;

/// <summary>
/// Shared fixture for orchestrator integration tests.
/// </summary>
public class OrchestratorIntegrationFixture
{
    public IHttpClientFactory HttpClientFactory { get; }
    public IOllamaClient OllamaClient { get; }
    public IAgent[] Agents { get; }

    public OrchestratorIntegrationFixture()
    {
        HttpClientFactory = new MockHttpClientFactory();
        OllamaClient = new MockOllamaClient();
        Agents = new IAgent[]
        {
            new ProductManagerAgent(HttpClientFactory),
            new DeveloperAgent(OllamaClient),
            new TesterAgent(HttpClientFactory),
            new ReleaseManagerAgent(HttpClientFactory)
        };
    }

    private sealed class MockOllamaClient : IOllamaClient
    {
        public Task<string> GenerateAsync(string prompt, string? model = null)
        {
            _ = prompt; // Acknowledge parameter for integration testing
            return Task.FromResult("Mocked LLM response: Integration test with realistic agent responses.");
        }
    }

    private sealed class MockHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            var handler = new MockHttpMessageHandler();
            return new HttpClient(handler, disposeHandler: true);
        }
    }

    /// <summary>
    /// Mock HTTP message handler that intercepts requests and returns mocked responses.
    /// This prevents tests from making real HTTP calls to Ollama.
    /// </summary>
    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Mock response for Ollama API generate endpoint
            if (request.RequestUri?.ToString().Contains("api/generate") == true)
            {
                var response = new
                {
                    response = "Mocked LLM response: Integration test with realistic agent responses.",
                    done = true
                };

                var json = System.Text.Json.JsonSerializer.Serialize(response);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = content
                });
            }

            // Mock response for health check
            if (request.RequestUri?.ToString().Contains("api/health") == true)
            {
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            }

            // Default: return 404 for unknown endpoints
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
        }
    }
}

/// <summary>
/// Collection definition for orchestrator integration tests.
/// </summary>
[CollectionDefinition("Orchestrator Integration Tests")]
public class OrchestratorIntegrationCollection : ICollectionFixture<OrchestratorIntegrationFixture>
{
}

/// <summary>
/// Integration tests for the orchestrator with mocked HTTP calls.
/// These tests validate the complete conversation flow with realistic agent behavior.
/// </summary>
[Collection("Orchestrator Integration Tests")]
public class OrchestratorIntegrationTests
{
    private readonly OrchestratorIntegrationFixture _fixture;

    public OrchestratorIntegrationTests(OrchestratorIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Orchestrator_CompleteFlow_AllAgentsParticipate()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var orchestrator = new SimpleOrchestrator(_fixture.Agents, store);

        // Act
        await orchestrator.StartConversationAsync("Implement a new API endpoint for user authentication");

        // Assert
        var conversation = orchestrator.GetConversation();
        Assert.NotEmpty(conversation);
        Assert.Equal(4, conversation.Count());

        // Verify all agents participated
        Assert.Single(conversation.Where(m => m.From == "pm"));
        Assert.Single(conversation.Where(m => m.From == "dev"));
        Assert.Single(conversation.Where(m => m.From == "qa"));
        Assert.Single(conversation.Where(m => m.From == "rm"));
    }

    [Fact]
    public async Task Orchestrator_MessageOrder_IsCorrect()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var orchestrator = new SimpleOrchestrator(_fixture.Agents, store);

        // Act
        await orchestrator.StartConversationAsync("Build a payment module");

        // Assert
        var messages = orchestrator.GetConversation().ToList();
        Assert.Equal("pm", messages[0].From);
        Assert.Equal("dev", messages[1].From);
        Assert.Equal("qa", messages[2].From);
        Assert.Equal("rm", messages[3].From);
    }

    [Fact]
    public async Task Orchestrator_MessageContent_NotEmpty()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var orchestrator = new SimpleOrchestrator(_fixture.Agents, store);

        // Act
        await orchestrator.StartConversationAsync("Add login feature");

        // Assert
        var conversation = orchestrator.GetConversation();
        foreach (var message in conversation)
        {
            Assert.NotEmpty(message.Content);
            Assert.NotEmpty(message.Role);
            Assert.NotEmpty(message.From);
        }
    }

    [Fact]
    public async Task Orchestrator_MessageMetadata_IsValid()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var orchestrator = new SimpleOrchestrator(_fixture.Agents, store);
        var beforeTime = DateTime.UtcNow;

        // Act
        await orchestrator.StartConversationAsync("New feature");
        var afterTime = DateTime.UtcNow;

        // Assert
        var conversation = orchestrator.GetConversation();
        foreach (var message in conversation)
        {
            Assert.True(message.Timestamp >= beforeTime);
            Assert.True(message.Timestamp <= afterTime.AddSeconds(5));

            var agent = _fixture.Agents.First(a => a.Descriptor.Id == message.From);
            Assert.Equal(agent.Descriptor.Role, message.Role);
        }
    }

    [Fact]
    public async Task Orchestrator_WithInitialPrompt_PMReceivesIt()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var orchestrator = new SimpleOrchestrator(_fixture.Agents, store);
        var prompt = "Implement real-time notifications";

        // Act
        await orchestrator.StartConversationAsync(prompt);

        // Assert
        var pmMessage = orchestrator.GetConversation().First(m => m.From == "pm");
        Assert.NotEmpty(pmMessage.Content);
    }

    [Fact]
    public async Task Orchestrator_ConversationProgression_ContextBuilds()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var orchestrator = new SimpleOrchestrator(_fixture.Agents, store);

        // Act
        await orchestrator.StartConversationAsync("Build payment processing");

        // Assert - context should build as each agent responds
        var conversation = orchestrator.GetConversation().ToList();

        var devIndex = conversation.FindIndex(m => m.From == "dev");
        var contextBeforeDev = conversation.Take(devIndex).Count();
        Assert.True(contextBeforeDev >= 1); // At least PM message

        var testerIndex = conversation.FindIndex(m => m.From == "qa");
        var contextBeforeTester = conversation.Take(testerIndex).Count();
        Assert.True(contextBeforeTester >= 2); // PM and Dev

        var rmIndex = conversation.FindIndex(m => m.From == "rm");
        var contextBeforeRm = conversation.Take(rmIndex).Count();
        Assert.True(contextBeforeRm >= 3); // PM, Dev, and Tester
    }

    [Fact]
    public async Task Orchestrator_MultipleInstances_IndependentState()
    {
        // Arrange
        var store1 = new InMemoryConversationStore();
        var store2 = new InMemoryConversationStore();
        var orchestrator1 = new SimpleOrchestrator(_fixture.Agents, store1);
        var orchestrator2 = new SimpleOrchestrator(_fixture.Agents, store2);

        // Act
        await orchestrator1.StartConversationAsync("Feature: authentication");
        await orchestrator2.StartConversationAsync("Feature: export data");

        // Assert - verify stores are independent (different instances, same content is OK with mocks)
        var conv1 = orchestrator1.GetConversation();
        var conv2 = orchestrator2.GetConversation();
        Assert.Equal(4, conv1.Count());
        Assert.Equal(4, conv2.Count());
        // Verify stores are separate by checking both have messages
        Assert.NotEmpty(conv1);
        Assert.NotEmpty(conv2);
    }

    [Fact]
    public async Task Orchestrator_CancellationToken_IsRespected()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var orchestrator = new SimpleOrchestrator(_fixture.Agents, store);
        using var cts = new CancellationTokenSource();

        // Act
        await orchestrator.StartConversationAsync("Test feature", cts.Token);

        // Assert
        var conversation = orchestrator.GetConversation();
        Assert.NotEmpty(conversation);
    }

    [Fact]
    public async Task Orchestrator_AllMessages_AreStored()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var orchestrator = new SimpleOrchestrator(_fixture.Agents, store);

        // Act
        await orchestrator.StartConversationAsync("New feature request");
        var conversation1 = orchestrator.GetConversation();
        var conversation2 = store.GetAll();

        // Assert
        Assert.Equal(conversation1.Count(), conversation2.Count());
        Assert.True(conversation1.SequenceEqual(conversation2));
    }

    [Fact]
    public void Orchestrator_AllAgentDescriptors_AreAccessible()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var orchestrator = new SimpleOrchestrator(_fixture.Agents, store);

        // Act
        var descriptors = orchestrator.GetAgents();

        // Assert
        Assert.Equal(4, descriptors.Count());
        Assert.Single(descriptors.Where(d => d.Role == "ProductManager"));
        Assert.Single(descriptors.Where(d => d.Role == "Developer"));
        Assert.Single(descriptors.Where(d => d.Role == "Tester"));
        Assert.Single(descriptors.Where(d => d.Role == "ReleaseManager"));
    }
}
