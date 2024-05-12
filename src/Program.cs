using dotenv.net;
using Microsoft.EntityFrameworkCore;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
//builder.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;

var connString = "server=" + Environment.GetEnvironmentVariable("DB_HOST") + ";port=" + Environment.GetEnvironmentVariable("DB_PORT") + ";database=" + Environment.GetEnvironmentVariable("DB_DATABASE") + ";user=" + Environment.GetEnvironmentVariable("DB_USERNAME") + ";password=" + Environment.GetEnvironmentVariable("DB_PASSWORD");
builder.Services.AddDbContext<DatabaseContext>(o => o.UseMySql(connString, ServerVersion.AutoDetect(connString)));

var app = builder.Build();


app.MapGet("/user", User.controller);
app.MapGet("/lobby", Lobby.controller);
app.MapGet("/challenge", Challenge.controller);

app.Run();

class DatabaseContext : DbContext {
    public DbSet<User> Users { get; set; }
    public DbSet<Auth> Auth { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>()
            .Property(b => b.Username)
            .IsRequired();
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    //   optionsBuilder.UseMySQL("server=xxxxxxxx;port=3306;database=xxxxxxxx;user=xxxxxxxx;password=xxxxxxxx");
    //}

}
