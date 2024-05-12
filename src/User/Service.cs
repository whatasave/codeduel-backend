namespace User;

public class Service(Repository repository) {
    public Service(DatabaseContext database) : this(new Repository(database)) {
    }

    public User FindById(int id) {
        return repository.FindById(id);
    }

    public User Create(User user) {
        return repository.Create(user);
    }
}