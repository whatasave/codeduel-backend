namespace Challenge;

public record Challenge(
    int Id,
    User.UserListItem Owner,
    string Title,
    string Description,
    string Content
);