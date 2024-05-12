public class User {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
    public string BackgroundImage { get; set; }
    public string Biography { get; set; }
    public string Role { get; set; }
    public Datetime CreatedAt { get; set; }
    public Datetime UpdatedAt { get; set; }
}

public class Auth {
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Provider { get; set; }
    public string ProviderId { get; set; }
    public Datetime CreatedAt { get; set; }
    public Datetime UpdatedAt { get; set; }
}

