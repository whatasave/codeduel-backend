namespace User;

public class Repository(Database.DatabaseContext database) {
    public User FindById(int id) {
        return (from user in database.Users where user.Id == id select new User(user)).Single();
    }

    public User? FindByUsername(string username) {
        return (from user in database.Users where user.Username == username select new User(user)).SingleOrDefault();
    }

    public IEnumerable<UserListItem> FindAll() {
        return from user in database.Users select new UserListItem(user);
    }

    public User Create(CreateUser user) {
        var entry = database.Users.Add(new Entity {
            Username = user.Username,
            Name = user.Name,
            Avatar = user.Avatar,
            BackgroundImage = user.BackgroundImage,
            Biography = user.Biography
        });
        database.SaveChanges();
        return new User(entry.Entity);
    }

    public void DeleteById(int id) {
        var user = database.Users.Find(id) ?? throw new Exception("User not found");
        user.Username = $"deleted-{id}";
        user.Name = $"deleted-{id}";
        user.Avatar = null;
        user.BackgroundImage = null;
        user.Biography = null;
    }
}