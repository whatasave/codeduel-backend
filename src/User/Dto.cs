namespace User;

public class UserDTO {
    public string? Name { get; init; }
    public required string Username { get; init; }
    public string? Avatar { get; init; }
    public string? BackgroundImage { get; init; }
    public string? Biography { get; init; }
    public string Role { get; init; } = "user";

    public UserDTO(User user) {
        this.Name = user.Name;
        this.Username = user.Username;
        this.Avatar = user.Avatar;
        this.BackgroundImage = user.BackgroundImage;
        this.Biography = user.Biography;
        this.Role = user.Role;
    }
}

public class UserListItemDTO {
    public string? Name { get; init; }
    public required string Username { get; init; }
    public string? Avatar { get; init; }
    public string Role { get; init; } = "user";

    public UserListItemDTO(User user) {
        this.Name = user.Name;
        this.Username = user.Username;
        this.Avatar = user.Avatar;
        this.Role = user.Role;
    }
}
