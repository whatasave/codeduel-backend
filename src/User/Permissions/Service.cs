using System.Threading.Tasks;

namespace Permissions;

public class Service(Repository repository) {
    public async Task<UserPermissions> FindByUserId(int userId) {
        return await repository.FindByUserId(userId);
    }

    public async Task<int> FindCompactByUserId(int userId) {
        return await repository.FindCompactByUserId(userId);
    }
}