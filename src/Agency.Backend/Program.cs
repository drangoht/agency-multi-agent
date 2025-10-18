
using Agency.Backend;
using Agency.Application.Interfaces;
using Agency.Application.Services;
using Agency.Backend.Hubs;
using Agency.Infrastructure.Agents;
using Agency.Infrastructure.Orchestrator;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

// Minimal API startup for Agency.Backend
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddSignalR();
builder.Services.AddSingleton<IConversationStore, InMemoryConversationStore>();
builder.Services.Configure<OllamaSettings>(builder.Configuration.GetSection("Ollama"));
builder.Services.AddHttpClient<IOllamaClient, OllamaClient>();

// Agents (register as IAgent so SimpleOrchestrator can resolve IEnumerable<IAgent>)
builder.Services.AddTransient<IAgent, DeveloperAgent>();
builder.Services.AddTransient<IAgent, ProductManagerAgent>();
builder.Services.AddTransient<IAgent, TesterAgent>();
builder.Services.AddTransient<IAgent, ReleaseManagerAgent>();

// Orchestrator and CORS
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

await app.RunAsync();

// StartRequest moved to StartRequest.cs to satisfy namespace rules
