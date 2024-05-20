using Permissions;

namespace Auth;

public record RefreshTokenPayload(int UserId);

public record AccessTokenPayload(int UserId, string Username, UserPermissions Permissions);

public record TokenPair(string AccessToken, string RefreshToken);
