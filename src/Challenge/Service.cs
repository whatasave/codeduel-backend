namespace Challenge;

public class Service(Repository repository) {
    public Challenge FindById(int id) {
        return repository.FindById(id);
    }

    public IEnumerable<Challenge> FindAll() {
        return repository.FindAll();
    }

    public Challenge Create(CreateChallenge challenge, int ownerId) {
        return repository.Create(challenge, ownerId);
    }

    public Challenge Update(int id, CreateChallenge challenge, int ownerId) {
        return repository.Update(id, challenge, ownerId);
    }

    public Challenge Delete(int id) {
        return repository.Delete(id);
    }

    public Challenge FindRandom() {
        return repository.FindRandom();
    }
}