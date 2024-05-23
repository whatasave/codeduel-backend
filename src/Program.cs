using Microsoft.AspNetCore.Diagnostics;
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

app.UseExceptionHandler(builder => {
    builder.Run(async context => {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>()!;
        var exception = exceptionHandlerPathFeature.Error;
        var errorId = Guid.NewGuid();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        Console.WriteLine($"Error ID: {errorId}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync($"Internal Server Error <{errorId}>");
    });
});

var v1 = app.MapGroup("/v1");

v1.MapGet("/health", () => Results.Ok(new { status = "ok" }));

new User.Controller(database).SetupRoutes(v1.MapGroup("/user"));
new Game.Controller(database).SetupRoutes(v1.MapGroup("/lobby"));
new Challenge.Controller(config, database).SetupRoutes(v1.MapGroup("/challenge"));
new Auth.Github.Controller(config, database).SetupRoutes(v1.MapGroup("/auth/github"));
new Auth.Controller(config, database).SetupRoutes(v1.MapGroup("/auth"));
new Permissions.Controller(database).SetupRoutes(v1.MapGroup("/user/permissions"));

app.Run();
