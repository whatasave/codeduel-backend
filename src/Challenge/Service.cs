namespace Challenge;

public class Service(Repository repository) {
    public Service(Database.DatabaseContext database) : this(new Repository(database)) {
    }

    public Challenge FindById(int id) {
        return repository.FindById(id);
    }
}