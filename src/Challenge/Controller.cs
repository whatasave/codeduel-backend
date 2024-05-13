namespace Challenge;
public class Controller(Service service) {
    public Controller(Database.DatabaseContext database) : this(new Service(database)) {
    }

    public void SetupRoutes(RouteGroupBuilder group) {
        group.MapGet("/{id}", FindById);
    }

    public Challenge FindById(int id) {
        return service.FindById(id);
    }
}
