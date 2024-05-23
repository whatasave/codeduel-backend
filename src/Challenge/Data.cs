namespace Challenge;

public record Challenge(
    int Id,
    User.UserListItem Owner,
    string Title,
    string Description,
    string Content
) {
    public Challenge(Entity entity) : this(
        entity.Id,
        new(entity.Owner!),
        entity.Title,
        entity.Description,
        entity.Content
    ) { }
}

public record CreateChallenge(
    string Title,
    string Description,
    string Content
);