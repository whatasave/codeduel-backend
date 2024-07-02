namespace User;

public record User(
    int Id,
    string Username,
    string Name,
    DateTime CreatedAt,
    string? Avatar = null,
    string? BackgroundImage = null,
    string? Biography = null
) {
    public User(Entity user) : this(user.Id, user.Username, user.Name, user.CreatedAt, user.Avatar, user.BackgroundImage, user.Biography) { }
}

public record UserListItem(
    int Id,
    string Username,
    string Name,
    string? Avatar = null
) {
    public UserListItem(Entity user) : this(user.Id, user.Username, user.Name, user.Avatar) { }
}

public record CreateUser(
    string Username,
    string Name,
    string? Avatar = null,
    string? BackgroundImage = null,
    string? Biography = null
);