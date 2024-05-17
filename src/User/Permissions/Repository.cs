namespace Permissions;

public class Repository(Database.DatabaseContext database) {
    public UserPermissions FindByUserId(int userId) {
        return new(0, new(false, false, false));
    }

    public int FindCompactByUserId(int userId) {
        return 0;
    }
}