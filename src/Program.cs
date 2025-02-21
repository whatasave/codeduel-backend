using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var config = Config.Config.FromEnv();
var builder = WebApplication.CreateBuilder(args);
var connString = config.Database.ConnectionString();
var database = () => new Database.DatabaseContext(
    new DbContextOptionsBuilder<Database.DatabaseContext>()
        .UseMySql(connString, ServerVersion.AutoDetect(connString))
        .Options
);
var controller = new Controller(config, database);

builder.Services.AddScoped(_ => database());
builder.Services.AddTransient(_ => config);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SupportNonNullableReferenceTypes();
    options.AddSchemaFilterInstance(new RemoveUndefinedSchemaFilter());
    options.AddOperationFilterInstance(new AuthOperationFilter());
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder => {
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

if (config.Swagger) {
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

app.UseCors();

var v1 = app.MapGroup("/v1");

v1.MapGet("/health", () => new HealthCheck("ok"));

controller.SetupRoutes(v1);
controller.SetupMiddleware(app);

app.Run();

record HealthCheck(string Status);

class JsonSnakeCaseNamingPolicy : JsonNamingPolicy {
    public override string ConvertName(string name) {
        return string.Concat(
            name.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())
        ).ToLower();
    }
}