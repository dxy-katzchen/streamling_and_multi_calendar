
using System.Text;


namespace API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Enable buffering to read the request body
            context.Request.EnableBuffering();

            // Read the request body into a string
            var requestBody = await ReadRequestBodyAsync(context.Request);

            // Log request details including the body
            _logger.LogInformation("Request:\nMethod: {method}\nUrl: {url}\nHeaders: {headers}\nBody: {body}",
                context.Request.Method,
                context.Request.Path,
                context.Request.Headers,
                requestBody);

            // Reset the request body stream position so it can be read again
            context.Request.Body.Position = 0;

            await _next(context);
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                return body;
            }
        }
    }
}