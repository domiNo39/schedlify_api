namespace SchedlifyApi.Middleware;

public class TelegramAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _apiToken;

    public TelegramAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiToken = configuration["TelegramBot:ApiToken"];
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for non-API endpoints
        if (!context.Request.Path.StartsWithSegments("/tgusers") && 
            !context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        // Check for the authorization header
        if (!context.Request.Headers.TryGetValue("X-API-KEY", out var apiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Authorization header is missing");
            return;
        }

        // Verify the token
        if (apiKey.ToString() != _apiToken)
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Invalid API token");
            return;
        }

        // Continue with the request
        await _next(context);
    }
}

// Extension method for middleware registration
public static class TelegramAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseTelegramAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TelegramAuthMiddleware>();
    }
}