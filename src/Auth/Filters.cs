namespace Auth;

public class AuthFilter(Config.Config config, Service service) : IEndpointFilter {
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        context.HttpContext.Request.Cookies.TryGetValue(config.Auth.AccessTokenCookieName, out var accessToken);
        if (accessToken == null) {
            return Results.Unauthorized();
        }

        var payload = service.ValidateAccessToken(accessToken);
        context.HttpContext.Items["auth"] = payload;
        return await next(context);
    }
}

public class OptionalAuthFilter(Config.Config config, Service service) : IEndpointFilter {
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        context.HttpContext.Request.Cookies.TryGetValue(config.Auth.AccessTokenCookieName, out var accessToken);
        if (accessToken != null) {
            var payload = service.ValidateAccessToken(accessToken);
            context.HttpContext.Items["auth"] = payload;
        }
        return await next(context);
    }
}

public class InternalAuthFilter(Config.Config config) : IEndpointFilter {
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        context.HttpContext.Request.Headers.TryGetValue(config.Auth.ServiceHeaderName, out var serviceToken);
        if (serviceToken.All(token => token != config.Auth.ServiceToken)) {
            return Results.Unauthorized();
        }
        return await next(context);
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
