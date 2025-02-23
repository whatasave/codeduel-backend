using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => {
        options.InvalidModelStateResponseFactory = context => {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => new { Field = e.Key, Errors = e.Value.Errors.Select(err => err.ErrorMessage) })
                .ToList();

            Console.WriteLine("Model Binding Errors:");
            foreach (var error in errors) {
                Console.WriteLine($"Field: {error.Field}, Errors: {string.Join(", ", error.Errors)}");
            }

            return new BadRequestObjectResult(errors);
        };
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

app.Use(async (context, next) => {
    // print request body
    var originalRequestBody = context.Request.Body;
    using var memoryStream2 = new MemoryStream();
    await context.Request.Body.CopyToAsync(memoryStream2);
    memoryStream2.Seek(0, SeekOrigin.Begin);
    string requestBody = await new StreamReader(memoryStream2).ReadToEndAsync();
    memoryStream2.Seek(0, SeekOrigin.Begin);
    context.Request.Body = memoryStream2;
    Console.WriteLine("Request: " + context.Request.Method + " " + context.Request.Path + " " + requestBody);

    var originalBodyStream = context.Response.Body;
    using var memoryStream = new MemoryStream();
    context.Response.Body = memoryStream;

    await next.Invoke();

    memoryStream.Position = 0;
    string responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
    memoryStream.Position = 0;

    await memoryStream.CopyToAsync(originalBodyStream);
    context.Response.Body = originalBodyStream;
    Console.WriteLine("Response: " + context.Response.StatusCode + " " + responseBody);
});

app.Run();

record HealthCheck(string Status);

class JsonSnakeCaseNamingPolicy : JsonNamingPolicy {
    public override string ConvertName(string name) {
        return string.Concat(
            name.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())
        ).ToLower();
    }
}

public class ValidationLoggingFilter : IActionFilter {
    private readonly ILogger<ValidationLoggingFilter> _logger;

    public ValidationLoggingFilter(ILogger<ValidationLoggingFilter> logger) {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context) {
        if (!context.ModelState.IsValid) {
            var errors = context.ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .Select(ms => new { Field = ms.Key, Errors = ms.Value.Errors.Select(e => e.ErrorMessage) })
                .ToList();

            _logger.LogWarning("Validation failed: {@Errors}", errors);

            // Restituisci un 400 con gli errori di validazione nel corpo della risposta
            context.Result = new BadRequestObjectResult(errors);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}