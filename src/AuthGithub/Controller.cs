using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace AuthGithub;

public class Controller(Service service) {
    public Controller(DatabaseContext database) : this(new Service(database)) {}
    
    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/", Login);
        group.MapGet("/callback", Callback);
    }

    public void Login(HttpRequest request, HttpResponse response) {
        var state = Guid.NewGuid().ToString();

        response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue("oauth_state", state) {
            HttpOnly = true,
            // Domain = request.Host.Value,
            Domain = "127.0.0.1",
            Path = "/",
            // Path = "/v1/auth/github",
            Expires = DateTimeOffset.Now.AddSeconds(60)
        }.ToString());


        var githubAuthUrl = "https://github.com/login/oauth/authorize";
        var query = new QueryBuilder() {
            { "client_id", Environment.GetEnvironmentVariable("AUTH_GITHUB_CLIENT_ID") ?? "null" },
            { "redirect_uri", Environment.GetEnvironmentVariable("AUTH_GITHUB_CLIENT_CALLBACK_URL") ?? "http://127.0.0.1:5000/v1/auth/github/callback" },
            { "state", state },
            { "scope", "user:email" },
            { "allow_signup", "true" }
        };
        var url = githubAuthUrl + query;
        response.StatusCode = StatusCodes.Status307TemporaryRedirect;
        // response.Headers.Append("Location", url);
        response.Redirect(url);
        return;
    }
    
    public void Callback(
        [FromQuery(Name = "code")] string code,
        [FromQuery(Name = "state")] string state,
        HttpRequest request,
        HttpResponse response
    ) {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state)) {
            _ = new StatusCodeResult(StatusCodes.Status400BadRequest);
            Console.Error.WriteLine("[Auth Github] Missing code or state");
            return;
        }

        var cookie = request.Cookies["oauth_state"];
        if (cookie != state) {
            _ = new StatusCodeResult(StatusCodes.Status400BadRequest);
            Console.Error.WriteLine("[Auth Github] state mismatch");
            return;
        }

        var ghAccessToken = service.GetAccessToken(code, state);
        if (ghAccessToken == null) {
            _ = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            Console.Error.WriteLine("[Auth Github] Failed to get access token");
            return;
        }

        var userData = service.GetUserData(ghAccessToken);
        if (userData == null) {
            _ = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            Console.Error.WriteLine("[Auth Github] Failed to get user data");
            return;
        }

        var user = service.GetUserByProviderAndId("github", userData.Id);

        if (user == null) {
            if (userData.Email == null) {
                var primaryEmail = service.GetUserPrimaryEmail(ghAccessToken);
                if (primaryEmail == null) {
                    _ = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    Console.Error.WriteLine("[Auth Github] Failed to get user emails");
                    return;
                }

                userData = userData with { Email = primaryEmail };
            }

            user = service.CreateAuthUser(userData);
        }

        var refreshToken = service.GenerateTokens(user)[1];

        Console.WriteLine("[Auth Github] User logged in: " + user.Username);
        Console.WriteLine("[Auth Github] Refresh token: " + refreshToken);
        // storing tokens in cookies
        response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue("refresh_token", refreshToken) {
            HttpOnly = true,
            // Domain = request.Host.Host,
            Domain = "127.0.0.1",
            Path = "/",
            Expires = DateTimeOffset.Now.AddDays(30)
        }.ToString());

        var frontendRedirect = request.Cookies["return_to"] ?? Environment.GetEnvironmentVariable("FRONTEND_URL_AUTH_CALLBACK");
        response.Redirect("http://127.0.0.1:5000/v1/auth/refresh?redirect=" + frontendRedirect);
        _ = new StatusCodeResult(StatusCodes.Status308PermanentRedirect);
        return;
    }
}
