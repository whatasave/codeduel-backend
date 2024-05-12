
using System.ComponentModel.DataAnnotations;

public class User {
    [Key]
    public required int Id { get; set; }
    public string? Name { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? Avatar { get; set; }
    public string? BackgroundImage { get; set; }
    public string? Biography { get; set; }
    public string Role { get; set; } = "user";
    public required DateTime CreatedAt { get; init; } = DateTime.Now;
    public required DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class Auth {
    [Key]
    public required int Id { get; set; }
    public required string UserId { get; set; }
    public required string Provider { get; set; }
    public required string ProviderId { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.Now;
    public required DateTime UpdatedAt { get; set; } = DateTime.Now;
}
