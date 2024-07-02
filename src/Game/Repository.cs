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
        var user = database.GameUsers.Single(gameUser =>
            gameUser.UserId == submission.UserId &&
            gameUser.LobbyId == submission.LobbyId
        );
        user.Code = submission.Code;
        user.Language = submission.Language;
        user.TestsPassed = submission.TestsPassed;
        user.SubmittedAt = submission.SubmittedAt;
        database.SaveChanges();
    }

    public void EndGame(string uniqueId) {
        var game = database.Games.Single(game => game.UniqueId == uniqueId);
        if (game.Ended) {
            throw new Exception("Game already ended");
        }
        game.Ended = true;
        database.SaveChanges();
    }

    public bool ShareCode(int userId, ShareCodeRequest request) {
        var user = database.GameUsers.SingleOrDefault(gameUser => gameUser.UserId == userId && gameUser.LobbyId == request.LobbyId);
        if (user == null) throw new Exception("User not found");
        user.ShowCode = request.ShowCode;
        database.SaveChanges();
        return user.ShowCode;
    }

    public IEnumerable<GameWithUserData> GetGamesByUserId(int userId) {
        return (from game in database.Games
                join gameUser in database.GameUsers on game.Id equals gameUser.LobbyId
                where gameUser.UserId == userId
                select new GameWithUserData(gameUser)).AsEnumerable();
    }

    public IEnumerable<GameWithUsersData> GetAllGames() {
        var query = from game in database.Games
                    join gameUser in database.GameUsers on game.Id equals gameUser.LobbyId
                    select new { game, gameUser };

        var games = query.ToList().GroupBy(e => e.game.Id).Select(g => new GameWithUsersData(g.First().game, g.Select(e => e.gameUser)));
        return games;
    }

}
