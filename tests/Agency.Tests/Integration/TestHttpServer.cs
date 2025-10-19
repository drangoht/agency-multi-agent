using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Agency.Tests.Integration;

/// <summary>
/// Test HTTP server that simulates Ollama API responses for integration testing.
/// Allows tests to make real HTTP calls without requiring an actual Ollama instance.
/// </summary>
public class TestHttpServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task _serverTask;
    private bool _disposed = false;

    public string BaseUrl { get; }
    public int Port { get; }

    // Request/response logging
    private readonly ConcurrentBag<string> _requestLog = new();
    public IReadOnlyCollection<string> RequestLog => _requestLog.ToList().AsReadOnly();

    public TestHttpServer(int port = 0)
    {
        Port = port == 0 ? GetAvailablePort() : port;
        BaseUrl = $"http://localhost:{Port}/";
        _listener = new HttpListener();
        _listener.Prefixes.Add(BaseUrl);
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Start()
    {
        if (_serverTask != null) return;
        
        _listener.Start();
        _serverTask = HandleRequestsAsync(_cancellationTokenSource.Token);
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        try
        {
            _serverTask?.Wait(TimeSpan.FromSeconds(5));
        }
        catch (OperationCanceledException)
        {
            // Expected
        }
        _listener.Stop();
    }

    private async Task HandleRequestsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var context = _listener.GetContextAsync();
                if (await Task.WhenAny(context, Task.Delay(100, cancellationToken)) == context)
                {
                    var httpContext = await context;
                    _requestLog.Add($"{httpContext.Request.HttpMethod} {httpContext.Request.Url?.PathAndQuery}");
                    await HandleRequestAsync(httpContext);
                }
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Server error: {ex.Message}");
            }
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;

            if (request.Url.PathAndQuery == "/api/generate" && request.HttpMethod == "POST")
            {
                using (var reader = new StreamReader(request.InputStream, Encoding.UTF8))
                {
                    _ = await reader.ReadToEndAsync();
                }

                var responseData = new
                {
                    model = "llama3",
                    response = "This is a simulated Ollama response for integration testing.",
                    done = true,
                    context = new int[] { },
                    total_duration = 1000000000,
                    load_duration = 100000000,
                    prompt_eval_count = 10,
                    prompt_eval_duration = 500000000,
                    eval_count = 20,
                    eval_duration = 400000000
                };

                var responseJson = JsonSerializer.Serialize(responseData);
                var responseBytes = Encoding.UTF8.GetBytes(responseJson);

                response.ContentType = "application/json";
                response.ContentLength64 = responseBytes.Length;
                response.StatusCode = 200;

                await response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                response.OutputStream.Close();
            }
            else if (request.Url.PathAndQuery == "/api/health" && request.HttpMethod == "GET")
            {
                var responseData = new { status = "ok" };
                var responseJson = JsonSerializer.Serialize(responseData);
                var responseBytes = Encoding.UTF8.GetBytes(responseJson);

                response.ContentType = "application/json";
                response.ContentLength64 = responseBytes.Length;
                response.StatusCode = 200;

                await response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                response.OutputStream.Close();
            }
            else
            {
                response.StatusCode = 404;
                response.OutputStream.Close();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Request handler error: {ex.Message}");
        }
    }

    private static int GetAvailablePort()
    {
        using (var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp))
        {
            socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 0));
            return ((System.Net.IPEndPoint)(socket.LocalEndPoint ?? throw new InvalidOperationException("LocalEndPoint is null"))).Port;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing)
        {
            Stop();
            _listener?.Close();
            _cancellationTokenSource?.Dispose();
        }
        _disposed = true;
    }

    ~TestHttpServer()
    {
        Dispose(false);
    }
}
