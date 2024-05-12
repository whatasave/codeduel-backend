namespace AuthGithub;

public class Repository(DatabaseContext database) {
    public Entity findById(int id) {
        return new Entity {
            Id = 2,
            UserId = "123456",
            Provider = "github",
            ProviderId = "123456",
        };
    }
}