using Agency.Domain.Models;
namespace Agency.Application.Interfaces;
public interface IAgentOrchestrator
{
    Task StartConversationAsync(string initialPrompt, CancellationToken cancellationToken = default);
    IEnumerable<AgentDescriptor> GetAgents();
    IEnumerable<AgentMessage> GetConversation();
}
