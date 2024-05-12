using dotenv.net;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
builder.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
builder.Services.AddDbContext<UserContext>(o => o.UseMySql(connString, ServerVersion.AutoDetect(connString)));

var app = builder.Build();


app.MapGet("/", () => "Hello World!");

app.Run();

class DatabaseContext : DbContext {
    public DbSet<User> Users { get; set; }
    public DbSet<Auth> Auth { get; set; }

    public DatabaseContext(DbContextOptions<UserContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>()
            .Property(b => b.Username)
            .IsRequired();
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    //   optionsBuilder.UseMySQL("server=xxxxxxxx;port=3306;database=xxxxxxxx;user=xxxxxxxx;password=xxxxxxxx");
    //}

}
