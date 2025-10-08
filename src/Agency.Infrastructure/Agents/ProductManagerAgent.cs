using Agency.Application.Interfaces;
using Agency.Domain.Models;
namespace Agency.Infrastructure.Agents;
public class ProductManagerAgent : AgentBase
{
    public ProductManagerAgent(string id = "pm") : base(id, "ProductManager") {}
    public override Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default)
    {
        // Decides a feature breakdown and delegates to developer
        var content = instruction is null
            ? "Product manager defines the scope: create a small feature - hello world endpoint and tests."
            : $"Product manager refines: {instruction}";
        var msg = new AgentMessage(Descriptor.Id, Descriptor.Role, content, DateTime.UtcNow);
        return Task.FromResult<AgentMessage?>(msg);
    }
}
