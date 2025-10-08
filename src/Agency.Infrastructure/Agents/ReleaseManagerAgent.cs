using Agency.Application.Interfaces;
using Agency.Domain.Models;
namespace Agency.Infrastructure.Agents;
public class ReleaseManagerAgent : AgentBase
{
    public ReleaseManagerAgent(string id = "rel", string? parent = "pm") : base(id, "ReleaseManager", parent) {}
    public override Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default)
    {
        var content = "Release manager prepares release notes and deployment steps (docker image, compose).";
        var msg = new AgentMessage(Descriptor.Id, Descriptor.Role, content, DateTime.UtcNow);
        return Task.FromResult<AgentMessage?>(msg);
    }
}
