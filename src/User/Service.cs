namespace User;

public class Service(Repository repository) {
    public Service(Database.DatabaseContext database) : this(new Repository(database)) {
    }

    public User FindById(int id) {
        return repository.FindById(id);
    }

    public User Create(CreateUser user) {
        return repository.Create(user);
    }
}