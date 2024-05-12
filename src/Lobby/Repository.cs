namespace Lobby;

public class Repository {
    private DatabaseContext database;

    public Repository(DatabaseContext database) {
        this.database = database;
    }
    public Lobby findById(int id) {
        return new Lobby {
            Id = 2,
            Username = "johndoe",
            Email = ""
        };
    }
}