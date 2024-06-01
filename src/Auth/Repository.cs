namespace Auth;

public class Repository(Database.DatabaseContext database) {
    public Entity? GetAuthByProviderAndId(string provider, int providerId) {
        Console.WriteLine($"[Auth Repository] Getting auth user by provider {provider} and id {providerId}");
        return database.Authentications.FirstOrDefault(a => a.Provider == provider && a.ProviderId == providerId);
    }
    public UserAuth Create(CreateAuth authUser) {
        Console.WriteLine($"[Auth Repository] Creating new auth user {authUser.Provider} {authUser.ProviderId}");
        var entry = database.Authentications.Add(new() {
            UserId = authUser.UserId,
            Provider = authUser.Provider,
            ProviderId = authUser.ProviderId
        });
        database.SaveChanges();
        return new UserAuth(entry.Entity);
    }
    public Entity? FindById(int id) {
        return database.Authentications.Find(id);
    }
    public void SaveRefreshToken(int userId, string refreshToken) {
        Console.WriteLine($"[Auth Repository] Saving refresh token for user {userId}");
        User.Entity? user = database.Users.Find(userId) ?? throw new Exception("User not found");

        if (database.RefreshTokens.Find(userId) != null) throw new Exception("Refresh token already exists");

        Console.WriteLine($"[Auth Repository] Saving {refreshToken} for user {userId}");
        database.RefreshTokens.Add(new() {
            User = user,
            RefreshToken = refreshToken
        });
        database.SaveChanges();
    }
    public void RemoveRefreshToken(int userId) {
        Console.WriteLine($"[Auth Repository] Removing refresh token for user {userId}");
        RefreshTokenEntity? refreshToken = database.RefreshTokens.Find(userId) ?? throw new Exception("Refresh token not found");

        database.RefreshTokens.Remove(refreshToken);
        database.SaveChanges();
    }
}
