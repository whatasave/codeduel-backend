namespace Challenge;

public class Repository(Database.DatabaseContext database) {
    public Challenge FindById(int id) {
        return (from c in database.Challenges where c.Id == id select new Challenge(c)).Single();
    }
}