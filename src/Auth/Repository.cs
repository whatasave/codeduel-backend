using Microsoft.EntityFrameworkCore;

namespace Auth;

public class Repository(Func<Database.DatabaseContext> database) {
    public async Task<Entity?> GetAuthByProviderAndId(string provider, int providerId) {
        Console.WriteLine($"[Auth Repository] Getting auth user by provider {provider} and id {providerId}");
        return await database().Authentications.FirstOrDefaultAsync(a => a.Provider == provider && a.ProviderId == providerId);
    }

    public async Task<UserAuth> Create(CreateAuth authUser) {
        Console.WriteLine($"[Auth Repository] Creating new auth user {authUser.Provider} {authUser.ProviderId}");
        var transaction = database();
        var entry = await transaction.Authentications.AddAsync(new() {
            UserId = authUser.UserId,
            Provider = authUser.Provider,
            ProviderId = authUser.ProviderId
        });
        await transaction.SaveChangesAsync();
        return new UserAuth(entry.Entity);
    }

    public async Task SaveRefreshToken(int userId, string refreshToken) {
        Console.WriteLine($"[Auth Repository] Saving refresh token for user {userId}");
        var transaction = database();
        var token = await transaction.RefreshTokens.FindAsync(userId);
        if (token != null) {
            token.RefreshToken = refreshToken;
        }
        else {
            await transaction.RefreshTokens.AddAsync(new() {
                UserId = userId,
                RefreshToken = refreshToken
            });
        }
        await transaction.SaveChangesAsync();
    }

    public async Task<bool> RemoveRefreshToken(int userId) {
        Console.WriteLine($"[Auth Repository] Removing refresh token for user {userId}");
        var transaction = database();
        var refreshToken = await transaction.RefreshTokens.FindAsync(userId);
        if (refreshToken == null) return false;

        transaction.RefreshTokens.Remove(refreshToken);
        await transaction.SaveChangesAsync();
        return true;
    }
}
