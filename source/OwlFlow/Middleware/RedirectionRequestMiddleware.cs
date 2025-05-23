using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlFlow.Models;
using OwlFlow.Service;

namespace OwlFlow.Middleware
{
    public class RedirectionRequestMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        public RedirectionRequestMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }
        private bool IsAdmin(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }
        public async Task InvokeAsync(HttpContext httpContext, ServiceSelectServer serviceSelectServer)
        {
            if (IsAdmin(httpContext))
            {
                await _requestDelegate.Invoke(httpContext);
            }
            Server server = serviceSelectServer.GetOptimalServer();
            httpContext.Response.Redirect(server.IPAddress); // TODO: Fix URL
            return; 
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