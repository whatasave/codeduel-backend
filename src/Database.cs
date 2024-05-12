using Microsoft.EntityFrameworkCore;
public class DatabaseContext : DbContext {
    public DbSet<User.User> Users { get; set; }
    public DbSet<User.Auth> Auth { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User.User>()
            .Property(b => b.Username)
            .IsRequired();
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    //   optionsBuilder.UseMySQL("server=xxxxxxxx;port=3306;database=xxxxxxxx;user=xxxxxxxx;password=xxxxxxxx");
    //}
}
