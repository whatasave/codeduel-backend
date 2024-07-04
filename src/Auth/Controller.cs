using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Auth;

public class Controller(Config.Config config, Service service, User.Service userService) {
    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/refresh", RefreshToken);
        group.MapGet("/logout", Logout);
    }

    public IResult RefreshToken(HttpRequest request, HttpResponse response) {
        var refreshToken = request.Cookies[config.Auth.RefreshTokenCookieName];

        if (refreshToken == null) {
            response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue("logged_in", "false") {
                HttpOnly = false,
                Domain = config.Cookie.Domain,
                Path = config.Cookie.Path,
                Secure = config.Cookie.Secure,
                Expires = DateTimeOffset.UnixEpoch
            }.ToString());
            return Results.Unauthorized();
        }

        RefreshTokenPayload jwt;
        try {
            jwt = service.ValidateRefreshToken(refreshToken);
        }
        catch (Exception) {
            response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue("logged_in", "false") {
                HttpOnly = false,
                Domain = config.Cookie.Domain,
                Path = config.Cookie.Path,
                Secure = config.Cookie.Secure,
                Expires = DateTimeOffset.UnixEpoch
            }.ToString());
            return Results.Unauthorized();
        }

        var user = userService.FindById(jwt.UserId);
        if (user == null) return Results.Unauthorized();

        var accessToken = service.GenerateAccessToken(user);

        response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue(config.Auth.AccessTokenCookieName, accessToken) {
            HttpOnly = config.Cookie.HttpOnly,
            Domain = config.Cookie.Domain,
            Path = config.Cookie.Path,
            Secure = config.Cookie.Secure,
            Expires = DateTimeOffset.Now.Add(config.Auth.AccessTokenExpires)
        }.ToString());

        if (request.Query.TryGetValue("redirect", out StringValues redirect)) {
            return Results.Redirect(redirect!);
        }

        return Results.NoContent();
    }

    public IResult Logout(HttpRequest request, HttpResponse response) {
        foreach (var cookie in new string[] { config.Auth.RefreshTokenCookieName, config.Auth.AccessTokenCookieName, "logged_in", "oauth_state" }) {
            response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue(cookie, "") {
                HttpOnly = config.Cookie.HttpOnly,
                Domain = config.Cookie.Domain,
                Path = config.Cookie.Path,
                Secure = config.Cookie.Secure,
                Expires = DateTimeOffset.UnixEpoch
            }.ToString());
        }

        return Results.NoContent();
    }
}
