using Agency.Application.Interfaces;
using Agency.Domain.Models;
namespace Agency.Infrastructure.Agents;
public abstract class AgentBase : IAgent
{
    public AgentDescriptor Descriptor { get; }
    protected AgentBase(string id, string role, string? parent = null) => Descriptor = new AgentDescriptor(id, role, parent);
    public abstract Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default);
}
