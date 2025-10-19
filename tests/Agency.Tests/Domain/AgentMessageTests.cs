using Agency.Domain.Models;
using Xunit;

namespace Agency.Tests.Domain;

public class AgentMessageTests
{
    [Fact]
    public void Constructor_CreatesMessageWithAllProperties()
    {
        // Arrange
        var from = "dev";
        var role = "Developer";
        var content = "Implementation complete";
        var timestamp = DateTime.UtcNow;

        // Act
        var message = new AgentMessage(from, role, content, timestamp);

        // Assert
        Assert.Equal(from, message.From);
        Assert.Equal(role, message.Role);
        Assert.Equal(content, message.Content);
        Assert.Equal(timestamp, message.Timestamp);
    }

    [Fact]
    public void Message_IsRecord()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var msg1 = new AgentMessage("dev", "Developer", "content", now);
        var msg2 = new AgentMessage("dev", "Developer", "content", now);

        // Act & Assert - records are equal by value
        Assert.Equal(msg1, msg2);
    }

    [Fact]
    public void Message_AreNotEqual_WhenContentDiffers()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var msg1 = new AgentMessage("dev", "Developer", "content1", now);
        var msg2 = new AgentMessage("dev", "Developer", "content2", now);

        // Act & Assert
        Assert.NotEqual(msg1, msg2);
    }

    [Fact]
    public void Message_AreNotEqual_WhenFromDiffers()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var msg1 = new AgentMessage("dev1", "Developer", "content", now);
        var msg2 = new AgentMessage("dev2", "Developer", "content", now);

        // Act & Assert
        Assert.NotEqual(msg1, msg2);
    }

    [Fact]
    public void Message_WithEmptyContent()
    {
        // Arrange & Act
        var message = new AgentMessage("dev", "Developer", "", DateTime.UtcNow);

        // Assert
        Assert.Empty(message.Content);
    }
}
