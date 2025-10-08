using Agency.Application.Interfaces;
using Agency.Domain.Models;
namespace Agency.Infrastructure.Agents;
public class TesterAgent : AgentBase
{
    public TesterAgent(string id = "tester", string? parent = "dev") : base(id, "Tester", parent) {}
    public override Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default)
    {
        var content = "Tester creates a unit test to validate HelloWorld endpoint returns expected text.";
        var msg = new AgentMessage(Descriptor.Id, Descriptor.Role, content, DateTime.UtcNow);
        return Task.FromResult<AgentMessage?>(msg);
    }
}
