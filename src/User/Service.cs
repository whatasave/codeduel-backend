namespace User;

public class Service(Repository repository) {
    public Service(Database.DatabaseContext database) : this(new Repository(database)) {
    }

    public User? FindByUsername(string username) {
        return repository.FindByUsername(username);
    }

    public User? FindById(int id) {
        return repository.FindById(id);
    }

    public IEnumerable<UserListItem> FindAll() {
        return repository.FindAll();
    }

    public User Create(CreateUser user) {
        return repository.Create(user);
    }
}