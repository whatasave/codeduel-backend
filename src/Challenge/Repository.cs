namespace Challenge;

public class Repository(Database.DatabaseContext database) {
    public Challenge FindById(int id) {
        return new(1, new User.UserListItem(1, ""), "title", "description", "content");
    }
}