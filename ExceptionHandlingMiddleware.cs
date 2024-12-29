using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace UserManagementSystem
{
  public class ExceptionHandlingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unhandled exception has occurred.");
        await HandleExceptionAsync(context);
      }
    }

    private static Task HandleExceptionAsync(HttpContext context)
    {
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      var response = new { error = "Internal server error." };
      return context.Response.WriteAsJsonAsync(response);
    }
  }
}