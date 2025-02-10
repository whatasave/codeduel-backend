using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Auth;

public class Repository(Database.DatabaseContext database) {
    public async Task<Entity?> GetAuthByProviderAndId(string provider, int providerId) {
        Console.WriteLine($"[Auth Repository] Getting auth user by provider {provider} and id {providerId}");
        return await database.Authentications.FirstOrDefaultAsync(a => a.Provider == provider && a.ProviderId == providerId);
    }

    public async Task<UserAuth> Create(CreateAuth authUser) {
        Console.WriteLine($"[Auth Repository] Creating new auth user {authUser.Provider} {authUser.ProviderId}");
        var entry = await database.Authentications.AddAsync(new() {
            UserId = authUser.UserId,
            Provider = authUser.Provider,
            ProviderId = authUser.ProviderId
        });
        await database.SaveChangesAsync();
        return new UserAuth(entry.Entity);
    }

    public async Task SaveRefreshToken(int userId, string refreshToken) {
        Console.WriteLine($"[Auth Repository] Saving refresh token for user {userId}");
        var token = await database.RefreshTokens.FindAsync(userId);
        if (token != null) {
            token.RefreshToken = refreshToken;
        } else {
            await database.RefreshTokens.AddAsync(new() {
                UserId = userId,
                RefreshToken = refreshToken
            });
        }
        await database.SaveChangesAsync();
    }

    public async Task<bool> RemoveRefreshToken(int userId) {
        Console.WriteLine($"[Auth Repository] Removing refresh token for user {userId}");
        var refreshToken = await database.RefreshTokens.FindAsync(userId);
        if (refreshToken == null) return false;

        database.RefreshTokens.Remove(refreshToken);
        await database.SaveChangesAsync();
        return true;
    }
}
