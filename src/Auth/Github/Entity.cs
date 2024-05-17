namespace Auth.Github;

public class Entity {
    public required int Id { get; set; }
    public required int UserId { get; set; }
    public required string Provider { get; set; }
    public required int ProviderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
