using Microsoft.AspNetCore.Http.Features;

namespace Auth;

[AttributeUsage(AttributeTargets.Method)]
public class AuthAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method)]
public class OptionalAuthAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method)]
public class InternalAuthAttribute : Attribute;

public class Middleware(Config.Config config, Service service) {
    public Task Auth(HttpContext context, Func<Task> next) {
        var hasAttribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata
                .Any(m => m is AuthAttribute) ?? false;
        if (!hasAttribute) return next();

        context.Request.Cookies.TryGetValue(config.Auth.AccessTokenCookieName, out var accessToken);
        if (accessToken == null) {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.WriteAsync("Unauthorized");
            return Task.CompletedTask;
        }

        var payload = service.ValidateAccessToken(accessToken);
        context.Items["auth"] = payload;
        return next();
    }

    public Task OptionalAuth(HttpContext context, Func<Task> next) {
        var hasAttribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata
                .Any(m => m is OptionalAuthAttribute) ?? false;
        if (!hasAttribute) return next();

        context.Request.Cookies.TryGetValue(config.Auth.AccessTokenCookieName, out var accessToken);
        if (accessToken != null) {
            var payload = service.ValidateAccessToken(accessToken);
            context.Items["auth"] = payload;
        }
        return next();
    }

    public Task InternalAuth(HttpContext context, Func<Task> next) {
        var hasAttribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata
                .Any(m => m is InternalAuthAttribute) ?? false;
        if (!hasAttribute) return next();

        context.Request.Headers.TryGetValue(config.Auth.ServiceHeaderName, out var serviceToken);
        if (serviceToken.All(token => token != config.Auth.ServiceToken)) {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.WriteAsync("Unauthorized");
            return Task.CompletedTask;
        }
        return next();
    }
}
public static class Extensions {
    public static AccessTokenPayload Auth(this HttpContext context) {
        return (AccessTokenPayload)context.Items["auth"]!;
    }

    public static AccessTokenPayload? OptionalAuth(this HttpContext context) {
        return (AccessTokenPayload?)context.Items["auth"];
    }
}
