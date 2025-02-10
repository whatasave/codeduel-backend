using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Game;

public class Repository(Database.DatabaseContext database) {
    public async Task<GameWithUsersData?> FindByUniqueId(string uniqueId) {
        Console.WriteLine($"[Game Repository] Getting game by unique id {uniqueId}");
        return await database.Games
            .Where(g => g.UniqueId == uniqueId)
            .Include(g => g.Challenge)
            .Include(g => g.Mode)
            .Include(g => g.Users)
            .Select(g => new GameWithUsersData(g, (from testCase in database.TestCases where testCase.ChallengeId == g.ChallengeId select testCase).Count()))
            .FirstOrDefaultAsync();
    }

    public async Task CreateGame(CreateGame game) {
        Console.WriteLine($"[Game Repository] Creating game with unique id {game.UniqueId}");
        var entry = await database.Games.AddAsync(new Entity {
            UniqueId = game.UniqueId,
            ChallengeId = game.ChallengeId,
            OwnerId = game.OwnerId,
            Ended = game.Ended,
            ModeId = game.ModeId,
            MaxPlayers = game.MaxPlayers,
            GameDuration = game.GameDuration,
            AllowedLanguages = game.AllowedLanguages
        });
        await database.GameUsers.AddRangeAsync(game.Users.Select(userId => new UserEntity {
            GameId = entry.Entity.Id,
            UserId = userId,
        }));
        await database.SaveChangesAsync();
    }

    public async Task UpdateGameSubmission(UpdateSubmission submission) {
        Console.WriteLine($"[Game Repository] Updating submission for user {submission.UserId} in game {submission.GameId}");
        var user = await database.GameUsers.SingleAsync(gameUser =>
            gameUser.UserId == submission.UserId &&
            gameUser.GameId == submission.GameId
        );
        user.Code = submission.Code;
        user.Language = submission.Language;
        user.TestsPassed = submission.TestsPassed;
        user.SubmittedAt = submission.SubmittedAt;
        await database.SaveChangesAsync();
    }

    public async Task EndGame(string uniqueId) {
        Console.WriteLine($"[Game Repository] Ending game with unique id {uniqueId}");
        var game = await database.Games.SingleAsync(game => game.UniqueId == uniqueId);
        if (game.Ended) throw new Exception("Game already ended");

        game.Ended = true;
        await database.SaveChangesAsync();
    }

    public async Task<bool> ShareCode(int userId, ShareCodeRequest request) {
        Console.WriteLine($"[Game Repository] Sharing code for user {userId} in game {request.LobbyId}");
        var user = await database.GameUsers.SingleOrDefaultAsync(gameUser => gameUser.UserId == userId && gameUser.GameId == request.LobbyId) ?? throw new Exception("User not found");
        user.ShowCode = request.ShowCode;
        await database.SaveChangesAsync();
        return user.ShowCode;
    }

    public async Task<List<GameWithUserData>> GetGamesByUserId(int userId) {
        Console.WriteLine($"[Game Repository] Getting games for user {userId}");
        var games = await database.GameUsers
            .Where(gameUser => gameUser.UserId == userId)
            .Include(gameUser => gameUser.User)
            .Include(gameUser => gameUser.Game)
            .Include(gameUser => gameUser.Game!.Challenge)
            .Include(gameUser => gameUser.Game!.Mode)
            .Select(gameUser => new {
                GameUser = gameUser,
                TestCaseCount = database.TestCases.Count(testCase => testCase.ChallengeId == gameUser.Game!.Challenge!.Id)
            }).ToListAsync();

        return games.Select(x => new GameWithUserData(x.GameUser, x.TestCaseCount)).ToList();
    }

    public async Task<List<GameWithUsersData>> GetAllGames() {
        Console.WriteLine("[Game Repository] Getting all games");
        var testCaseCounts = await database.TestCases
            .GroupBy(tc => tc.ChallengeId)
            .Select(group => new {
                ChallengeId = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

        var countsDict = testCaseCounts.ToDictionary(x => x.ChallengeId, x => x.Count);

        var games = await database.Games
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
