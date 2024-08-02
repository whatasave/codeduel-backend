using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using User;

namespace Auth;

public class Controller(Config.Config config, Service service, User.Service userService) {
    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/refresh", RefreshToken);
        group.MapGet("/logout", Logout);
        group.MapPost("/validate_token", ValidateToken);
    }

    public Results<NoContent, UnauthorizedHttpResult, RedirectHttpResult> RefreshToken(HttpRequest request, HttpResponse response) {
        var refreshToken = request.Cookies[config.Auth.RefreshTokenCookieName];

        if (refreshToken == null) {
            response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue("logged_in", "false") {
                HttpOnly = false,
                Domain = config.Cookie.Domain,
                Path = config.Cookie.Path,
                Secure = config.Cookie.Secure,
                Expires = DateTimeOffset.UnixEpoch
            }.ToString());
            return TypedResults.Unauthorized();
        }

        RefreshTokenPayload jwt;
        try {
            jwt = service.ValidateRefreshToken(refreshToken);
        } catch (Exception) {
            response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue("logged_in", "false") {
                HttpOnly = false,
                Domain = config.Cookie.Domain,
                Path = config.Cookie.Path,
                Secure = config.Cookie.Secure,
                Expires = DateTimeOffset.UnixEpoch
            }.ToString());
            return TypedResults.Unauthorized();
        }

        var user = userService.FindById(jwt.UserId);
        if (user == null) return TypedResults.Unauthorized();

        var accessToken = service.GenerateAccessToken(user);

        response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue(config.Auth.AccessTokenCookieName, accessToken) {
            HttpOnly = config.Cookie.HttpOnly,
            Domain = config.Cookie.Domain,
            Path = config.Cookie.Path,
            Secure = config.Cookie.Secure,
            Expires = DateTimeOffset.Now.Add(config.Auth.AccessTokenExpires)
        }.ToString());

        if (request.Query.TryGetValue("redirect", out StringValues redirect)) {
            return TypedResults.Redirect(redirect!);
        }

        return TypedResults.NoContent();
    }

    public NoContent Logout(HttpRequest request, HttpResponse response) {
        foreach (var cookie in new string[] { config.Auth.RefreshTokenCookieName, config.Auth.AccessTokenCookieName, "logged_in", "oauth_state" }) {
            response.Cookies.Delete(cookie);
        }

        return TypedResults.NoContent();
    }

    [InternalAuth]
    public Results<Ok<LobbyUser>, UnauthorizedHttpResult> ValidateToken(VerifyTokenPayload payload) {
        try {
            var jwt = service.ValidateAccessToken(payload.Token);
            var user = userService.FindById(jwt.UserId);
            if (user == null) return TypedResults.Unauthorized();
            return TypedResults.Ok(new LobbyUser(user));
        } catch (Exception) {
            return TypedResults.Unauthorized();
        }
    }
}
