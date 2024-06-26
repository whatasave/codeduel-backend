using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Auth.Github;

public class Controller(Config.Config config, Service service, Auth.Service authService) {

    public Controller(Config.Config config, Database.DatabaseContext database) : this(
        config,
        new Service(config, database),
        new Auth.Service(config, database)
    ) { }

    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/", Login);
        group.MapGet("/callback", Callback);
    }

    public IResult Login(HttpRequest request, HttpResponse response) {
        var state = Guid.NewGuid().ToString();

        response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue("oauth_state", state) {
            HttpOnly = config.Cookie.HttpOnly,
            Domain = config.Cookie.Domain,
            Path = config.Cookie.Path,
            Secure = config.Cookie.Secure,
            Expires = DateTimeOffset.Now.AddSeconds(60)
        }.ToString());

        var url = "https://github.com/login/oauth/authorize" + new QueryBuilder() {
            { "client_id", config.Auth.Github.ClientId },
            { "redirect_uri", config.Auth.Github.CallbackUrl },
            { "state", state },
            { "scope", "user:email" },
            { "allow_signup", "true" }
        };
        return Results.Redirect(url);
    }

    public async Task<IResult> Callback([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state, HttpRequest request, HttpResponse response) {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state)) {
            Console.WriteLine("[Auth Github] Missing code or state");
            return Results.Redirect(config.Auth.LoginRedirect + "?error=5001");
        }

        var cookie = request.Cookies["oauth_state"];
        if (cookie != state) {
            Console.WriteLine("[Auth Github] state mismatch");
            return Results.Redirect(config.Auth.LoginRedirect + "?error=5002");
        }

        var ghAccessToken = await service.GetAccessToken(code, state);
        if (ghAccessToken == null || ghAccessToken.IsEmpty) {
            Console.WriteLine("[Auth Github] Failed to get access token");
            return Results.Redirect(config.Auth.LoginRedirect + "?error=5003");
        }

        var userData = await service.GetUserData(ghAccessToken.AccessToken);
        if (userData == null) {
            Console.WriteLine("[Auth Github] Failed to get user data");
            return Results.Redirect(config.Auth.LoginRedirect + "?error=5004");
        }

        var user = service.GetUserByProviderId(userData.Id) ?? service.Create(userData);
        var tokens = authService.GenerateTokens(user);

        // Setting Cookies
        response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue(config.Auth.RefreshTokenCookieName, tokens.RefreshToken) {
            HttpOnly = config.Cookie.HttpOnly,
            Domain = config.Cookie.Domain,
            Path = config.Cookie.Path,
            Secure = config.Cookie.Secure,
            Expires = DateTimeOffset.Now.Add(config.Auth.RefreshTokenExpires)
        }.ToString());

        response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue(config.Auth.AccessTokenCookieName, tokens.AccessToken) {
            HttpOnly = config.Cookie.HttpOnly,
            Domain = config.Cookie.Domain,
            Path = config.Cookie.Path,
            Secure = config.Cookie.Secure,
            Expires = DateTimeOffset.Now.Add(config.Auth.AccessTokenExpires)
        }.ToString());

        response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue("logged_in", "true") {
            HttpOnly = config.Cookie.HttpOnly,
            Domain = config.Cookie.Domain,
            Path = config.Cookie.Path,
            Secure = config.Cookie.Secure,
            Expires = DateTimeOffset.Now.Add(config.Auth.AccessTokenExpires)
        }.ToString());

        response.Cookies.Delete("oauth_state");

        // Handling Redirect
        var returnTo = request.Cookies["return_to"];
        response.Cookies.Delete("return_to");

        // check if non null, if the domain is in the list of allowed domains and if it is a valid url
        if (!string.IsNullOrEmpty(returnTo) && config.Auth.AllowedDomains.Contains(new Uri(returnTo).Host)) {
            return Results.Redirect(returnTo, true);
        }

        return Results.Redirect(config.Auth.LoginRedirect, true);
    }
}
