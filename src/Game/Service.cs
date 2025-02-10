using System.Threading.Tasks;

namespace Game;

public class Service(Repository repository) {
    public async Task<GameWithUsersData?> GetGameResults(string uniqueId) {
        return await repository.FindByUniqueId(uniqueId);
    }

    public async Task<IEnumerable<GameWithUsersData>> GetAllGames() {
        return await repository.GetAllGames();
    }

    public async Task CreateGame(CreateGame request) {
        await repository.CreateGame(request);
    }

    public void UpdateSubmission(string uniqueId, UpdateSubmission request) {
        // TODO: get userId from token
        // return repository.UpdateGameSubmission(uniqueId, request);
    }

    public async Task EndGame(string uniqueId) {
        await repository.EndGame(uniqueId);
    }

    public async Task ShareGameCode(int userId, ShareCodeRequest request) {
        await repository.ShareCode(userId, request);
    }

    public async Task<IEnumerable<GameWithUserData>> GetGamesByUserId(int userId) {
        return await repository.GetGamesByUserId(userId);
    }
}
