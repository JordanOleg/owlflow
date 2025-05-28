using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlFlow.Models;
using OwlFlow.Service;
using System.Net.Sockets;

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
            if (httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString().Contains("127.0.0.0"))
            {
                var result = httpContext.Connection.LocalIpAddress;
                _logger.LogInformation($"User IP {result}");
                return true;
            }
            else return false;
        }
        public async Task InvokeAsync(HttpContext httpContext, ServiceSelectServer serviceSelectServer)
        {
            if (IsAdmin(httpContext))
            {
                await _requestDelegate.Invoke(httpContext);
            }
            else
            {
                if (!httpContext.Response.HasStarted)
                {
                    Server server = serviceSelectServer.GetOptimalServer();
                    httpContext.Response.Redirect(server.IPAddress); // TODO: Fix URL
                    _logger.LogInformation($"Redirect user in server {server.URI}");
                }
                return; 
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