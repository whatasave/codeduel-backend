using Microsoft.EntityFrameworkCore;

namespace Challenge;

public class Repository(Database.DatabaseContext database) {
    public Challenge? FindById(int id) {
        return (from challenge in database.Challenges
                join testCase in database.TestCases on challenge.Id equals testCase.ChallengeId
                where challenge.Id == id
                group testCase by challenge into testCases
                select new Challenge(testCases.Key, testCases.Count())).SingleOrDefault();
    }

    public IEnumerable<Challenge> FindAll() {
        return (from challenge in database.Challenges
                join testCase in database.TestCases on challenge.Id equals testCase.ChallengeId
                group testCase by challenge into testCases
                select new Challenge(testCases.Key, testCases.Count())).AsEnumerable();
    }

    public Challenge Create(CreateChallenge challenge, int ownerId) {
        var entity = database.Challenges.Add(new Entity {
            OwnerId = ownerId,
            Title = challenge.Title,
            Description = challenge.Description,
            Content = challenge.Content
        });
        database.SaveChanges();
        return new Challenge(entity.Entity, 0);
    }

    public bool Update(int id, CreateChallenge challenge, int ownerId) {
        var entity = database.Challenges.Update(new Entity {
            Id = id,
            OwnerId = ownerId,
            Title = challenge.Title,
            Description = challenge.Description,
            Content = challenge.Content
        });
        database.SaveChanges();
        return entity.State == EntityState.Modified;
    }

    public bool Delete(int id) {
        var entity = database.Challenges.Single(c => c.Id == id);
        if (entity == null) return false;
        database.Challenges.Remove(entity);
        database.SaveChanges();
        return true;
    }

    public ChallengeDetailed FindRandom() {
        return new(database.Challenges.OrderBy(_ => Guid.NewGuid()).Include(c => c.TestCases).First());
    }
}
