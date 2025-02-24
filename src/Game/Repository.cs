using Microsoft.EntityFrameworkCore;

namespace Game;

public class Repository(Func<Database.DatabaseContext> database) {
    public async Task<GameWithUsersData?> FindByUniqueId(string uniqueId) {
        Console.WriteLine($"[Game Repository] Getting game by unique id {uniqueId}");
        var transaction = database();
        return await transaction.Games
            .Where(g => g.UniqueId == uniqueId)
            .Include(g => g.Challenge)
            .Include(g => g.Mode)
            .Include(g => g.Users)
            .Include(g => g.Owner)
            .Select(g => new GameWithUsersData(g, (from testCase in transaction.TestCases where testCase.ChallengeId == g.ChallengeId select testCase).Count()))
            .FirstOrDefaultAsync();
    }

    public async Task CreateGame(CreateGame game) {
        Console.WriteLine($"[Game Repository] Creating game with unique id {game.UniqueId}");
        var transaction = database();
        var entry = await transaction.Games.AddAsync(new Entity {
            UniqueId = game.UniqueId,
            ChallengeId = game.ChallengeId,
            OwnerId = game.OwnerId,
            Ended = game.Ended,
            ModeId = game.ModeId,
            MaxPlayers = game.MaxPlayers,
            GameDuration = game.GameDuration,
            AllowedLanguages = game.AllowedLanguages
        });
        await transaction.SaveChangesAsync();
        await transaction.GameUsers.AddRangeAsync(game.Users.Select(userId => new UserEntity {
            GameId = entry.Entity.Id,
            UserId = userId,
        }));
        await transaction.SaveChangesAsync();
    }

    public async Task UpdateGameSubmission(UpdateSubmission submission) {
        Console.WriteLine($"[Game Repository] Updating submission for user {submission.UserId} in game {submission.GameId}");
        var transaction = database();
        var user = await transaction.GameUsers.SingleAsync(gameUser =>
            gameUser.UserId == submission.UserId &&
            gameUser.GameId == submission.GameId
        );
        user.Code = submission.Code;
        user.Language = submission.Language;
        user.TestsPassed = submission.TestsPassed;
        user.SubmittedAt = submission.SubmittedAt;
        await transaction.SaveChangesAsync();
    }

    public async Task EndGame(string uniqueId) {
        Console.WriteLine($"[Game Repository] Ending game with unique id {uniqueId}");
        var transaction = database();
        var game = await transaction.Games.SingleAsync(game => game.UniqueId == uniqueId);
        if (game.Ended) throw new Exception("Game already ended");

        game.Ended = true;
        await transaction.SaveChangesAsync();
    }

    public async Task<bool> ShareCode(int userId, ShareCodeRequest request) {
        Console.WriteLine($"[Game Repository] Sharing code for user {userId} in game {request.LobbyId}");
        var transaction = database();
        var user = await transaction.GameUsers.SingleOrDefaultAsync(gameUser => gameUser.UserId == userId && gameUser.GameId == request.LobbyId) ?? throw new Exception("User not found");
        user.ShowCode = request.ShowCode;
        await transaction.SaveChangesAsync();
        return user.ShowCode;
    }

    public async Task<List<GameWithUserData>> GetGamesByUserId(int userId) {
        Console.WriteLine($"[Game Repository] Getting games for user {userId}");
        var transaction = database();
        var games = await transaction.GameUsers
            .Where(gameUser => gameUser.UserId == userId)
            .Include(gameUser => gameUser.User)
            .Include(gameUser => gameUser.Game)
            .Include(gameUser => gameUser.Game!.Challenge)
            .Include(gameUser => gameUser.Game!.Mode)
            .Select(gameUser => new {
                GameUser = gameUser,
                TestCaseCount = transaction.TestCases.Count(testCase => testCase.ChallengeId == gameUser.Game!.Challenge!.Id)
            }).ToListAsync();

        return games.Select(x => new GameWithUserData(x.GameUser, x.TestCaseCount)).ToList();
    }

    public async Task<List<GameWithUsersData>> GetAllGames() {
        Console.WriteLine("[Game Repository] Getting all games");
        var transaction = database();
        var testCaseCounts = await transaction.TestCases
            .GroupBy(tc => tc.ChallengeId)
            .Select(group => new {
                ChallengeId = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

        var countsDict = testCaseCounts.ToDictionary(x => x.ChallengeId, x => x.Count);

        var games = await transaction.Games
            .Include(g => g.Challenge)
            .Include(g => g.Mode)
            .Include(g => g.Users)
            .ToListAsync();

        var result = games.Select(g => new GameWithUsersData(g,
            countsDict.TryGetValue(g.ChallengeId, out int count) ? count : 0
        )).ToList();

        return result;
    }

}
