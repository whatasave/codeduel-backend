namespace Game;

public class Repository(Database.DatabaseContext database) {
    public Game FindById(int id) {
        return new(1, "uuid", new(1, new(1, ""), "title", "description", "content"), 1, false, "mode", 1, 1, "allowed_languages");
    }
}