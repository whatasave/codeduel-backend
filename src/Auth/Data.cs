using Permissions;

namespace Auth;

public record RefreshTokenPayload(int UserId);

public record AccessTokenPayload(int UserId, string Username, UserPermissions Permissions);