namespace Challenge;

public class Service(Repository repository) {
    public Service(Database.DatabaseContext database) : this(new Repository(database)) {
    }

    public Challenge FindById(int id) {
        return repository.FindById(id);
    }

    public IEnumerable<Challenge> FindAll() {
        return repository.FindAll();
    }

    public Challenge Create(Challenge challenge) {
        return repository.Create(challenge);
    }

    public Challenge Update(Challenge challenge) {
        return repository.Update(challenge);
    }

    public Challenge Delete(int id) {
        return repository.Delete(id);
    }

    public Challenge FindRandom() {
        return repository.FindRandom();
    }
}