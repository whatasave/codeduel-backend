using dotenv.net;
using Microsoft.EntityFrameworkCore;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
var connString = "server=" + Environment.GetEnvironmentVariable("DB_HOST") +
                ";port=" + Environment.GetEnvironmentVariable("DB_PORT") +
                ";database=" + Environment.GetEnvironmentVariable("DB_DATABASE") +
                ";user=" + Environment.GetEnvironmentVariable("DB_USERNAME") +
                ";password=" + Environment.GetEnvironmentVariable("DB_PASSWORD");
// builder.Services.AddDbContext<DatabaseContext>(o => o.UseMySql(connString, ServerVersion.AutoDetect(connString)));

var app = builder.Build();
var database = app.Services.GetRequiredService<DatabaseContext>();

var v1 = app.MapGroup("/v1");

new User.Controller(database).SetupRoutes(v1.MapGroup("/user"));
new Lobby.Controller(database).Setup(v1.MapGroup("/lobby"));
new Challenge.Controller(database).Setup(v1.MapGroup("/challenge"));
new AuthGithub.Controller(database).Setup(v1.MapGroup("/auth/github"));

app.Run();
