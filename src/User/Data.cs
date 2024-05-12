namespace User;

public record User(
    int Id,
    string Username,
    string? Name = null,
    string? Avatar = null,
    string? BackgroundImage = null,
    string? Biography = null
);

public record UserListItem(
    int Id,
    string Username,
    string? Name = null,
    string? Avatar = null
);
