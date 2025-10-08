namespace Agency.Domain.Models;
public record AgentDescriptor(string Id, string Role, string? ParentId = null);
