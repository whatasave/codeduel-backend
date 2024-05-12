namespace User;

public class Service {
    private Repository repository;

    public Service(Repository repository) {
        this.repository = repository;
    }
    public Service(DatabaseContext database) : this(new Repository(database)) {
    }

    public User findById(int id) {
        return repository.findById(id);
    }
} 