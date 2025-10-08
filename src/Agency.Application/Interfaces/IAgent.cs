using Agency.Domain.Models;
namespace Agency.Application.Interfaces;
public interface IAgent
{
    AgentDescriptor Descriptor { get; }
    Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default);
}
