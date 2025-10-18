namespace Application.Services
{
    public class OllamaSettings
    {
        public string BaseUrl { get; set; }
        public string Model { get; set; }
        public int TimeoutSeconds { get; set; } = 60;
    }
}
