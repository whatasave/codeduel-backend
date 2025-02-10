using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Permissions;

public class Repository(Database.DatabaseContext database) {
    public async Task<UserPermissions> FindByUserId(int userId) {
        Console.WriteLine($"[Permissions Repository] Getting permissions for user {userId}");
        return await database.Permissions.Where(p => p.UserId == userId).Select(p => new UserPermissions(p)).SingleAsync();
    }

    public async Task<int> FindCompactByUserId(int userId) {
        Console.WriteLine($"[Permissions Repository] Getting compact permissions for user {userId}");
        return await database.Permissions.Where(p => p.UserId == userId).Select(p => p.CompactNotation).SingleOrDefaultAsync();
    }
}