namespace Game;

public class Service(Repository repository) {
    public Service(Database.DatabaseContext database) : this(new Repository(database)) {}

    public GameWithUsersData GetGameResults(string uniqueId) {
        return repository.FindByUniqueId(uniqueId);
    }

    public IEnumerable<GameWithUsersData> GetAllGames() {
        return repository.GetAllGames();
    }

    public Game CreateGame(CreateGame request) {
        return repository.CreateGame(request);
    }

    public bool SubmitGameAction(string uniqueId, UpdateSubmission request) {
        return repository.SubmitGameAction(uniqueId, request);
    }

    public bool EndGame(string uniqueId) {
        return repository.EndGame(uniqueId);
    }

    public bool ShareGameCode(string uniqueId, int userId, ShareCodeRequest request) {
        return repository.ShareGameCode(uniqueId, userId, request);
    }

    public IEnumerable<GameWithUsersData> GetGamesByUsername(string username) {
        return repository.GetGamesByUsername(username);
    }

}
