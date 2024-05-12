namespace User;

public class Repository(DatabaseContext database) {
    public Entity findById(int id) {
        return new Entity {
            Id = 2,
            Username = "johndoe",
            Email = ""
        };
    }
}