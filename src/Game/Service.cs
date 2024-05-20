namespace Game;

public class Service(Repository repository) {
    public Service(Database.DatabaseContext database) : this(new Repository(database)) {
    }

    public Game FindById(int id) {
        return repository.FindById(id);
    }
}