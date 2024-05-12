namespace User;

public class Repository(DatabaseContext database) {
    public User FindById(int id) {
        return new User(id, "Tizio");
    }
}