using Agency.Application.Interfaces;
using Agency.Domain.Models;
namespace Agency.Infrastructure.Agents;
public class DeveloperAgent : AgentBase
{
    public DeveloperAgent(string id = "dev", string? parent = "pm") : base(id, "Developer", parent) {}
    public override Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default)
    {
        var content = instruction is null
            ? "Developer implements: minimal API controller with a HelloWorld endpoint."
            : $"Developer implements: {instruction}";
        var msg = new AgentMessage(Descriptor.Id, Descriptor.Role, content, DateTime.UtcNow);
        return Task.FromResult<AgentMessage?>(msg);
    }
}
