namespace Lobby;

public class Service(Repository repository) {
    public Service(Database.DatabaseContext database) : this(new Repository(database)) {
    }

    public Lobby FindById(int id) {
        return repository.FindById(id);
    }
}