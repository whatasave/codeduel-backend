using Microsoft.EntityFrameworkCore;
public class DatabaseContext : DbContext {
    public DbSet<User.Entity> Users { get; set; }
    public DbSet<AuthGithub.Entity> Auths { get; set; }
    public DbSet<Lobby.Entity> Lobbies { get; set; }
    public DbSet<Lobby.UserEntity> LobbiesUsers { get; set; }
    public DbSet<Lobby.ModeEntity> LobbiesModes { get; set; }
    public DbSet<Challenge.Entity> Challenges { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    //protected override void OnModelCreating(ModelBuilder modelBuilder) {
    //    modelBuilder.Entity<User.Entity>()
    //        .Property(b => b.Username)
    //        .IsRequired();
    //}

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    //   optionsBuilder.UseMySQL("server=xxxxxxxx;port=3306;database=xxxxxxxx;user=xxxxxxxx;password=xxxxxxxx");
    //}
}
