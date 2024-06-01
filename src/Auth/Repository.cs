namespace Auth;

public class Repository(Database.DatabaseContext database) {
    public Entity? GetAuthByProviderAndId(string provider, int providerId) {
        return database.Authentications.FirstOrDefault(a => a.Provider == provider && a.ProviderId == providerId);
    }
    public UserAuth Create(CreateAuth authUser) {
        var entry = database.Authentications.Add(new() {
            UserId = authUser.UserId,
            Provider = authUser.Provider,
            ProviderId = authUser.ProviderId
        });
        return new UserAuth(entry.Entity);
    }
    public Entity? FindById(int id) {
        return database.Authentications.Find(id);
    }
    public void SaveRefreshToken(int userId, string refreshToken) {
        User.Entity? user = database.Users.Find(userId) ?? throw new Exception("User not found");

        if (database.RefreshTokens.Find(userId) != null) throw new Exception("Refresh token already exists");

        database.RefreshTokens.Add(new() {
            Id = -1,
            User = user,
            RefreshToken = refreshToken
        });
    }
    public void RemoveRefreshToken(int userId) {
        RefreshTokenEntity? refreshToken = database.RefreshTokens.Find(userId) ?? throw new Exception("Refresh token not found");

        database.RefreshTokens.Remove(refreshToken);
    }
}
