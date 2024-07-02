using Microsoft.EntityFrameworkCore;

namespace Challenge;

public class Repository(Database.DatabaseContext database) {
    public Challenge FindById(int id) {
        return new Challenge(database.Challenges.Single(c => c.Id == id));
    }

    public IEnumerable<Challenge> FindAll() {
        return (from c in database.Challenges select new Challenge(c)).AsEnumerable();
    }

    public Challenge Create(CreateChallenge challenge, int ownerId) {
        var entity = database.Challenges.Add(new Entity {
            OwnerId = ownerId,
            Title = challenge.Title,
            Description = challenge.Description,
            Content = challenge.Content
        });
        database.SaveChanges();
        return new Challenge(entity.Entity);
    }

    public Challenge Update(int id, CreateChallenge challenge, int ownerId) {
        var entity = database.Challenges.Update(new Entity {
            Id = id,
            OwnerId = ownerId,
            Title = challenge.Title,
            Description = challenge.Description,
            Content = challenge.Content
        });
        database.SaveChanges();
        return new Challenge(entity.Entity);
    }

    public Challenge Delete(int id) {
        var entity = database.Challenges.Single(c => c.Id == id);
        database.Challenges.Remove(entity);
        database.SaveChanges();
        return new Challenge(entity);
    }

    public Challenge FindRandom() {
        return new Challenge(database.Challenges.OrderBy(c => Guid.NewGuid()).First());
    }
}
