using Microsoft.AspNetCore.SignalR;
using Agency.Application.Interfaces;
using Agency.Domain.Models;

namespace Agency.Backend.Hubs;

public class AgentsHub : Hub
{
    private readonly IAgentOrchestrator _orchestrator;

    public AgentsHub(IAgentOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public async Task StartConversation(string initialPrompt)
    {
        Console.WriteLine($"Starting conversation: {initialPrompt}");
        
        try
        {
            // Start the conversation
            await _orchestrator.StartConversationAsync(initialPrompt);
            
            // Get all messages and send them one by one
            var messages = _orchestrator.GetConversation().ToList();
            
            foreach (var message in messages)
            {
                Console.WriteLine($"Sending message: {message.Role} - {message.Content}");
                await Clients.All.SendAsync("ReceiveMessage", message);
                await Task.Delay(1000); // 1 second between each message
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            // Use Context.ConnectionId to send only to the calling client
            await Clients.Client(Context.ConnectionId).SendAsync("Error", ex.Message);
        }
    }
}
