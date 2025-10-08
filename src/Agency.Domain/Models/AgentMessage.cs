namespace Agency.Domain.Models;
public record AgentMessage(string From, string Role, string Content, DateTime Timestamp);
