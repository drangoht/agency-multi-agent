using Agency.Application.Services;
using Agency.Domain.Models;
using Xunit;

namespace Agency.Tests.Services;

public class ConversationStoreTests
{
    [Fact]
    public void GetAll_ReturnsEmptyList_WhenNoMessagesAdded()
    {
        // Arrange
        var store = new InMemoryConversationStore();

        // Act
        var messages = store.GetAll();

        // Assert
        Assert.NotNull(messages);
        Assert.Empty(messages);
    }

    [Fact]
    public void Add_AddsMessageToStore()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var message = new AgentMessage("dev", "Developer", "test content", DateTime.UtcNow);

        // Act
        store.Add(message);
        var messages = store.GetAll();

        // Assert
        Assert.Single(messages);
        Assert.Equal(message, messages[0]);
    }

    [Fact]
    public void Add_MultipleMessages_MaintainsOrder()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        var msg1 = new AgentMessage("pm", "ProductManager", "msg1", DateTime.UtcNow);
        var msg2 = new AgentMessage("dev", "Developer", "msg2", DateTime.UtcNow.AddSeconds(1));
        var msg3 = new AgentMessage("tester", "Tester", "msg3", DateTime.UtcNow.AddSeconds(2));

        // Act
        store.Add(msg1);
        store.Add(msg2);
        store.Add(msg3);
        var messages = store.GetAll();

        // Assert
        Assert.Equal(3, messages.Count);
        Assert.Equal(msg1, messages[0]);
        Assert.Equal(msg2, messages[1]);
        Assert.Equal(msg3, messages[2]);
    }

    [Fact]
    public void GetAll_ReturnsReadOnlyList()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        store.Add(new AgentMessage("dev", "Developer", "content", DateTime.UtcNow));

        // Act
        var messages = store.GetAll();

        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<AgentMessage>>(messages);
    }

    [Fact]
    public void GetAll_ReturnsReadOnlyCollection()
    {
        // Arrange
        var store = new InMemoryConversationStore();
        store.Add(new AgentMessage("dev", "Developer", "content1", DateTime.UtcNow));

        // Act
        var messages = store.GetAll();

        // Assert - should be read-only collection
        var readOnlyList = messages as System.Collections.ObjectModel.ReadOnlyCollection<AgentMessage>;
        Assert.NotNull(readOnlyList);
    }
}
