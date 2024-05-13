namespace AuthGithub;

public class Repository(Database.DatabaseContext database) {
    public Entity GetAuthByProviderAndId(string provider, int id) {
        return new Entity {
            Id = 1,
            UserId = 1,
            Provider = "github",
            ProviderId = 123456,
        };
    }

    public Entity Create(Entity entity) {
        // Save entity to database
        return entity;
    }

    public Entity FindById(int id) {
        return new Entity {
            Id = 2,
            UserId = 2,
            Provider = "github",
            ProviderId = 123456,
        };
    }
}