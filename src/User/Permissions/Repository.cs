namespace Permissions;

public class Repository(Database.DatabaseContext database) {
    public UserPermissions FindByUserId(int userId) {
        return (from permissions in database.Permissions where permissions.UserId == userId select new UserPermissions(permissions)).Single();
    }

    public int FindCompactByUserId(int userId) {
        return (from permissions in database.Permissions where permissions.UserId == userId select permissions.CompactNotation).Single();
    }
}