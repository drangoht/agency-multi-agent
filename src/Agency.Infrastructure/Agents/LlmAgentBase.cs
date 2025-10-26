using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Agency.Application.Interfaces;
using Agency.Domain.Models;

namespace Agency.Infrastructure.Agents
{
    public abstract class LLMAgentBase : AgentBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _model;
        private readonly string _endpoint;

        protected abstract string SystemPrompt { get; }

        protected LLMAgentBase(IHttpClientFactory httpClientFactory, string id, string role, string model = "llama3", string endpoint = "http://localhost:11434/api/generate")
            : base(id, role)
        {
            _httpClientFactory = httpClientFactory;
            _model = model;
            _endpoint = endpoint;
        }

        public override async Task<AgentMessage?> HandleAsync(IEnumerable<AgentMessage> conversation, string? instruction = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine(SystemPrompt);

                if (!string.IsNullOrWhiteSpace(instruction))
                {
                    sb.AppendLine($"Instruction: {instruction}");
                }

                if (conversation != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Context:");
                    foreach (var msg in conversation.Skip(Math.Max(0, conversation.Count() - 12)))
                    {
                        sb.AppendLine($"{msg.Role} ({msg.From}): {msg.Content}");
                    }
                }

                var prompt = sb.ToString();

                var payload = new
                {
                    model = _model,
                    prompt,
                    max_tokens = 512,
                    temperature = 0.3,
                    stream = false
                };

                var client = _httpClientFactory.CreateClient();
                using var response = await client.PostAsJsonAsync(_endpoint, payload, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return new AgentMessage(Descriptor.Id, Descriptor.Role, $"Ollama Error: {response.StatusCode}", DateTime.UtcNow);

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var text = ParseOllamaResponse(json);
                if (string.IsNullOrWhiteSpace(text))
                    text = "[LLMAgentBase] Empty response.";

                return new AgentMessage(Descriptor.Id, Descriptor.Role, text, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                return new AgentMessage(Descriptor.Id, Descriptor.Role, $"[LLMAgentBase] Exception: {ex.Message}", DateTime.UtcNow);
            }
        }

        private static string ParseOllamaResponse(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("response", out var res))
                    return res.GetString() ?? "";
                return doc.RootElement.ToString();
            }
            catch
            {
                return json;
            }
        }
    }
}
