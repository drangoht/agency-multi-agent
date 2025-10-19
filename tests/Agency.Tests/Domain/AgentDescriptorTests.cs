using Agency.Domain.Models;
using Xunit;

namespace Agency.Tests.Domain;

public class AgentDescriptorTests
{
    [Fact]
    public void Constructor_CreatesDescriptorWithIdAndRole()
    {
        // Arrange & Act
        var descriptor = new AgentDescriptor("agent1", "Developer");

        // Assert
        Assert.Equal("agent1", descriptor.Id);
        Assert.Equal("Developer", descriptor.Role);
        Assert.Null(descriptor.ParentId);
    }

    [Fact]
    public void Constructor_CreatesDescriptorWithParentId()
    {
        // Arrange & Act
        var descriptor = new AgentDescriptor("agent1", "Developer", "parent1");

        // Assert
        Assert.Equal("agent1", descriptor.Id);
        Assert.Equal("Developer", descriptor.Role);
        Assert.Equal("parent1", descriptor.ParentId);
    }

    [Fact]
    public void Descriptor_IsRecord()
    {
        // Arrange
        var desc1 = new AgentDescriptor("id1", "Developer", "parent1");
        var desc2 = new AgentDescriptor("id1", "Developer", "parent1");

        // Act & Assert - records are equal by value
        Assert.Equal(desc1, desc2);
    }

    [Fact]
    public void Descriptor_AreNotEqual_WhenIdDiffers()
    {
        // Arrange
        var desc1 = new AgentDescriptor("id1", "Developer");
        var desc2 = new AgentDescriptor("id2", "Developer");

        // Act & Assert
        Assert.NotEqual(desc1, desc2);
    }
}
