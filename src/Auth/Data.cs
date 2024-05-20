using Permissions;

namespace Auth;

public record RefreshTokenPayload(int UserId);

public record AccessTokenPayload(int UserId, string Username, UserPermissions Permissions);

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

public record CreateAuth(int UserId, string Provider, int ProviderId);
