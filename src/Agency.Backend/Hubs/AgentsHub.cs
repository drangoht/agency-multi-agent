using Microsoft.AspNetCore.SignalR;
using Agency.Application.Interfaces;
namespace Agency.Backend.Hubs;
public class AgentsHub : Hub
{
    private readonly IAgentOrchestrator _orchestrator;
    public AgentsHub(IAgentOrchestrator orchestrator) => _orchestrator = orchestrator;
    public async Task StartConversation(string initialPrompt)
    {
        await _orchestrator.StartConversationAsync(initialPrompt);
        // broadcast conversation
        var conv = _orchestrator.GetConversation();
        await Clients.All.SendAsync("ConversationUpdated", conv);
    }
}
