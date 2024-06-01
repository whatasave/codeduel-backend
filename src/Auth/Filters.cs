using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth;


public class AuthFilter(Config.Config config, Service service) : IActionFilter {
    public AuthFilter(Config.Config config, Database.DatabaseContext database) : this(config, new Service(config, database)) {}

    public void OnActionExecuting(ActionExecutingContext context) {
        context.HttpContext.Request.Cookies.TryGetValue(config.Auth.AccessTokenCookieName, out var accessToken);
        if (accessToken == null) {
            context.Result = new UnauthorizedResult();
            return;
        }

        var payload = service.ValidateAccessToken(accessToken);
        context.HttpContext.Items["auth"] = payload;
    }

    public void OnActionExecuted(ActionExecutedContext context) {}
}

public class OptionalAuthFilter(Config.Config config, Service service) : IActionFilter {
    public OptionalAuthFilter(Config.Config config, Database.DatabaseContext database) : this(config, new Service(config, database)) {}

    public void OnActionExecuting(ActionExecutingContext context) {
        context.HttpContext.Request.Cookies.TryGetValue(config.Auth.AccessTokenCookieName, out var accessToken);
        if (accessToken == null) return;

        var payload = service.ValidateAccessToken(accessToken);
        context.HttpContext.Items["auth"] = payload;
    }

    public void OnActionExecuted(ActionExecutedContext context) {}
}

public static class Extensions {
    public static AccessTokenPayload Auth(this HttpContext context) {
        return (AccessTokenPayload)context.Items["auth"]!;
    }

    public static AccessTokenPayload? OptionalAuth(this HttpContext context) {
        return (AccessTokenPayload?)context.Items["auth"];
    }
}
