using dotenv.net;
using dotenv.net.Utilities;
using Microsoft.EntityFrameworkCore;

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
            .WithOrigins(EnvReader.GetStringValue("FRONTEND_URL"))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        //.AllowAnyMethod(EnvReader.GetStringValue("CORS_METHODS"))
        //.AllowAnyHeader(EnvReader.GetStringValue("CORS_HEADERS"))
        //.AllowCredentials(EnvReader.GetStringValue("CORS_CREDENTIALS"));
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

app.Run();
