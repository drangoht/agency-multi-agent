using Agency.Application.Interfaces;
using Agency.Application.Services;
using Agency.Infrastructure.Agents;
using Agency.Infrastructure.Orchestrator;
using Microsoft.AspNetCore.SignalR;
using Agency.Backend.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddSingleton<IConversationStore, InMemoryConversationStore>();
builder.Services.AddSingleton<IAgent, ProductManagerAgent>();
builder.Services.AddSingleton<IAgent, DeveloperAgent>();
builder.Services.AddSingleton<IAgent, TesterAgent>();
builder.Services.AddSingleton<IAgent, ReleaseManagerAgent>();
builder.Services.AddSingleton<IAgentOrchestrator, SimpleOrchestrator>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:5000")));
var app = builder.Build();
app.UseStaticFiles();
app.MapHub<AgentsHub>("/agentsHub");
app.MapGet("/api/agents", (IAgentOrchestrator orch) => Results.Ok(orch.GetAgents()));
app.MapGet("/api/conversation", (IAgentOrchestrator orch) => Results.Ok(orch.GetConversation()));
app.MapPost("/api/start", async (IAgentOrchestrator orch, HttpContext ctx) =>
{
    var body = await ctx.Request.ReadFromJsonAsync<StartRequest>();
    if (body is null || string.IsNullOrWhiteSpace(body.InitialPrompt)) return Results.BadRequest();
    await orch.StartConversationAsync(body.InitialPrompt);
    return Results.Accepted();
});
app.MapFallbackToFile("index.html");
app.Run();

record StartRequest(string InitialPrompt);
