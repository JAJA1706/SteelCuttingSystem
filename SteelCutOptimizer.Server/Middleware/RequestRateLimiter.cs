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
                    if ((currentTime - lastRequestTime).TotalSeconds < 1)
                    {
                        context.Response.StatusCode = 429; // Too Many Requests
                        return;
                    }
                }
                _clients[clientIP] = currentTime;
            }

            await _next(context);
        }
    }
}
