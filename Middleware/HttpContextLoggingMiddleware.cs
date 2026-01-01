using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Middleware
{
    /// <summary>
    /// Middleware for comprehensive request/response logging and correlation tracking.
    /// 
    /// Features:
    /// - Logs all incoming requests with method, path, IP address
    /// - Tracks request duration and response status
    /// - Adds correlation ID to response headers for request tracing
    /// - Logs exceptions with full context
    /// - Skips logging for static assets and health checks to reduce noise
    /// </summary>
    public class HttpContextLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpContextLoggingMiddleware> _logger;

        // Paths to exclude from logging (static assets, health checks, etc.)
        private static readonly string[] ExcludedPaths = new[]
        {
            "/_framework",
            "/_content",
            "/css",
            "/js",
            "/images",
            "/favicon.ico",
            "/swagger",
            "/health"
        };

        public HttpContextLoggingMiddleware(RequestDelegate next, ILogger<HttpContextLoggingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip logging for excluded paths (static assets, etc.)
            if (ShouldSkipLogging(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var correlationId = context.TraceIdentifier;
            var method = context.Request.Method;
            var path = context.Request.Path + context.Request.QueryString;
            var ipAddress = GetClientIpAddress(context);

            // Add correlation ID to response headers for request tracing
            if (!context.Response.Headers.ContainsKey("X-Correlation-Id"))
            {
                context.Response.Headers["X-Correlation-Id"] = correlationId;
            }

            // Log request start
            _logger.LogInformation(
                "Request started: {Method} {Path} from {IpAddress} | CorrelationId: {CorrelationId}",
                method,
                path,
                ipAddress,
                correlationId
            );

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log exception with full context
                _logger.LogError(ex,
                    "Request failed: {Method} {Path} | Status: {StatusCode} | CorrelationId: {CorrelationId} | Error: {ErrorMessage}",
                    method,
                    path,
                    context.Response.StatusCode,
                    correlationId,
                    ex.Message
                );
                
                // Re-throw to let exception handling middleware process it
                throw;
            }
            finally
            {
                stopwatch.Stop();
                
                // Log request completion with performance metrics
                var logLevel = context.Response.StatusCode >= 500 ? LogLevel.Error
                             : context.Response.StatusCode >= 400 ? LogLevel.Warning
                             : LogLevel.Information;

                _logger.Log(logLevel,
                    "Request completed: {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | CorrelationId: {CorrelationId}",
                    method,
                    path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    correlationId
                );
            }
        }

        /// <summary>
        /// Determines if logging should be skipped for the given path
        /// </summary>
        private static bool ShouldSkipLogging(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            foreach (var excludedPath in ExcludedPaths)
            {
                if (path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the client IP address, considering proxy headers
        /// </summary>
        private static string GetClientIpAddress(HttpContext context)
        {
            // Check for forwarded IP (when behind a proxy/load balancer)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For can contain multiple IPs, take the first one
                var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (ips.Length > 0)
                {
                    return ips[0].Trim();
                }
            }

            // Check for real IP header
            var realIp = context.Request.Headers["X-Real-IP"].ToString();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // Fallback to connection remote IP
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}

