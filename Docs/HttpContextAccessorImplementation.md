# IHttpContextAccessor Implementation Guide

## Overview

This document describes the implementation of `IHttpContextAccessor` in the EyeHospitalPOS application with DevExpress Blazor. The implementation provides access to HTTP context information from anywhere in the application, including services, components, and middleware.

## Components

### 1. HttpContextService (`Services/HttpContextService.cs`)

A wrapper service that provides easy access to HTTP context information. This service encapsulates `IHttpContextAccessor` and provides convenient methods for common operations.

**Key Features:**
- Access to current user claims (ID, username, email, role)
- Request/response information (headers, cookies, query parameters)
- Client information (IP address, user agent, device detection)
- URL information (base URL, full URL, path)
- Cookie management (get, set, delete)

**Usage Example:**
```csharp
[Inject] private HttpContextService HttpContextService { get; set; }

// Get current user ID
var userId = HttpContextService.GetUserId();

// Get client IP address
var ipAddress = HttpContextService.GetClientIpAddress();

// Check if user is authenticated
if (HttpContextService.IsAuthenticated)
{
    // Perform authenticated operations
}
```

### 2. ApiClientService Integration

The `ApiClientService` base class now uses `HttpContextService` to:
- Automatically get JWT tokens from HTTP context when sessionStorage is unavailable
- Add correlation IDs to API requests for request tracking
- Access request headers and context information

### 3. HttpContextLoggingMiddleware (`Middleware/HttpContextLoggingMiddleware.cs`)

A custom middleware that demonstrates using HTTP context for:
- Request logging with correlation IDs
- Performance tracking (request duration)
- Error logging with context information

## Registration in Startup.cs

```csharp
// Register IHttpContextAccessor
services.AddHttpContextAccessor();

// Register HttpContextService wrapper
services.AddScoped<HttpContextService>();

// Configure HttpClient with HttpContextAccessor support
services.AddHttpClient("ApiClient", (sp, client) =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;
    
    if (request != null)
    {
        var baseUrl = $"{request.Scheme}://{request.Host}";
        client.BaseAddress = new Uri(baseUrl);
    }
});
```

## Use Cases

### 1. Authentication & Authorization
```csharp
// Get current user information
var userId = HttpContextService.GetUserId();
var userRole = HttpContextService.GetUserRole();

// Check authentication status
if (HttpContextService.IsAuthenticated)
{
    // User is logged in
}
```

### 2. Request Tracking & Logging
```csharp
// Get correlation ID for request tracking
var correlationId = HttpContextService.GetCorrelationId();

// Log request information
_logger.LogInformation("Request from {IpAddress} by {UserId}", 
    HttpContextService.GetClientIpAddress(), 
    HttpContextService.GetUserId());
```

### 3. Cookie Management
```csharp
// Get cookie value
var theme = HttpContextService.GetCookie("theme");

// Set cookie
HttpContextService.SetCookie("theme", "dark", new CookieOptions
{
    HttpOnly = false,
    Secure = true,
    SameSite = SameSiteMode.Strict,
    Expires = DateTimeOffset.UtcNow.AddDays(30)
});

// Delete cookie
HttpContextService.DeleteCookie("theme");
```

### 4. Query Parameters & Headers
```csharp
// Get query parameter
var page = HttpContextService.GetQueryParameter("page");

// Get request header
var userAgent = HttpContextService.GetUserAgent();
var acceptLanguage = HttpContextService.GetRequestHeader("Accept-Language");
```

### 5. Device Detection
```csharp
// Check if request is from mobile device
if (HttpContextService.IsMobileDevice())
{
    // Optimize UI for mobile
}
```

## Best Practices

1. **Always check for null**: HTTP context may not be available in all scenarios (background services, signalR hubs, etc.)
   ```csharp
   if (HttpContextService.HttpContext != null)
   {
       // Safe to use HTTP context
   }
   ```

2. **Use HttpContextService instead of direct IHttpContextAccessor**: The wrapper provides better error handling and convenience methods.

3. **Be aware of scope**: HTTP context is only available during request processing. Don't try to access it from background services or outside request scope.

4. **Use correlation IDs**: Always include correlation IDs in logs and API requests for better traceability.

## Integration with DevExpress Blazor

The implementation works seamlessly with DevExpress Blazor components. HTTP context information can be accessed from:
- Blazor components (via dependency injection)
- DevExpress services
- Custom middleware
- API controllers

## Notes

- `IHttpContextAccessor` is registered as a singleton, but `HttpContextService` is registered as scoped to match the request lifecycle.
- HTTP context is not available during prerendering. Always check for null or use try-catch blocks.
- For Blazor Server, HTTP context is available in components. For Blazor WebAssembly, use JSRuntime for client-side operations.

