namespace Game;

public class Service(Repository repository) {
    public Service(Database.DatabaseContext database) : this(
        new Repository(database)
    ) {}

    public GameWithUsersData FindByUniqueId(string uniqueId) {
        return repository.FindByUniqueId(uniqueId);
    }
}
