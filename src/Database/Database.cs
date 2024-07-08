using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options) {
    public DbSet<User.Entity> Users { get; set; }
    public DbSet<Auth.Entity> Authentications { get; set; }
    public DbSet<Auth.RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<Game.Entity> Games { get; set; }
    public DbSet<Game.UserEntity> GameUsers { get; set; }
    public DbSet<Game.ModeEntity> GameModes { get; set; }
    public DbSet<Challenge.Entity> Challenges { get; set; }
    public DbSet<Challenge.TestCaseEntity> TestCases { get; set; }
    public DbSet<Permissions.Entity> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // change all names to snake_case
        foreach (var entity in modelBuilder.Model.GetEntityTypes()) {
            entity.SetTableName(ToSnakeCase(entity.GetTableName()));
            foreach (var property in entity.GetProperties()) {
                property.SetColumnName(ToSnakeCase(property.GetColumnName()));
            }
            foreach (var key in entity.GetKeys()) {
                key.SetName(ToSnakeCase(key.GetName()));
            }
            foreach (var key in entity.GetForeignKeys()) {
                key.SetConstraintName(ToSnakeCase(key.GetConstraintName()));
            }
            foreach (var index in entity.GetIndexes()) {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
            }
        }
    }

    private static string? ToSnakeCase(string? text) {
        if (text == null) {
            return null;
        }
        if (text.Length < 2) {
            return text.ToLowerInvariant();
        }
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));
        for (int i = 1; i < text.Length; ++i) {
            char c = text[i];
            if (char.IsUpper(c)) {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}