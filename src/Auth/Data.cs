using Microsoft.AspNetCore.Mvc;

namespace Auth;

public record RefreshTokenPayload(int UserId);

public record AccessTokenPayload(int UserId, string Username, int PermissionsCompactNotation, Permissions.Permissions Permissions);

public record TokenPair(string AccessToken, string RefreshToken);

public record UserAuth(
    int UserId,
    string Provider,
    int ProviderId,
    DateTime CreatedAt,
    DateTime UpdatedAt
) {
    public UserAuth(Entity entity) : this(
        entity.UserId,
        entity.Provider,
        entity.ProviderId,
        entity.CreatedAt,
        entity.UpdatedAt
    ) { }
}

public record VerifyTokenPayload(
    string Token
);

public record LobbyUser(
    int Id,
    string Username,
    string Name,
    string? Avatar = null,
    string? BackgroundImage = null
) {
    public LobbyUser(User.User user) : this(user.Id, user.Username, user.Name, user.Avatar, user.BackgroundImage) { }
}


public record CreateAuth(int UserId, string Provider, int ProviderId);
