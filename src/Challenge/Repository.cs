using Microsoft.EntityFrameworkCore;

namespace Challenge;

public class Repository(Func<Database.DatabaseContext> database) {
    public async Task<Challenge?> FindById(int id) {
        Console.WriteLine($"[Challenge Repository] Finding challenge by id {id}");
        var transaction = database();
        return await (
            from challenge in transaction.Challenges
            join testCase in transaction.TestCases on challenge.Id equals testCase.ChallengeId
            join owner in transaction.Users on challenge.OwnerId equals owner.Id
            where challenge.Id == id
            group testCase by challenge into testCases
            select new Challenge(testCases.Key, testCases.Count())
        ).SingleOrDefaultAsync();
    }

    public async Task<List<Challenge>> FindAll() {
        Console.WriteLine("[Challenge Repository] Finding all challenges");
        var transaction = database();
        return await (
            from challenge in transaction.Challenges
            join testCase in transaction.TestCases on challenge.Id equals testCase.ChallengeId
            join owner in transaction.Users on challenge.OwnerId equals owner.Id
            group testCase by challenge into testCases
            select new Challenge(testCases.Key, testCases.Count())
        ).ToListAsync();
    }

    public async Task<Challenge> Create(CreateChallenge challenge, int ownerId) {
        Console.WriteLine($"[Challenge Repository] Creating new challenge {challenge.Title}");
        var transaction = database();
        var entity = await transaction.Challenges.AddAsync(new Entity {
            OwnerId = ownerId,
            Title = challenge.Title,
            Description = challenge.Description,
            Content = challenge.Content
        });
        await transaction.SaveChangesAsync();
        return new Challenge(entity.Entity, 0);
    }

    public async Task<bool> Update(int id, CreateChallenge challenge, int ownerId) {
        Console.WriteLine($"[Challenge Repository] Updating challenge {id}");
        var transaction = database();
        var entity = transaction.Challenges.Update(new Entity {
            Id = id,
            OwnerId = ownerId,
            Title = challenge.Title,
            Description = challenge.Description,
            Content = challenge.Content
        });
        await transaction.SaveChangesAsync();
        return entity.State == EntityState.Modified;
    }

    public async Task<bool> Delete(int id) {
        Console.WriteLine($"[Challenge Repository] Deleting challenge {id}");
        var transaction = database();
        var entity = await transaction.Challenges.SingleOrDefaultAsync(c => c.Id == id);
        if (entity == null) return false;

        transaction.Challenges.Remove(entity);
        await transaction.SaveChangesAsync();
        return true;
    }

    public async Task<ChallengeDetailed> FindRandom() {
        Console.WriteLine("[Challenge Repository] Finding random challenge");
        return new(await database().Challenges.OrderBy(_ => Guid.NewGuid()).Include(c => c.TestCases).Include(c => c.Owner).FirstAsync());
    }
}
