using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class OllamaClient : IOllamaClient
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaSettings _settings;
        private readonly ILogger<OllamaClient> _logger;

        public OllamaClient(HttpClient httpClient, IOptions<OllamaSettings> settings, ILogger<OllamaClient> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        }

        public async Task<string> GenerateAsync(string prompt, string model = null)
        {
            var payload = new
            {
                model = model ?? _settings.Model,
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_settings.BaseUrl, content);

                // Lire le corps de la réponse pour le logger, puis vérifier le statut.
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Appel Ollama échoué : {StatusCode}. Réponse: {ResponseBody}", response.StatusCode, jsonResponse);
                    response.EnsureSuccessStatusCode(); // lance l'exception après logging
                }

                using var doc = JsonDocument.Parse(jsonResponse);

                if (doc.RootElement.TryGetProperty("response", out var result))
                    return result.GetString() ?? string.Empty;

                return jsonResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'appel à Ollama: {Message}", ex.Message);
                throw;
            }
        }
    }
}
