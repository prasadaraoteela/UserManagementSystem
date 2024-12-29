using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
public class TokenValidationMiddleware
{
  private readonly RequestDelegate _next;
  public TokenValidationMiddleware(RequestDelegate next)
  {
    _next = next;
  }
  public async Task Invoke(HttpContext context)
  {
    if (!context.Request.Headers.TryGetValue("Authorization", out var token))
    {
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      await context.Response.WriteAsync("Unauthorized");
      return;
    }
    try
    {
      await _next(context);
    }
    catch
    {
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      await context.Response.WriteAsync("Unauthorized");
    }
  }
}