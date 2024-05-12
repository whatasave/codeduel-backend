namespace Challenge;

public class Repository {
    private DatabaseContext database;

    public Repository(DatabaseContext database) {
        this.database = database;
    }
    public Challenge findById(int id) {
        return new Challenge {
            Id = 2,
            Username = "johndoe",
            Email = ""
        };
    }
}