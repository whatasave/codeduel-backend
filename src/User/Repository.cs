namespace User;

public class Repository(Database.DatabaseContext database) {
    public User FindById(int id) {
        return new User(id, "Tizio");
    }

    public User Create(User user) {
        return user;
    }
}