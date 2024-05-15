using Microsoft.EntityFrameworkCore;

var config = Config.Config.FromEnv();
var builder = WebApplication.CreateBuilder(args);
var connString = config.Database.ConnectionString();
// builder.Services.AddDbContext<Database.DatabaseContext>(o => o.UseMySql(connString, ServerVersion.AutoDetect(connString)));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAllPolicy", builder => {
        builder
            .WithOrigins(config.Cors.Origins)
            .WithMethods(config.Cors.Methods)
            .WithHeaders(config.Cors.Headers);

        if (config.Cors.AllowCredentials) {
            builder.AllowCredentials();
        }
    });
});

var app = builder.Build();

// var database = app.Services.GetRequiredService<Database.DatabaseContext>();
var database = new Database.DatabaseContext(
    new DbContextOptionsBuilder<Database.DatabaseContext>()
        .UseMySql(connString, ServerVersion.AutoDetect(connString))
        .Options
);

var v1 = app.MapGroup("/v1");

v1.MapGet("/health", (HttpRequest request, HttpResponse response) => {
    response.StatusCode = StatusCodes.Status200OK;
    response.ContentType = "application/json";
    response.WriteAsync("{\"status\":\"ok\"}");
});

new User.Controller(database).SetupRoutes(v1.MapGroup("/user"));
new Lobby.Controller(database).SetupRoutes(v1.MapGroup("/lobby"));
new Challenge.Controller(database).SetupRoutes(v1.MapGroup("/challenge"));
new AuthGithub.Controller(database).SetupRoutes(v1.MapGroup("/auth/github"));
new Auth.Controller(config, database).SetupRoutes(v1.MapGroup("/auth"));

app.Run();
