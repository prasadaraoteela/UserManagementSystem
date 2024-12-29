using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UserManagementSystem
{
  public class RequestResponseLoggingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      // Log the request
      context.Request.EnableBuffering();
      var requestBody = await ReadStreamAsync(context.Request.Body);
      _logger.LogInformation($"Incoming Request: {context.Request.Method} {context.Request.Path} {requestBody}");
      context.Request.Body.Position = 0;

      // Capture the response
      var originalResponseBodyStream = context.Response.Body;
      using var responseBodyStream = new MemoryStream();
      context.Response.Body = responseBodyStream;

      await _next(context);

      // Log the response
      context.Response.Body.Seek(0, SeekOrigin.Begin);
      var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
      context.Response.Body.Seek(0, SeekOrigin.Begin);
      _logger.LogInformation($"Outgoing Response: {context.Response.StatusCode} {responseBody}");

      await responseBodyStream.CopyToAsync(originalResponseBodyStream);
    }

    private async Task<string> ReadStreamAsync(Stream stream)
    {
      stream.Seek(0, SeekOrigin.Begin);
      using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
      var text = await reader.ReadToEndAsync();
      stream.Seek(0, SeekOrigin.Begin);
      return text;
    }
  }
}