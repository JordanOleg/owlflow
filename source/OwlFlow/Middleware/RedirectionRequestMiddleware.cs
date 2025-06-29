using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlFlow.Models;
using OwlFlow.Service;
using System.Net.Sockets;
using System.Net;

namespace OwlFlow.Middleware
{
    public class RedirectionRequestMiddleware
    {
        private readonly ILogger<RedirectionRequestMiddleware> _logger;
        private readonly RequestDelegate _requestDelegate;
        public RedirectionRequestMiddleware(RequestDelegate requestDelegate, ILogger<RedirectionRequestMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }
        private bool IsAdmin(HttpContext httpContext)
        {
            return httpContext.Session.GetString("IsAdmin") == "true";
        }

        public async Task InvokeAsync(HttpContext httpContext, ServiceSelectServer serviceSelectServer)
        {
            var path = httpContext.Request.Path;
    
    // Сторінки, які доступні всім
            if (path.StartsWithSegments("/Login") || 
                path.StartsWithSegments("/css") || 
                path.StartsWithSegments("/js") ||
                path.StartsWithSegments("/lib") ||
                path.StartsWithSegments("/images"))
            {
                await _requestDelegate.Invoke(httpContext);
                return;
            }
            if (IsAdmin(httpContext))
            {
                await _requestDelegate.Invoke(httpContext);
                return;
            }

            if (httpContext.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started, skipping redirect.");
                return;
            }
            
            Server server = serviceSelectServer.GetOptimalServer();
            if (server == null || string.IsNullOrEmpty(server.IPAddress))
            {
                httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                _logger.LogError("No available servers found.");
                return;
            }

            if (Uri.TryCreate($"http://{server.IPAddress}/", UriKind.Absolute, out Uri? uri))
            {
                httpContext.Response.Redirect(uri.ToString());
                _logger.LogInformation($"Redirected server: {server.IPAddress}");
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                _logger.LogError($"Invalid server IP: {server.IPAddress}");
            }
        }
    }
    public static class RedirectionRequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseRedirectRequestMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RedirectionRequestMiddleware>();
        }
    }
}