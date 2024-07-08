namespace Challenge;

public class Service(Repository repository) {
    public Challenge? FindById(int id) {
        return repository.FindById(id);
    }

    public IEnumerable<Challenge> FindAll() {
        return repository.FindAll();
    }

    public Challenge Create(CreateChallenge challenge, int ownerId) {
        return repository.Create(challenge, ownerId);
    }

    public bool Update(int id, CreateChallenge challenge, int ownerId) {
        return repository.Update(id, challenge, ownerId);
    }

    public bool Delete(int id) {
        return repository.Delete(id);
    }

    public ChallengeDetailed FindRandom() {
        return repository.FindRandom();
    }
}