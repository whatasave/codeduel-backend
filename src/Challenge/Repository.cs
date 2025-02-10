using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Challenge;

public class Repository(Database.DatabaseContext database) {
    public async Task<Challenge?> FindById(int id) {
        Console.WriteLine($"[Challenge Repository] Finding challenge by id {id}");
        return await (
            from challenge in database.Challenges
            join testCase in database.TestCases on challenge.Id equals testCase.ChallengeId
            where challenge.Id == id
            group testCase by challenge into testCases
            select new Challenge(testCases.Key, testCases.Count())
        ).SingleOrDefaultAsync();
    }

    public async Task<List<Challenge>> FindAll() {
        Console.WriteLine("[Challenge Repository] Finding all challenges");
        return await (
            from challenge in database.Challenges
            join testCase in database.TestCases on challenge.Id equals testCase.ChallengeId
            group testCase by challenge into testCases
            select new Challenge(testCases.Key, testCases.Count())
        ).ToListAsync();
    }

    public async Task<Challenge> Create(CreateChallenge challenge, int ownerId) {
        Console.WriteLine($"[Challenge Repository] Creating new challenge {challenge.Title}");
        var entity = await database.Challenges.AddAsync(new Entity {
            OwnerId = ownerId,
            Title = challenge.Title,
            Description = challenge.Description,
            Content = challenge.Content
        });
        await database.SaveChangesAsync();
        return new Challenge(entity.Entity, 0);
    }

    public async Task<bool> Update(int id, CreateChallenge challenge, int ownerId) {
        Console.WriteLine($"[Challenge Repository] Updating challenge {id}");
        var entity = database.Challenges.Update(new Entity {
            Id = id,
            OwnerId = ownerId,
            Title = challenge.Title,
            Description = challenge.Description,
            Content = challenge.Content
        });
        await database.SaveChangesAsync();
        return entity.State == EntityState.Modified;
    }

    public async Task<bool> Delete(int id) {
        Console.WriteLine($"[Challenge Repository] Deleting challenge {id}");
        var entity = await database.Challenges.SingleOrDefaultAsync(c => c.Id == id);
        if (entity == null) return false;

        database.Challenges.Remove(entity);
        await database.SaveChangesAsync();
        return true;
    }

    public async Task<ChallengeDetailed> FindRandom() {
        Console.WriteLine("[Challenge Repository] Finding random challenge");
        return new(await database.Challenges.OrderBy(_ => Guid.NewGuid()).Include(c => c.TestCases).FirstAsync());
    }
}
