using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace MonyLoop.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
                await HandleNotFoundEndPointAsync(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while processing the request.");

                var problem = new ProblemDetails
                {
                    Title = "Error while processing the HTTP request",
                    Detail = _env.IsDevelopment() ? ex.Message : "Please contact support if the problem persists.",
                    Instance = httpContext.Request.Path,
                    Status = ex switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                        _ => StatusCodes.Status500InternalServerError
                    }
                };

                httpContext.Response.ContentType = "application/problem+json";
                httpContext.Response.StatusCode = problem.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(problem);
            }
        }

        private static async Task HandleNotFoundEndPointAsync(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound && !httpContext.Response.HasStarted)
            {
                var problem = new ProblemDetails
                {
                    Title = "Error while processing the HTTP request - Endpoint Not Found",
                    Detail = $"Endpoint {httpContext.Request.Path} was not found.",
                    Status = StatusCodes.Status404NotFound,
                    Instance = httpContext.Request.Path
                };

                httpContext.Response.ContentType = "application/problem+json";
                await httpContext.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
