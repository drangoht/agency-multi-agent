using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOllamaClient
    {
        Task<string> GenerateAsync(string prompt, string model = null);
    }
}
