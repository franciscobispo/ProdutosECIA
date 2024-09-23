using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net;

namespace ProdutosECIA.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            httpContext.Request.EnableBuffering();
            var bodyAsText = await new System.IO.StreamReader(httpContext.Request.Body).ReadToEndAsync();
            httpContext.Request.Body.Position = 0;

            var url = httpContext.Request.GetDisplayUrl();

            _logger.LogError($"Message: {ex.Message}");
            _logger.LogError($"url: {url}");

            if (!string.IsNullOrEmpty(bodyAsText))
                _logger.LogError($"body: {bodyAsText}");

            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var result = new
        {
            error = ex.Message,
            stackTrace = ex.StackTrace,
            details = ex.InnerException?.Message
        };

        return context.Response.WriteAsJsonAsync(result);
    }
}