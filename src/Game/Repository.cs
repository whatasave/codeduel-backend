namespace Game;

public class Repository(Database.DatabaseContext database) {
    public GameWithUsersData FindByUniqueId(string uniqueId) {
        var query = from game in database.Games
                    join gameUser in database.GameUsers on game.Id equals gameUser.LobbyId
                    where game.UniqueId == uniqueId
                    select new { game, gameUser };
        return new(query.First().game, query.Select(e => e.gameUser).AsEnumerable());
    }

    public Game CreateGame(CreateGame game) {
        var entry = database.Games.Add(new Entity {
            UniqueId = game.UniqueId,
            ChallengeId = game.ChallengeId,
            OwnerId = game.OwnerId,
            Ended = game.Ended,
            ModeId = game.ModeId,
            MaxPlayers = game.MaxPlayers,
            GameDuration = game.GameDuration,
            AllowedLanguages = game.AllowedLanguages
        });
        database.GameUsers.AddRange(game.Users.Select(userId => new UserEntity {
            LobbyId = entry.Entity.Id,
            UserId = userId,
        }));
        database.SaveChanges();
        return new Game(entry.Entity);
    }

    public void UpdateGameSubmission(UpdateSubmission submission) {
        var user = database.GameUsers.Single(gameUser => gameUser.UserId == submission.UserId && gameUser.LobbyId == submission.LobbyId);
        user.Code = submission.Code;
        user.Language = submission.Language;
        user.TestsPassed = submission.TestsPassed;
        user.SubmittedAt = submission.SubmittedAt;
        database.SaveChanges();
    }

    public void EndLobby(string uniqueId) {
        var game = database.Games.Single(game => game.UniqueId == uniqueId);
        if (game.Ended) {
            throw new Exception("Game already ended");
        }
        game.Ended = true;
        database.SaveChanges();
    }

    public void ShareCode(int userId, int lobbyId, bool showCode) {
        var user = database.GameUsers.Single(gameUser => gameUser.UserId == userId && gameUser.LobbyId == lobbyId);
        user.ShowCode = showCode;
        database.SaveChanges();
    }

    public IEnumerable<GameWithUserData> GetMatchesById(int userId) {
        return (from game in database.Games
                join gameUser in database.GameUsers on game.Id equals gameUser.LobbyId
                where gameUser.UserId == userId
                select new GameWithUserData(gameUser)).AsEnumerable();
    }


}