namespace Game;

public class Service(Repository repository) {
    public GameWithUsersData GetGameResults(string uniqueId) {
        return repository.FindByUniqueId(uniqueId);
    }

    public IEnumerable<GameWithUsersData> GetAllGames() {
        return repository.GetAllGames();
    }

    public Game CreateGame(CreateGame request) {
        return repository.CreateGame(request);
    }

    public void UpdateSubmission(string uniqueId, UpdateSubmission request) {
        // TODO: get userId from token
        // return repository.UpdateGameSubmission(uniqueId, request);
    }

    public void EndGame(string uniqueId) {
        repository.EndGame(uniqueId);
    }

    public void ShareGameCode(int userId, ShareCodeRequest request) {
        repository.ShareCode(userId, request);
    }

    public IEnumerable<GameWithUserData> GetGamesByUserId(int userId) {
        return repository.GetGamesByUserId(userId);
    }
}
