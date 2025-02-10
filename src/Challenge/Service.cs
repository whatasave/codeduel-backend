using System.Threading.Tasks;

namespace Challenge;

public class Service(Repository repository) {
    public async Task<Challenge?> FindById(int id) {
        return await repository.FindById(id);
    }

    public async Task<IEnumerable<Challenge>> FindAll() {
        return await repository.FindAll();
    }

    public async Task<Challenge> Create(CreateChallenge challenge, int ownerId) {
        return await repository.Create(challenge, ownerId);
    }

    public async Task<bool> Update(int id, CreateChallenge challenge, int ownerId) {
        return await repository.Update(id, challenge, ownerId);
    }

    public async Task<bool> Delete(int id) {
        return await repository.Delete(id);
    }

    public async Task<ChallengeDetailed> FindRandom() {
        return await repository.FindRandom();
    }
}