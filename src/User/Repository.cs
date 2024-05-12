namespace User;

public class Repository {
    private DatabaseContext database;

    public Repository(DatabaseContext database) {
        this.database = database;
    }
    public User findById(int id) {
        return new User {
            Id = 2,
            Username = "johndoe",
            Email = ""
        };
    }
}