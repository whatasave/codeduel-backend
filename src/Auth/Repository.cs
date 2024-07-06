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

    public void SaveRefreshToken(int userId, string refreshToken) {
        var token = database.RefreshTokens.Find(userId);
        if (token != null) {
            token.RefreshToken = refreshToken;
        }
        else {
            database.RefreshTokens.Add(new() {
                UserId = userId,
                RefreshToken = refreshToken
            });
        }
        database.SaveChanges();
    }

    public bool RemoveRefreshToken(int userId) {
        var refreshToken = database.RefreshTokens.Find(userId);
        if (refreshToken == null) {
            return false;
        }
        database.RefreshTokens.Remove(refreshToken);
        database.SaveChanges();
        return true;
    }
}
