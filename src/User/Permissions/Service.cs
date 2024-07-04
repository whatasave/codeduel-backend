namespace Permissions;

public class Service(Repository repository) {
    public UserPermissions FindByUserId(int userId) {
        return repository.FindByUserId(userId);
    }

    public int FindCompactByUserId(int userId) {
        return repository.FindCompactByUserId(userId);
    }
}