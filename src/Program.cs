using dotenv.net;
using dotenv.net.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
var connString = "server=" + EnvReader.GetStringValue("MARIADB_HOST") +
                ";port=" + EnvReader.GetStringValue("MARIADB_PORT") +
                ";database=" + EnvReader.GetStringValue("MARIADB_DATABASE") +
                ";user=" + EnvReader.GetStringValue("MARIADB_USER") +
                ";password=" + EnvReader.GetStringValue("MARIADB_PASSWORD");
// builder.Services.AddDbContext<DatabaseContext>(o => o.UseMySql(connString, ServerVersion.AutoDetect(connString)));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAllPolicy", builder => {
        builder
            .WithOrigins(EnvReader.GetStringValue("CORS_ORIGIN"))
            .WithMethods(EnvReader.GetStringValue("CORS_METHODS"))
            .WithHeaders(EnvReader.GetStringValue("CORS_HEADERS"));
        
        if (EnvReader.GetStringValue("CORS_CREDENTIALS") == "true") {
            builder.AllowCredentials();
        }
    });
});

var app = builder.Build();

// var database = app.Services.GetRequiredService<DatabaseContext>();
var database = new DatabaseContext(
    new DbContextOptionsBuilder<DatabaseContext>()
        .UseMySql(connString, ServerVersion.AutoDetect(connString))
        .Options
);

var v1 = app.MapGroup("/v1");

new User.Controller(database).SetupRoutes(v1.MapGroup("/user"));
new Lobby.Controller(database).SetupRoutes(v1.MapGroup("/lobby"));
new Challenge.Controller(database).SetupRoutes(v1.MapGroup("/challenge"));
new AuthGithub.Controller(database).SetupRoutes(v1.MapGroup("/auth/github"));

v1.MapGet("/health", (HttpRequest request, HttpResponse response) => {
    response.StatusCode = StatusCodes.Status200OK;
    response.ContentType = "application/json";
    response.WriteAsync("{\"status\":\"ok\"}");
});

v1.MapGet("/auth/refresh" , (HttpRequest request, HttpResponse response) => {
    var refreshToken = request.Cookies["refresh_token"];
    Console.WriteLine("--refresh token: " + refreshToken);

    if (refreshToken == null) {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        response.ContentType = "application/json";
        response.WriteAsync("{\"error\":\"missing refresh token\"}");
        return;
    }

    var jwtService = new Jwt.Service(database);

    JwtPayload? jwt = null;
    try {
        jwt = (JwtPayload?) jwtService.ValidateJwt(refreshToken);
    } catch (Exception) {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        response.ContentType = "application/json";
        response.WriteAsync("{\"error\":\"invalid refresh token\"}");
        return;
    }

    if (jwt == null) {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        response.ContentType = "application/json";
        response.WriteAsync("{\"error\":\"invalid refresh token\"}");
        return;
    }


    var userService = new User.Service(database);
    // var user = database.Users.Find(jwt.Sub);
    var user = userService.FindById(jwt.Sub);
    if (user == null) {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        response.ContentType = "application/json";
        response.WriteAsync("{\"error\":\"user not found\"}");
        return;
    }

    var accessTokenPayload = new User.User(1, "test") {
        Name = "test",
        Avatar = ""
    };

    var accessToken = jwtService.GenerateAccessToken(accessTokenPayload);

    response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue("access_token", accessToken) {
        HttpOnly = Environment.GetEnvironmentVariable("COOKIE_HTTP_ONLY") == "true",
        Domain = Environment.GetEnvironmentVariable("COOKIE_DOMAIN") ?? request.Host.Host,
        Path = Environment.GetEnvironmentVariable("COOKIE_PATH") ?? "/",
        Secure = Environment.GetEnvironmentVariable("COOKIE_SECURE") == "true",
        Expires = DateTimeOffset.Now.AddMinutes(int.Parse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRES_IN_MINUTES") ?? "15"))
    }.ToString());

    var redirect = request.Query["redirect"];

    if (redirect != "") {
        response.StatusCode = StatusCodes.Status301MovedPermanently;
        response.Redirect(redirect!);
    } else {
        response.StatusCode = StatusCodes.Status200OK;
        response.ContentType = "application/json";
        response.WriteAsync("{\"access_token\":\"" + accessToken + "\"}");
    }
    return;
});

v1.MapGet("/auth/logout", (HttpRequest request, HttpResponse response) => {
    foreach (var cookie in new string[] { "refresh_token", "access_token", "oauth_state" }) {
        response.Headers.Append(HeaderNames.SetCookie, new SetCookieHeaderValue(cookie, "") {
            HttpOnly = Environment.GetEnvironmentVariable("COOKIE_HTTP_ONLY") == "true",
            Domain = Environment.GetEnvironmentVariable("COOKIE_DOMAIN") ?? request.Host.Host,
            Path = Environment.GetEnvironmentVariable("COOKIE_PATH") ?? "/",
            Secure = Environment.GetEnvironmentVariable("COOKIE_SECURE") == "true",
            Expires = DateTimeOffset.Now.AddDays(-1)
        }.ToString());
    }

    response.StatusCode = StatusCodes.Status200OK;
    response.ContentType = "application/json";
    response.WriteAsync("{\"status\":\"ok\"}");
});

app.Run();

class JwtPayload {
    public int Sub { get; set; }
    public DateTime ExpireAt { get; set; }
}
