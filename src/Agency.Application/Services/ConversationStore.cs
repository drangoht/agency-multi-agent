using Agency.Domain.Models;
namespace Agency.Application.Services;
public interface IConversationStore
{
    void Add(AgentMessage message);
    IReadOnlyList<AgentMessage> GetAll();
}
public class InMemoryConversationStore : IConversationStore
{
    private readonly List<AgentMessage> _messages = new();
    public void Add(AgentMessage message) => _messages.Add(message);
    public IReadOnlyList<AgentMessage> GetAll() => _messages.AsReadOnly();
}
