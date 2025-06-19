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
            var remoteIp = httpContext.Connection.RemoteIpAddress ?? 
                        (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedIps) 
                        ? IPAddress.Parse(forwardedIps.First().Split(':')[0]) : null);

            if (remoteIp == null)
            {
                _logger.LogWarning("IP-адреса клієнта не визначена");
                return false;
            }

            // Перевірка на localhost (IPv4/IPv6) та Docker
            if (IPAddress.IsLoopback(remoteIp) || 
                remoteIp.ToString() == "172.17.0.1" || 
                remoteIp.ToString().StartsWith("192.168."))
            {
                _logger.LogInformation($"Доступ адміна з IP: {remoteIp}");
                return true;
            }

            return false;
        }

public async Task InvokeAsync(HttpContext httpContext, ServiceSelectServer serviceSelectServer)
{
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

    if (Uri.TryCreate(server.IPAddress, UriKind.Absolute, out var redirectUri))
    {
        httpContext.Response.Redirect(redirectUri.ToString());
        _logger.LogInformation($"Redirected user to server: {server.IPAddress}");
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