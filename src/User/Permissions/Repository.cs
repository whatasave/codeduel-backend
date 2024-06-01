namespace Permissions;

public class Repository(Database.DatabaseContext database) {
    public UserPermissions FindByUserId(int userId) {
        return database.Permissions.Where(p => p.UserId == userId).Select(p => new UserPermissions(p)).Single();
    }

    public int FindCompactByUserId(int userId) {
        return database.Permissions.Where(p => p.UserId == userId).Select(p => p.CompactNotation).SingleOrDefault();
    }
}