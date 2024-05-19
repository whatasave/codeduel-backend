using Microsoft.EntityFrameworkCore;

var config = Config.Config.FromEnv();
var builder = WebApplication.CreateBuilder(args);
var connString = config.Database.ConnectionString();
var database = new Database.DatabaseContext(
    new DbContextOptionsBuilder<Database.DatabaseContext>()
        .UseMySql(connString, ServerVersion.AutoDetect(connString))
        .Options
);

builder.Services.AddScoped(_ => database);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

var v1 = app.MapGroup("/v1");

v1.MapGet("/health", () => Results.Ok(new { status = "ok" }));

new User.Controller(database).SetupRoutes(v1.MapGroup("/user"));
new Lobby.Controller(database).SetupRoutes(v1.MapGroup("/lobby"));
new Challenge.Controller(database).SetupRoutes(v1.MapGroup("/challenge"));
new Auth.Github.Controller(config, database).SetupRoutes(v1.MapGroup("/auth/github"));
new Auth.Controller(config, database).SetupRoutes(v1.MapGroup("/auth"));
new Permissions.Controller(database).SetupRoutes(v1.MapGroup("/user/permissions"));

app.Run();
