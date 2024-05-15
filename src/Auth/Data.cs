namespace Auth;

public record RefreshTokenPayload(int Sub, DateTime ExpireAt);