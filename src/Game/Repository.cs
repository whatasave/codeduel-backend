using Microsoft.EntityFrameworkCore;

namespace Game;

public class Repository(Database.DatabaseContext database) {
    public GameWithUsersData? FindByUniqueId(string uniqueId) {
        return database.Games
            .Where(g => g.UniqueId == uniqueId)
            .Include(g => g.Challenge)
            .Include(g => g.Mode)
            .Include(g => g.Users)
            .Select(g => new GameWithUsersData(g, (from testCase in database.TestCases where testCase.ChallengeId == g.ChallengeId select testCase).Count()))
            .FirstOrDefault();
    }

    public void CreateGame(CreateGame game) {
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
            GameId = entry.Entity.Id,
            UserId = userId,
        }));
        database.SaveChanges();
    }

    public void UpdateGameSubmission(UpdateSubmission submission) {
        var user = database.GameUsers.Single(gameUser =>
            gameUser.UserId == submission.UserId &&
            gameUser.GameId == submission.GameId
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
        var user = database.GameUsers.SingleOrDefault(gameUser => gameUser.UserId == userId && gameUser.GameId == request.LobbyId) ?? throw new Exception("User not found");
        user.ShowCode = request.ShowCode;
        database.SaveChanges();
        return user.ShowCode;
    }

    public IEnumerable<GameWithUserData> GetGamesByUserId(int userId) {
        return database.GameUsers
            .Where(gameUser => gameUser.UserId == userId)
            .Include(gameUser => gameUser.User)
            .Include(gameUser => gameUser.Game)
            .Include(gameUser => gameUser.Game!.Challenge)
            .Include(gameUser => gameUser.Game!.Mode)
            .Select(gameUser => new GameWithUserData(gameUser, (from testCase in database.TestCases where testCase.ChallengeId == gameUser.Game!.Challenge!.Id select testCase).Count()))
            .ToList();
    }

    public IEnumerable<GameWithUsersData> GetAllGames() {
        return database.Games
            .Include(g => g.Challenge)
            .Include(g => g.Mode)
            .Include(g => g.Users)
            .Select(g => new GameWithUsersData(g, (from testCase in database.TestCases where testCase.ChallengeId == g.ChallengeId select testCase).Count()))
            .AsEnumerable();
    }
}
