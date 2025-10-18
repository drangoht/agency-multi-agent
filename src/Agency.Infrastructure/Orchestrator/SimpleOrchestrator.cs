using Agency.Application.Interfaces;
using Agency.Application.Services;
using Agency.Domain.Models;
namespace Agency.Infrastructure.Orchestrator;
public class SimpleOrchestrator : IAgentOrchestrator
{
    private readonly IEnumerable<IAgent> _agents;
    private readonly IConversationStore _store;
    public SimpleOrchestrator(IEnumerable<IAgent> agents, IConversationStore store)
    {
        _agents = agents;
        _store = store;
    }

    public IEnumerable<AgentDescriptor> GetAgents() => _agents.Select(a => a.Descriptor);

    public IEnumerable<AgentMessage> GetConversation() => _store.GetAll();

    public async Task StartConversationAsync(string initialPrompt, CancellationToken cancellationToken = default)
    {
        // Product manager starts
        var pm = _agents.First(a => a.Descriptor.Role == "ProductManager");
        var pmMsg = await pm.HandleAsync(Array.Empty<AgentMessage>(), initialPrompt, cancellationToken);
        if (pmMsg is not null) { _store.Add(pmMsg); }

        // Optional intermediate LLM agent (role: "OllamaGuy"). Make lookup safe so orchestrator
        // still works when the agent is not registered (tests and lightweight runs).
        var ollama = _agents.FirstOrDefault(a => a.Descriptor.Role == "OllamaGuy");
        if (ollama is not null)
        {
            var ollamaMsg = await ollama.HandleAsync(_store.GetAll(), pmMsg?.Content, cancellationToken);
            if (ollamaMsg is not null) { _store.Add(ollamaMsg); }
        }
        
        // Developer responds to PM
        var dev = _agents.First(a => a.Descriptor.Role == "Developer");
        var devMsg = await dev.HandleAsync(_store.GetAll(), pmMsg?.Content, cancellationToken);
        if (devMsg is not null) { _store.Add(devMsg); }

        // Tester writes tests
        var tester = _agents.First(a => a.Descriptor.Role == "Tester");
        var testMsg = await tester.HandleAsync(_store.GetAll(), null, cancellationToken);
        if (testMsg is not null) { _store.Add(testMsg); }

        // Release manager prepares release
        var rel = _agents.First(a => a.Descriptor.Role == "ReleaseManager");
        var relMsg = await rel.HandleAsync(_store.GetAll(), null, cancellationToken);
        if (relMsg is not null) { _store.Add(relMsg); }
    }
}
