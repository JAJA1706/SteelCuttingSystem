using System.Collections.Immutable;
using System.Text;

namespace SteelCutOptimizer.Server.Middleware
{
    public class RequestRateLimiter
    {
        private readonly RequestDelegate _next;
        private static Dictionary<string, DateTime> _clients = new Dictionary<string, DateTime>();
        private static readonly object _lock = new object();

        public RequestRateLimiter(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var clientIP = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var currentTime = DateTime.UtcNow;

            lock (_lock)
            {
                if (_clients.ContainsKey(clientIP))
                {
                    var lastRequestTime = _clients[clientIP];
                    if ((currentTime - lastRequestTime).TotalMilliseconds < 250)
                    {
                        context.Response.StatusCode = 429;
                        context.Response.Body.WriteAsync(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes("Too many requests")));
                        return;
                    }
                }
                _clients[clientIP] = currentTime;
            }

            await _next(context);
        }
    }
}
