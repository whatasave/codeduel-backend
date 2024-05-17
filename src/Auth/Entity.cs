namespace Auth;

public class Entity {
    public required int Id { get; set; }
    public required User.Entity User { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
